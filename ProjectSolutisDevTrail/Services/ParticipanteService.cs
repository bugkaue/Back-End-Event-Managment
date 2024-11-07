using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Services.Interfaces;
using ProjectSolutisDevTrail.Data.Dtos;
using AutoMapper;

namespace ProjectSolutisDevTrail.Services;

public class ParticipanteService : IParticipanteService
{
    private readonly IParticipanteRepository _repository;
    private readonly IMapper _mapper;

    public ParticipanteService(IParticipanteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ReadParticipanteDto> CreateAsync(CreateParticipanteDto createParticipanteDto)
    {
        var participante = _mapper.Map<Participante>(createParticipanteDto);
        await _repository.AddAsync(participante);
        return _mapper.Map<ReadParticipanteDto>(participante);
    }

    public async Task<ReadParticipanteDto> GetByIdAsync(int id)
    {
        var participante = await _repository.GetByIdAsync(id);
        return participante == null ? null : _mapper.Map<ReadParticipanteDto>(participante);
    }

    public async Task<List<ReadParticipanteDto>> GetAllAsync()
    {
        var participantes = await _repository.GetAllAsync();
        return _mapper.Map<List<ReadParticipanteDto>>(participantes);
    }

    public async Task UpdateAsync(int id, UpdateParticipanteDto updateParticipanteDto)
    {
        var participante = await _repository.GetByIdAsync(id);
        if (participante != null)
        {
            _mapper.Map(updateParticipanteDto, participante);
            await _repository.UpdateAsync(participante);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var participante = await _repository.GetByIdAsync(id);
        if (participante != null)
        {
            await _repository.DeleteAsync(participante);
        }
    }

    public async Task<int> CountAsync()
    {
        return await _repository.CountAsync();
    }
}