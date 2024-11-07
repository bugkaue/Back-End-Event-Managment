using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces;

public interface IParticipanteRepository
{
    Task<Participante> AddAsync(Participante participante);
    Task<Participante> GetByIdAsync(int id);
    Task<List<Participante>> GetAllAsync();
    Task UpdateAsync(Participante participante);
    Task DeleteAsync(Participante participante);
    Task<int> CountAsync();
}