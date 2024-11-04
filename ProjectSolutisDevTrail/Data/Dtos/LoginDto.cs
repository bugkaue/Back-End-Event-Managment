using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Data.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "O campo de e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; }
}
