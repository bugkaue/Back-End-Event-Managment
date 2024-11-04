using ProjectSolutisDevTrail.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Evento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título do evento é obrigatório")]
    [StringLength(100, ErrorMessage = "O título pode ter no máximo 100 caracteres")]
    public string? Titulo { get; set; }

    [Required(ErrorMessage = "A descrição do evento é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A data e horário do evento são obrigatórios")]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "O local do evento é obrigatório")]
    [StringLength(200, ErrorMessage = "O local pode ter no máximo 200 caracteres")]
    public string? Local { get; set; }

    [Required(ErrorMessage = "A capacidade máxima é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "A capacidade máxima deve ser maior que 0")]
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