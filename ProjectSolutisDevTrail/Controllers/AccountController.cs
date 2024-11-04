using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectSolutisDevTrail.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data;
using ProjectSolutisDevTrail.Data.Dtos;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Text;

namespace ProjectSolutisDevTrail.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ISendGridClient _sendGridClient; // Usando SendGrid
    private readonly EventoContext _context; // Contexto do banco

    public AccountController(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        IConfiguration configuration,
        ISendGridClient sendGridClient,
        EventoContext context) // Injetando o contexto do evento
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _sendGridClient = sendGridClient; // Inicializando o SendGrid Client
        _context = context; // Inicializando o contexto
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verifica se o e-mail já está registrado
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            // Se o usuário com o e-mail já existe, retorna uma mensagem de erro clara
            return BadRequest(new { message = "O e-mail fornecido já está registrado." });
        }

        var user = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            Nome = model.Nome,
            Sobrenome = model.Sobrenome,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Criar o participante com as informações do usuário
        var participante = new Participante
        {
            Nome = model.Nome,
            Sobrenome = model.Sobrenome, 
            Email = model.Email,
            UsuarioId = user.Id // Relaciona o participante com o usuário
        };

        // Adicionar o participante ao contexto e salvar
        _context.Participantes.Add(participante);
        await _context.SaveChangesAsync();

        // Gerar token de confirmação de e-mail
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

        // Enviar e-mail de confirmação
        await SendConfirmationEmail(user.Email, confirmationLink);

        return Ok(new
        {
            message = "Registro bem-sucedido! Verifique seu e-mail para confirmar sua conta.",
            participanteId = participante.Id // Retorna o ID do participante criado
        });
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return BadRequest("Usuário não encontrado.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return Ok(new { message = "E-mail confirmado com sucesso!" });
        else
            return BadRequest("Falha na confirmação do e-mail.");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            if (!user.EmailConfirmed)
                return Unauthorized(new { message = "E-mail não confirmado. Verifique sua caixa de entrada para confirmar seu e-mail." });

            // Verifica se o usuário é um administrador
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var participante = await _context.Participantes
                .FirstOrDefaultAsync(p => p.UsuarioId == user.Id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                participanteId = participante?.Id,
                roles = userRoles,
                isAdmin // Retorna a informação se o usuário é um administrador
            });
        }

        return Unauthorized(new { message = "Credenciais inválidas." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            return BadRequest("Usuário não encontrado ou e-mail não confirmado.");

        // Gerar um código de redefinição
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        // Enviar e-mail com o código
        await SendPasswordResetEmail(user.Email, code);

        return Ok(new { message = "Instruções para redefinição de senha foram enviadas para seu e-mail." });
    }

    private async Task SendPasswordResetEmail(string email, string code)
    {
        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        var subject = "Código de Redefinição de Senha";
        var to = new EmailAddress(email);
        var plainTextContent = $"Seu código de redefinição de senha é: {code}";
        var htmlContent = $"<strong>Seu código de redefinição de senha é:</strong> {code}";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        try
        {
            var response = await _sendGridClient.SendEmailAsync(msg);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Erro ao enviar e-mail: {response.StatusCode} - {responseBody}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            throw new Exception("Erro ao enviar e-mail de redefinição de senha. Verifique as configurações do SendGrid e tente novamente.");
        }
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        // Validar o código
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok(new { message = "Senha redefinida com sucesso!" });
        }

        return BadRequest("Falha ao redefinir a senha.");
    }
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Remove o cookie que contém o token
        Response.Cookies.Delete("jwtToken"); // O nome do cookie deve ser o mesmo que você usou ao criá-lo

        return Ok(new { message = "Logout bem-sucedido!" });
    }


    private async Task SendConfirmationEmail(string email, string confirmationLink)
    {
        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        var subject = "Confirmação de E-mail";
        var to = new EmailAddress(email);
        var plainTextContent = $"Por favor, confirme sua conta clicando no link: {confirmationLink}";
        var htmlContent = $"<strong>Por favor, confirme sua conta clicando no link:</strong> <a href='{confirmationLink}'>Confirmar E-mail</a>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        try
        {
            var response = await _sendGridClient.SendEmailAsync(msg);

            // Verificar a resposta do SendGrid para mensagens de erro mais detalhadas
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Erro ao enviar e-mail: {response.StatusCode} - {responseBody}");
            }
        }
        catch (Exception ex)
        {
            // Exibir detalhes do erro no console para depuração
            Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            throw new Exception("Erro ao enviar e-mail de confirmação. Verifique as configurações do SendGrid e tente novamente.");
        }
    }

    private string GenerateJwtToken(Usuario user)
    {
        var claims = new List<Claim> 
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

        var userRoles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpDelete("delete-user/{email}")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound(new { message = "Usuário não encontrado." });
        }

        
        var participante = await _context.Participantes.FirstOrDefaultAsync(p => p.UsuarioId == user.Id);
        if (participante != null)
        {
            _context.Participantes.Remove(participante);
            await _context.SaveChangesAsync();
        }

        
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Erro ao excluir o usuário." });
        }

        return Ok(new { message = "Usuário e dados relacionados excluídos com sucesso." });
    }

}