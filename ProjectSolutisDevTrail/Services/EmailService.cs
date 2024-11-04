using Microsoft.Extensions.Options;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProjectSolutisDevTrail.Services;
public class EmailService : IEmailService
{
    private readonly string _sendGridApiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IOptions<SendGridOptions> options)
    {
        _sendGridApiKey = options.Value.ApiKey;
        _fromEmail = options.Value.FromEmail;
        _fromName = options.Value.FromName;
    }

    public async Task SendEmailConfirmation(string email, string confirmationLink)
    {
        var client = new SendGridClient(_sendGridApiKey);
        var from = new EmailAddress(_fromEmail, _fromName);
        var subject = "Confirmação de E-mail";
        var to = new EmailAddress(email);
        var plainTextContent = $"Por favor, confirme seu e-mail clicando no link: {confirmationLink}";
        var htmlContent = $"<strong>Por favor, confirme seu e-mail clicando no link:</strong> <a href=\"{confirmationLink}\">Confirmar E-mail</a>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = await client.SendEmailAsync(msg);
    }
}
