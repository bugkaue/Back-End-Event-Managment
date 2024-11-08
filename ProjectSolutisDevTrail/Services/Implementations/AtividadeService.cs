using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Services.Implementations;

public class AtividadeService : IAtividadeService
{
    private readonly IAtividadeRepository _atividadeRepository;

    public AtividadeService(IAtividadeRepository atividadeRepository)
    {
        _atividadeRepository = atividadeRepository;
    }

    public async Task AdicionarAtividadeAsync(AtividadeRecente atividade)
    {
        await _atividadeRepository.AdicionarAtividadeAsync(atividade);
    }

    public async Task<IEnumerable<AtividadeRecente>> GetAtividadesRecentesAsync()
    {
        return await _atividadeRepository.GetAtividadesRecentesAsync(); // Você precisa implementar este método no repositório
    }
}
