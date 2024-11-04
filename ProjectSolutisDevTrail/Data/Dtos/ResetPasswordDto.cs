using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Data.Dtos;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }
}
