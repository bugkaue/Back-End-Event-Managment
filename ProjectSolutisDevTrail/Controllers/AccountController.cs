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

    [HttpPost("send-confirmation-email")]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] string email)
    {
        var confirmationLink = await generateJwtTokenService.GenerateConfirmationLink(email);

        await mailService.SendConfirmationEmail(email, confirmationLink);

        return Ok(new { message = "E-mail de confirmação enviado!" });
    }

    [HttpPost("send-password-reset-email")]
    public async Task<IActionResult> SendPasswordResetEmail([FromBody] string email)
    {
        var resetToken = await generateJwtTokenService.GeneratePasswordResetTokenAsync(email);

        await mailService.SendPasswordResetEmail(email, resetToken);

        return Ok(new { message = "E-mail de redefinição de senha enviado!" });
    }
}