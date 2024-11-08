using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[Route("api/email")]
[ApiController]
public class EmailController(
    IGenerateJwtToken generateJwtTokenService,
    IEmailService mailService) : ControllerBase
{
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