namespace ProjectSolutisDevTrail.Data.Dtos;

public class EventoSimplificadoDto
{
    public int EventoId { get; set; }  
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataHora { get; set; }
    public string? Local { get; set; }
    public int CapacidadeMaxima { get; set; }
    public bool IsAtivo { get; set; } // Add this property
    public int NumeroInscricoes { get; set; } // Adicione esta propriedade
}
