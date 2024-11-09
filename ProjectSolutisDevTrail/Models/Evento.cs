using ProjectSolutisDevTrail.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Evento
{
    [Key]
    public int Id { get; set; }

    public string? Titulo { get; set; }

    public string? Descricao { get; set; }

    public DateTime DataHora { get; set; }

    public string? Local { get; set; }

    public int CapacidadeMaxima { get; set; }

    public ICollection<Inscricao>? Inscricoes { get; set; }

    public string? UsuarioId { get; set; }  
    public Usuario? Usuario { get; set; }  

    [NotMapped] // Não persiste no banco de dados
    public bool IsAtivo => DataHora > DateTime.Now; // Propriedade calculada

    [NotMapped] // Não persiste no banco de dados
    public bool IsFinalizado => DataHora <= DateTime.Now; // Propriedade que verifica se o evento está finalizado

    [NotMapped] // Não persiste no banco de dados
    public int NumeroInscricoes => Inscricoes?.Count ?? 0; // Propriedade calculada
}