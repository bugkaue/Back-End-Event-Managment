using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces;

public interface IAtividadeRepository
{
    Task AdicionarAtividadeAsync(AtividadeRecente atividade);
    Task<IEnumerable<AtividadeRecente>> GetAtividadesRecentesAsync();
}
