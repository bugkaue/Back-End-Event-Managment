using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Services;
public class InscricaoService(IEventoRepository eventoRepository, IInscricaoRepository inscricaoRepository, ReportService reportService) : IInscricaoService
{

    public async Task<IEnumerable<Inscricao>> GetAllAsync()
    {
        return await inscricaoRepository.GetAllAsync(); 
    }

    public Task<int> GetInscricoesCountByEventoIdAsync(int eventoId)
    {
        return inscricaoRepository.GetInscricoesCountByEventoIdAsync(eventoId);
    }

    public async Task<Inscricao> GetByIdAsync(int id)
    {
        return await inscricaoRepository.GetByIdAsync(id); 
    }

    public async Task<Inscricao> GetInscricaoByIdAsync(int id)
    {
        return await GetByIdAsync(id); 
    }

    public async Task<IEnumerable<ReadEventoDto>> GetEventosComInscricoesByParticipanteIdAsync(int participanteId)
    {
        var eventos = await inscricaoRepository.GetEventosByParticipanteIdAsync(participanteId);
        if (eventos == null || !eventos.Any())
        {
            return Enumerable.Empty<ReadEventoDto>();
        }

        var eventoIds = eventos.Select(e => e.Id).ToList();
        var inscricoesCounts = await eventoRepository.GetInscricoesCountsByEventoIds(eventoIds);

        return eventos.Select(evento =>
        {
            var dto = new ReadEventoDto
            {
                Id = evento.Id,
                Titulo = evento.Titulo,
                Descricao = evento.Descricao,
                DataHora = evento.DataHora,
                Local = evento.Local,
                CapacidadeMaxima = evento.CapacidadeMaxima,
                IsAtivo = evento.IsAtivo,
                NumeroInscricoes = inscricoesCounts.FirstOrDefault(ic => ic.EventoId == evento.Id)?.NumeroInscricoes ?? 0
            };
            return dto;
        }).ToList();
    }

    public async Task<int> GetInscricoesCountAsync()
    {
        return await inscricaoRepository.GetInscricoesCountAsync();
    }

    public async Task<Inscricao> AddAsync(Inscricao inscricao)
    {
        return await inscricaoRepository.AddAsync(inscricao);
    }

    public async Task DeleteAsync(int id)
    {
        var inscricao = await GetByIdAsync(id); 
        if (inscricao != null)
        {
            await inscricaoRepository.DeleteAsync(id); 
        }
    }

    public async Task UpdateAsync(Inscricao inscricao)
    {
        await inscricaoRepository.UpdateAsync(inscricao);
    }

    public async Task<Inscricao> GetInscricaoByParticipanteAndEventoIdAsync(int participanteId, int eventoId)
    {
        return await inscricaoRepository.GetInscricaoByParticipanteAndEventoIdAsync(participanteId, eventoId);
    }

    public async Task<IEnumerable<Participante>> GetParticipantesByEventoIdAsync(int eventoId)
    {
        return await inscricaoRepository.GetParticipantesPorEventoId(eventoId);
    }

    public async Task<IEnumerable<Participante>> GetParticipantesPorEventoId(int eventoId)
    {
        return await inscricaoRepository.GetParticipantesPorEventoId(eventoId);
    }

    public async Task<bool> DeleteInscricaoByParticipanteAndEventoAsync(int participanteId, int eventoId)
    {
        var inscricao = await inscricaoRepository.GetInscricaoByParticipanteAndEventoIdAsync(participanteId, eventoId);
        if (inscricao != null)
        {
            await inscricaoRepository.DeleteAsync(inscricao.Id);
            return true; 
        }
        return false;
    }

    public async Task<IEnumerable<InscricaoCountDto>> GetInscricoesCountsByEventoIdsAsync(List<int> eventoIds)
    {
        return await eventoRepository.GetInscricoesCountsByEventoIds(eventoIds);
    }

    public async Task GenerateReportAsync(int eventoId, Stream outputStream)
    {
        var evento = await eventoRepository.GetEventoByIdAsync(eventoId);
        var participantes = await inscricaoRepository.GetParticipantesPorEventoId(eventoId);

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
        var evento = await eventoRepository.GetEventoByIdAsync(eventoId);
        var participantes = await inscricaoRepository.GetParticipantesPorEventoId(eventoId);

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
        return await inscricaoRepository.GetByIdsAsync(ids);
    }

    public async Task<IEnumerable<EventoSimplificadoDto>> GetEventosSimplificadosByParticipanteIdAsync(int participanteId)
    {
        var eventos = await inscricaoRepository.GetEventosByParticipanteIdAsync(participanteId);
        var eventoIds = eventos.Select(e => e.Id).ToList();
        var inscricoesCounts = await inscricaoRepository.GetInscricoesCountsByEventoIdsAsync(eventoIds);

        return eventos.Select(evento => new EventoSimplificadoDto
        {
            EventoId = evento.Id,
            Titulo = evento.Titulo,
            Descricao = evento.Descricao,
            DataHora = evento.DataHora,
            Local = evento.Local,
            CapacidadeMaxima = evento.CapacidadeMaxima,
            IsAtivo = evento.IsAtivo,
            NumeroInscricoes = inscricoesCounts.FirstOrDefault(ic => ic.EventoId == evento.Id)?.NumeroInscricoes ?? 0
        }).ToList();
    }

    public async Task<IEnumerable<Evento>> GetEventosByParticipanteIdAsync(int participanteId)
    {
        return await inscricaoRepository.GetEventosByParticipanteIdAsync(participanteId);
    }
}
