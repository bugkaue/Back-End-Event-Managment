using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IParticipanteService
{
    Task<ReadParticipanteDto> CreateAsync(CreateParticipanteDto createParticipanteDto);
    Task<ReadParticipanteDto> GetByIdAsync(int id);
    Task<List<ReadParticipanteDto>> GetAllAsync();
    Task UpdateAsync(int id, UpdateParticipanteDto updateParticipanteDto);
    Task DeleteAsync(int id);
    Task<int> CountAsync();
}