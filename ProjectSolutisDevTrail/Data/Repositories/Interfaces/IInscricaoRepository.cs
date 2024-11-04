using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Interfaces;

public interface IInscricaoRepository : IGenericRepository<Inscricao>
{
    Task<IEnumerable<Evento>> GetEventosByParticipanteIdAsync(int participanteId);
    Task<int> GetInscricoesCountAsync();
    Task<Inscricao> GetInscricaoByParticipanteAndEventoIdAsync(int participanteId, int eventoId);
    Task<IEnumerable<Participante>> GetParticipantesPorEventoId(int eventoId);
    Task GenerateReportAsync(int eventoId, string outputStream);
    Task<bool> DeleteInscricaoByParticipanteAndEventoAsync(int participanteId, int eventoId);
    Task<int> GetInscricoesCountByEventoIdAsync(int eventoId); 
        Task<IEnumerable<InscricaoCountDto>> GetInscricoesCountsByEventoIdsAsync(List<int> eventoIds); // Adicione este método

}