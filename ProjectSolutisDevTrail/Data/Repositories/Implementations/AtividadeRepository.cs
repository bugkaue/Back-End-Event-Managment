using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Implementations;

public class AtividadeRepository : IAtividadeRepository
{
    private readonly EventoContext _context;

    public AtividadeRepository(EventoContext context)
    {
        _context = context;
    }

    public async Task AdicionarAtividadeAsync(AtividadeRecente atividade)
    {
        await _context.AtividadesRecentes.AddAsync(atividade);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AtividadeRecente>> GetAtividadesRecentesAsync()
    {
        return await _context.AtividadesRecentes.ToListAsync();
    }
}
