using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IEventoService : IGenericRepository<Evento>
{
    Task<IEnumerable<Evento>> GetAllEventosAsync();
    Task<Evento> GetEventoByIdAsync(int id);
    Task AddEventoAsync(Evento evento);
    Task UpdateEventoAsync(Evento evento);
    Task DeleteEventoAsync(int id);
    Task<int> GetEventosCountAsync();
    Task<int> GetEventosCountAsync(bool? isFinalizado); 
}