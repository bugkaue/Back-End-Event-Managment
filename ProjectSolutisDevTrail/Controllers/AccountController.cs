using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(
    IAccountService accountService,
    IGenerateJwtToken generateJwtTokenService,
    IEmailService mailService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        return await accountService.Register(model);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        return await accountService.ConfirmEmail(token, email);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        return await accountService.Login(model);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        return await accountService.ForgotPassword(model);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        return await accountService.ResetPassword(model);
    }


    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
    public IActionResult Logout()
    {
        if (Response?.Cookies != null)
        {
            Response.Cookies.Delete("jwtToken");
            return Ok(new { message = "Logout bem-sucedido!" });
        }
        return BadRequest(new { message = "Falha ao realizar o logout." });
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromQuery] string email)
    {
        return await accountService.DeleteUser(email);
    }
}