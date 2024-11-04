using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Data.Dtos;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail deve ser válido.")]
    public string Email { get; set; }
}
