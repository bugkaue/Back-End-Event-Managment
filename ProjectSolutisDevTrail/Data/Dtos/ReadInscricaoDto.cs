namespace ProjectSolutisDevTrail.Data.Dtos;

public class ReadInscricaoDto
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public int ParticipanteId { get; set; }
    public DateTime DataInscricao { get; set; }
}
