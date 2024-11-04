namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces;

using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
public interface IEventoRepository : IGenericRepository<Evento>
{
    Task<Evento> GetEventoByIdAsync(int id);
    Task<List<EventoSimplificadoDto>> GetInscricoesPorParticipanteId(int participanteId);
    Task<int> GetEventosCountAsync(bool? isFinalizado);
    Task<int> GetEventosCountAsync();
   Task<List<InscricaoCountDto>> GetInscricoesCountsByEventoIds(List<int> eventoIds);
}
