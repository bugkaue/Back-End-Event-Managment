using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Services;
public class EventoService : IEventoService, IGenericRepository<Evento>
{
    private readonly IGenericRepository<Evento> _genericRepository;

    public EventoService(IGenericRepository<Evento> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<IEnumerable<Evento>> GetAllAsync()
    {
        return await _genericRepository.GetAllAsync();
    }

    public async Task<Evento> GetByIdAsync(int id)
    {
        return await _genericRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(Evento entity)
    {
        await _genericRepository.AddAsync(entity);
    }

    public async Task UpdateAsync(Evento entity)
    {
        await _genericRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _genericRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Evento>> GetAllEventosAsync()
    {
        return await _genericRepository.GetAllAsync();
    }

    public async Task<Evento> GetEventoByIdAsync(int id)
    {
        return await _genericRepository.GetByIdAsync(id);
    }

    public async Task AddEventoAsync(Evento evento)
    {
        await _genericRepository.AddAsync(evento);
    }

    public async Task UpdateEventoAsync(Evento evento)
    {
        await _genericRepository.UpdateAsync(evento);
    }

    public async Task DeleteEventoAsync(int id)
    {
        await _genericRepository.DeleteAsync(id);
    }

    public async Task<int> GetEventosCountAsync()
    {
        var eventos = await _genericRepository.GetAllAsync();
        return eventos.Count();
    }

    public async Task<int> GetEventosCountAsync(bool? isFinalizado)
    {
        var eventos = await _genericRepository.GetAllAsync();
        return eventos.Count(e => e.IsFinalizado == isFinalizado);
    }
}

