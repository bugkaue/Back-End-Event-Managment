using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Usuario : IdentityUser
{
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public virtual ICollection<Participante>? Participantes { get; set; }
    public ICollection<Evento>? Eventos { get; set; } 
}
