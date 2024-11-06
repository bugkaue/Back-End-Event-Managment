using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Services.Interfaces;

public interface IInscricaoService : IGenericRepository<Inscricao>, IInscricaoRepository
{
    Task<IEnumerable<ReadEventoDto>> GetEventosComInscricoesByParticipanteIdAsync(int participanteId);
    Task<IEnumerable<Evento>> GetEventosByParticipanteIdAsync(int participanteId); 
    Task<Inscricao> GetInscricaoByParticipanteAndEventoIdAsync(int participanteId, int eventoId);
    Task<IEnumerable<InscricaoCountDto>> GetInscricoesCountsByEventoIdsAsync(List<int> eventoIds);
    Task<IEnumerable<Participante>> GetParticipantesByEventoIdAsync(int eventoId); 
    Task<int> GetInscricoesCountAsync();
    Task GenerateReportAsync(int eventoId, Stream outputStream);    
}