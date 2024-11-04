using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Inscricao
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O evento é obrigatório")]
    [ForeignKey("Evento")]
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }

    [Required(ErrorMessage = "O participante é obrigatório")]
    [ForeignKey("Participante")]
    public int ParticipanteId { get; set; }
    public Participante? Participante { get; set; }

    [Required(ErrorMessage = "A data de inscrição é obrigatória")]
    public DateTime DataInscricao { get; set; }
}