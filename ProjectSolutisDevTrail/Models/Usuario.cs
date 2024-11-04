using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Usuario : IdentityUser
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    public string? Sobrenome { get; set; }

    // Relacionamento 1:N com Evento
     public virtual ICollection<Participante>? Participantes { get; set; }
    public ICollection<Evento>? Eventos { get; set; } // Um usuário pode ter vários eventos
}

