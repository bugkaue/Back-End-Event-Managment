namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmation(string email, string confirmationLink);
}
