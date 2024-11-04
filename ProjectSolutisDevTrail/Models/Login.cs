using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Login
{
    [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string? Password { get; set; }
}