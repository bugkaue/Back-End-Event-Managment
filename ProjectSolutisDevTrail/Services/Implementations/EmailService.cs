using ProjectSolutisDevTrail.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace ProjectSolutisDevTrail.Services.Implementations;
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ISendGridClient _sendGridClient;

    public EmailService(IConfiguration configuration, ISendGridClient sendGridClient)
    {
        _configuration = configuration;
        _sendGridClient = sendGridClient;
    }

    public async Task SendPasswordResetEmail(string email, string code)
    {
        var subject = "Código de Redefinição de Senha";
        var plainTextContent = $"Seu código de redefinição de senha é: {code}";
        var htmlContent = $"<strong>Seu código de redefinição de senha é:</strong> {code}";
        await SendEmail(email, subject, plainTextContent, htmlContent);
    }

    public async Task SendConfirmationEmail(string email, string confirmationLink)
    {
        var subject = "Confirmação de E-mail";
        var plainTextContent = $"Por favor, confirme sua conta clicando no link: {confirmationLink}";
        var htmlContent = $"<strong>Por favor, confirme sua conta clicando no link:</strong> <a href='{confirmationLink}'>Confirmar E-mail</a>";
        await SendEmail(email, subject, plainTextContent, htmlContent);
    }

    public async Task SendEmail(string email, string subject, string plainTextContent, string htmlContent)
    {
        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        var to = new EmailAddress(email);
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
            throw new Exception("Erro ao enviar e-mail. Verifique as configurações do SendGrid e tente novamente.");
        }
    }
}
