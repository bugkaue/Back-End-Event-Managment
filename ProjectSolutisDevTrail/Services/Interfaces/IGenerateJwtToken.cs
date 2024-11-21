using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces
{
    public interface IGenerateJwtToken
    {
        string GenerateToken(Usuario user, string role);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<string> GenerateConfirmationLink(string email);
    }
}
