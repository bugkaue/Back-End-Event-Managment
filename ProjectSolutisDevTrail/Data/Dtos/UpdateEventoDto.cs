namespace ProjectSolutisDevTrail.Data.Dtos;
public class UpdateEventoDto
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public DateTime? DataHora { get; set; }
    public string? Local { get; set; }
    public int? CapacidadeMaxima { get; set; }
}
