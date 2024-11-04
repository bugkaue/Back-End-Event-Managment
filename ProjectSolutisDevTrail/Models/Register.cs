using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Register
{
    [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
    [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
    public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    public string? Sobrenome { get; set; }
}
