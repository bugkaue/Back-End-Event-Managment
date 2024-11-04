using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IAtividadeService
{
    Task AdicionarAtividadeAsync(AtividadeRecente atividade);
    Task<IEnumerable<AtividadeRecente>> GetAtividadesRecentesAsync();
}
