using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSolutisDevTrail.Models;

public class Inscricao
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Evento")]
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    [ForeignKey("Participante")]
    public int ParticipanteId { get; set; }
    public Participante? Participante { get; set; }
    public DateTime DataInscricao { get; set; }
}