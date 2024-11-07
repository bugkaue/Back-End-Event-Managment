using Microsoft.AspNetCore.Identity;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<Usuario> FindByEmailAsync(string email);
    Task<IdentityResult> CreateAsync(Usuario user, string password);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IdentityResult> CreateRoleAsync(IdentityRole role);
    Task<IdentityResult> AddToRoleAsync(Usuario user, string role);
    public Task<Participante> AddParticipanteAsync(Participante participante);
    Task SaveChangesAsync();
    Task<string> GenerateEmailConfirmationTokenAsync(Usuario user);
    Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token);
    Task<bool> CheckPasswordAsync(Usuario user, string password);
    Task<bool> IsInRoleAsync(Usuario user, string role);
    Task<List<string>> GetRolesAsync(Usuario user);
    Task<string> GeneratePasswordResetTokenAsync(Usuario user);
    Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword);
    Task<IdentityResult> DeleteAsync(Usuario user);
    Task RemoveParticipanteAsync(Participante participante); // Add this method
    Task<Participante> GetParticipanteByUsuarioIdAsync(string usuarioId); // Add this method

}
