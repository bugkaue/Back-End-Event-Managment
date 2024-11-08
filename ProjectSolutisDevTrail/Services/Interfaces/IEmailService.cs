namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmail(string email, string code);
    Task SendConfirmationEmail(string email, string confirmationLink);
    Task SendEmail(string email, string subject, string plainTextContent, string htmlContent);
}
