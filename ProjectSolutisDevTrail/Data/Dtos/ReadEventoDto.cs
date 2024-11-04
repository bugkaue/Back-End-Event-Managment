namespace ProjectSolutisDevTrail.Data.Dtos;

public class ReadEventoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataHora { get; set; }
    public string Local { get; set; }
    public int CapacidadeMaxima { get; set; }
    
    // Novas propriedades
    public bool IsAtivo { get; set; } // Para indicar se o evento está ativo
    public int NumeroInscricoes { get; set; } // Para armazenar o número de inscrições
}