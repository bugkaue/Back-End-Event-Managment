using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces;

public interface IParticipanteRepository : IGenericRepository<Participante>
{
    Task DeleteAsync(Participante participante);
    Task<int> CountAsync();
}