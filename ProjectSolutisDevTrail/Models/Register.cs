using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Register
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
}
