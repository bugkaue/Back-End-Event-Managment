using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Login
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}