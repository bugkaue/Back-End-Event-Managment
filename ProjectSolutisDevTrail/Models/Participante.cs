using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Participante
{
    [Key]
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public string? Email { get; set; }
    public string? UsuarioId { get; set; } 
    public Usuario? Usuario { get; set; } 

    public ICollection<Inscricao>? Inscricoes { get; set; }
}
