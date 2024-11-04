namespace ProjectSolutisDevTrail.Models;

public class AtividadeRecente
{
    public int Id { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataHora { get; set; }
    public string? UsuarioId { get; set; } 
}
