using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IActionResult> Register(RegisterDto model);
        Task<IActionResult> ConfirmEmail(string token, string email);
        Task<IActionResult> Login(LoginDto model);
        Task<IActionResult> ForgotPassword(ForgotPasswordDto model);
        Task<IActionResult> ResetPassword(ResetPasswordDto model);
        Task<IActionResult> Logout();
        Task<IActionResult> DeleteUser(string email);
    }
}
