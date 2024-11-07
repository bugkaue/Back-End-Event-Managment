using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ProjectSolutisDevTrail.Services
{
    public class AccountService : ControllerBase , IAccountService 
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;
        private readonly IAccountRepository _accountRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IConfiguration configuration,
            ISendGridClient sendGridClient,
            IAccountRepository accountRepository,
            RoleManager<IdentityRole> roleManager,
            IUrlHelperFactory urlHelperFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _sendGridClient = sendGridClient;
            _accountRepository = accountRepository;
            _roleManager = roleManager;
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new BadRequestObjectResult(new { message = "O e-mail fornecido já está registrado." });
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
                return new BadRequestObjectResult(result.Errors);

            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                if (!roleResult.Succeeded)
                    return new BadRequestObjectResult("Erro ao criar o papel 'User'.");
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addToRoleResult.Succeeded)
                return new BadRequestObjectResult(addToRoleResult.Errors);

            var participante = new Participante
            {
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Email = model.Email,
                UsuarioId = user.Id
            };

            await _accountRepository.AddParticipanteAsync(participante);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generate the URL helper instance.
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));
            var confirmationLink = urlHelper.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, _httpContextAccessor.HttpContext.Request.Scheme);

            if (confirmationLink == null)
            {
                return new BadRequestObjectResult("Erro ao gerar o link de confirmação de e-mail.");
            }
            await SendConfirmationEmail(user.Email, confirmationLink);

            return new OkObjectResult(new
            {
                message = "Registro bem-sucedido! Verifique seu e-mail para confirmar sua conta.",
                participanteId = participante.Id
            });
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new BadRequestObjectResult("Usuário não encontrado.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return new OkObjectResult(new { message = "E-mail confirmado com sucesso!" });
            else
                return new BadRequestObjectResult("Falha na confirmação do e-mail.");
        }
       
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (!user.EmailConfirmed)
                    return new UnauthorizedObjectResult(new { message = "E-mail não confirmado. Verifique sua caixa de entrada para confirmar seu e-mail." });

                bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                var participante = await _accountRepository.GetParticipanteByUsuarioIdAsync(user.Id);

                var userRoles = await _userManager.GetRolesAsync(user);

                var token = GenerateJwtToken(user);

                return new OkObjectResult(new
                {
                    token,
                    participanteId = participante?.Id,
                    roles = userRoles,
                    isAdmin
                });
            }

            return new UnauthorizedObjectResult(new { message = "Credenciais inválidas." });
        }

        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return new BadRequestObjectResult("Usuário não encontrado ou e-mail não confirmado.");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendPasswordResetEmail(user.Email, code);

            return new OkObjectResult(new { message = "Instruções para redefinição de senha foram enviadas para seu e-mail." });
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new NotFoundObjectResult("Usuário não encontrado.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { message = "Senha redefinida com sucesso!" });
            }

            return new BadRequestObjectResult("Falha ao redefinir a senha.");
        }

        public async Task<IActionResult> Logout()
        {
            if (Response != null && Response.Cookies != null)
            {
                Response.Cookies.Delete("jwtToken");
            }
            else
            {
                // Log error or handle the null case
                return new BadRequestObjectResult(new { message = "Falha ao realizar o logout." });
            }

            return new OkObjectResult(new { message = "Logout bem-sucedido!" });
        }
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "Usuário não encontrado." });
            }

            var participante = await _accountRepository.GetParticipanteByUsuarioIdAsync(user.Id);
            if (participante != null)
            {
                await _accountRepository.RemoveParticipanteAsync(participante);
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(new { message = "Erro ao excluir o usuário." });
            }

            return new OkObjectResult(new { message = "Usuário e dados relacionados excluídos com sucesso." });
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

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    throw new Exception($"Erro ao enviar e-mail: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
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
    }
}