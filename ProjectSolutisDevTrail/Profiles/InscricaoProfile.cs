using AutoMapper;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Profiles;

public class InscricaoProfile : Profile
{

    public InscricaoProfile()
    {
        CreateMap<CreateInscricaoDto, Inscricao>();
        CreateMap<Inscricao, ReadInscricaoDto>();
        CreateMap<Inscricao, InscricaoCountDto>()
            .ForMember(dest => dest.NumeroInscricoes, opt => opt.MapFrom(src => 1)); // 1 por inscrição
    }
}