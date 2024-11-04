using AutoMapper;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Profiles;

public class ParticipanteProfile : Profile
{
    public ParticipanteProfile()
    {
        CreateMap<CreateParticipanteDto, Participante>();
        CreateMap<UpdateParticipanteDto, Participante>();
        CreateMap<Participante, ReadParticipanteDto>();
    }
}
