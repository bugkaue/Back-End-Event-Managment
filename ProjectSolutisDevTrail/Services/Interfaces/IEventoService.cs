using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IEventoService : IGenericRepository<Evento>
{
    Task<int> GetEventosCountAsync();
    Task<int> GetEventosCountAsync(bool? isFinalizado); 
    Task<List<EventoSimplificadoDto>> GetInscricoesPorParticipanteId(int participanteId);
    Task<List<InscricaoCountDto>> GetInscricoesCountsByEventoIds(List<int> eventoIds);
}