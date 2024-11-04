using AutoMapper;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Profiles;

public class EventoProfile : Profile
{
    public EventoProfile()
    {
        CreateMap<CreateEventoDto, Evento>();
        CreateMap<Evento, CreateEventoDto>();
        CreateMap<UpdateEventoDto, Evento>();
        CreateMap<Evento, ReadEventoDto>();
        CreateMap<Evento, EventoSimplificadoDto>();
    }
}