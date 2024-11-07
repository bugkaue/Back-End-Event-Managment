using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Services.Interfaces;
using ProjectSolutisDevTrail.Data.Dtos;
using AutoMapper;

namespace ProjectSolutisDevTrail.Services;

public class ParticipanteService(IParticipanteRepository repository, IMapper mapper) : IParticipanteService
{
    public async Task<ReadParticipanteDto> CreateAsync(CreateParticipanteDto createParticipanteDto)
    {
        var participante = mapper.Map<Participante>(createParticipanteDto);
        await repository.AddAsync(participante);
        return mapper.Map<ReadParticipanteDto>(participante);
    }

    public async Task<ReadParticipanteDto> GetByIdAsync(int id)
    {
        var participante = await repository.GetByIdAsync(id);
        return participante == null ? null : mapper.Map<ReadParticipanteDto>(participante);
    }

    public async Task<List<ReadParticipanteDto>> GetAllAsync()
    {
        var participantes = await repository.GetAllAsync();
        return mapper.Map<List<ReadParticipanteDto>>(participantes);
    }

    public async Task UpdateAsync(int id, UpdateParticipanteDto updateParticipanteDto)
    {
        var participante = await repository.GetByIdAsync(id);
        if (participante != null)
        {
            mapper.Map(updateParticipanteDto, participante);
            await repository.UpdateAsync(participante);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var participante = await repository.GetByIdAsync(id);
        if (participante != null)
        {
            await repository.DeleteAsync(participante);
        }
    }

    public async Task<int> CountAsync()
    {
        return await repository.CountAsync();
    }
}