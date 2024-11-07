using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EventoContext _context;

        public AccountRepository(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, EventoContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<Usuario> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateAsync(Usuario user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<IdentityResult> CreateRoleAsync(IdentityRole role)
        {
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> AddToRoleAsync(Usuario user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<Participante> AddParticipanteAsync(Participante participante)
        {
            _context.Participantes.Add(participante);
            await _context.SaveChangesAsync();
            return participante;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Usuario user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<bool> CheckPasswordAsync(Usuario user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> IsInRoleAsync(Usuario user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<List<string>> GetRolesAsync(Usuario user)
        {
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> DeleteAsync(Usuario user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task RemoveParticipanteAsync(Participante participante)
        {
            _context.Participantes.Remove(participante);
            await _context.SaveChangesAsync();
        }

        public async Task<Participante> GetParticipanteByUsuarioIdAsync(string usuarioId)
        {
            return await _context.Participantes.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }
    }
}