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
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // Endpoint para registro de um novo usuário
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        return await _accountService.Register(model);
    }

    // Endpoint para confirmar o e-mail do usuário
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        return await _accountService.ConfirmEmail(token, email);
    }

    // Endpoint para login do usuário
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        return await _accountService.Login(model);
    }

    // Endpoint para enviar instruções de redefinição de senha
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        return await _accountService.ForgotPassword(model);
    }

    // Endpoint para redefinir a senha
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        return await _accountService.ResetPassword(model);
    }

    // Endpoint para logout do usuário
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (Response?.Cookies != null)
        {
            Response.Cookies.Delete("jwtToken");
            return Ok(new { message = "Logout bem-sucedido!" });
        }
        return BadRequest(new { message = "Falha ao realizar o logout." });
    }

    // Endpoint para excluir um usuário
    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromQuery] string email)
    {
        return await _accountService.DeleteUser(email);
    }
}