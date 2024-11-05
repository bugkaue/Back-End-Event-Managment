using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Services;
public class InscricaoService : IInscricaoService
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IInscricaoRepository _inscricaoRepository;
    private readonly ReportService _reportService;

    public InscricaoService(IEventoRepository eventoRepository, IInscricaoRepository inscricaoRepository, ReportService reportService)
    {
        _eventoRepository = eventoRepository;
        _inscricaoRepository = inscricaoRepository;
        _reportService = reportService;
    }

    // Implementation of GetAllAsync from IGenericRepository<Inscricao>
    public async Task<IEnumerable<Inscricao>> GetAllAsync()
    {
        return await _inscricaoRepository.GetAllAsync(); // Delegate to the repository
    }
    public Task<int> GetInscricoesCountByEventoIdAsync(int eventoId)
    {
        return _inscricaoRepository.GetInscricoesCountByEventoIdAsync(eventoId);
    }

    // Implementation of GetByIdAsync from IGenericRepository<Inscricao>
    public async Task<Inscricao> GetByIdAsync(int id)
    {
        return await _inscricaoRepository.GetByIdAsync(id); // Delegate to the repository
    }

    public async Task<Inscricao> GetInscricaoByIdAsync(int id)
    {
        return await GetByIdAsync(id); // Reuse the implemented method
    }

    public async Task<IEnumerable<Evento>> GetEventosByParticipanteIdAsync(int participanteId)
    {
        return await _inscricaoRepository.GetEventosByParticipanteIdAsync(participanteId);
    }

    public async Task<int> GetInscricoesCountAsync()
    {
        return await _inscricaoRepository.GetInscricoesCountAsync();
    }

    public async Task<Inscricao> AddAsync(Inscricao inscricao)
    {
        return await _inscricaoRepository.AddAsync(inscricao);
    }

    public async Task DeleteAsync(int id)
    {
        var inscricao = await GetByIdAsync(id); // Use the implemented method
        if (inscricao != null)
        {
            await _inscricaoRepository.DeleteAsync(id); // Pass the ID
        }
    }

    public async Task UpdateAsync(Inscricao inscricao)
    {
        await _inscricaoRepository.UpdateAsync(inscricao);
    }

    public async Task<Inscricao> GetInscricaoByParticipanteAndEventoIdAsync(int participanteId, int eventoId)
    {
        return await _inscricaoRepository.GetInscricaoByParticipanteAndEventoIdAsync(participanteId, eventoId);
    }

    public async Task<IEnumerable<Participante>> GetParticipantesByEventoIdAsync(int eventoId)
    {
        return await _inscricaoRepository.GetParticipantesPorEventoId(eventoId);
    }

    public async Task<IEnumerable<Participante>> GetParticipantesPorEventoId(int eventoId)
    {
        return await _inscricaoRepository.GetParticipantesPorEventoId(eventoId);
    }
    public async Task<bool> DeleteInscricaoByParticipanteAndEventoAsync(int participanteId, int eventoId)
    {
        var inscricao = await _inscricaoRepository.GetInscricaoByParticipanteAndEventoIdAsync(participanteId, eventoId);
        if (inscricao != null)
        {
            await _inscricaoRepository.DeleteAsync(inscricao.Id); // Exclua a inscrição
            return true; // Retorne true se a exclusão foi bem-sucedida
        }
        return false; // Retorne false se a inscrição não foi encontrada
    }
        public async Task<IEnumerable<InscricaoCountDto>> GetInscricoesCountsByEventoIdsAsync(List<int> eventoIds)
        {
            return await _eventoRepository.GetInscricoesCountsByEventoIds(eventoIds);
        }
    public async Task GenerateReportAsync(int eventoId, Stream outputStream)
    {
        var evento = await _eventoRepository.GetEventoByIdAsync(eventoId);
        var participantes = await _inscricaoRepository.GetParticipantesPorEventoId(eventoId);

        using (var pdfWriter = new PdfWriter(outputStream))
        {
            using (var pdfDocument = new PdfDocument(pdfWriter))
            {
                var document = new Document(pdfDocument);
                document.Add(new Paragraph($"Relatório para o Evento: {evento.Titulo}").SetFontSize(20));

                foreach (var participante in participantes)
                {
                    document.Add(new Paragraph($"Participante: {participante.Nome} {participante.Sobrenome}"));
                }
                document.Close();
            }
        }
    }
    public async Task GenerateReportAsync(int eventoId, string outputStream)
    {
        var evento = await _eventoRepository.GetEventoByIdAsync(eventoId);
        var participantes = await _inscricaoRepository.GetParticipantesPorEventoId(eventoId);

        using (var pdfWriter = new PdfWriter(outputStream))
        {
            using (var pdfDocument = new PdfDocument(pdfWriter))
            {
                var document = new Document(pdfDocument);
                document.Add(new Paragraph($"Relatório para o Evento: {evento.Titulo}").SetFontSize(20));

                foreach (var participante in participantes)
                {
                    document.Add(new Paragraph($"Participante: {participante.Nome} {participante.Sobrenome}"));
                }
                document.Close();
            }
        }
    }
    public async Task<IEnumerable<Inscricao>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _inscricaoRepository.GetByIdsAsync(ids);
    }
}


