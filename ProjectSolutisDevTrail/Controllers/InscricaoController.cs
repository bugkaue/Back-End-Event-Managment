using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[ApiController]
[Route("[controller]")]
public class InscricaoController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IEventoService _eventoService;
    private readonly IInscricaoService _inscricaoService; // Service for Inscricao
    private readonly ReportService _reportService;
    private readonly IInscricaoRepository _inscricaoRepository; // Add this line

    public InscricaoController(IMapper mapper, IEventoService eventoService, ReportService reportService, IInscricaoService inscricaoService, IInscricaoRepository inscricaoRepository) // Add IInscricaoRepository parameter
    {
        _mapper = mapper;
        _eventoService = eventoService;
        _reportService = reportService;
        _inscricaoService = inscricaoService; // Inject InscricaoService
        _inscricaoRepository = inscricaoRepository; // Initialize _inscricaoRepository
    }
    [HttpPost]
    public async Task<ActionResult<ReadInscricaoDto>> CreateInscricao(CreateInscricaoDto createInscricaoDto)
    {
        // Verifica se o participante já está inscrito no evento
        var existingInscricao = await _inscricaoService.GetInscricaoByParticipanteAndEventoIdAsync(createInscricaoDto.ParticipanteId, createInscricaoDto.EventoId);
        if (existingInscricao != null)
        {
            return BadRequest("O participante já está inscrito neste evento.");
        }

        // Verifica se o evento está finalizado
        var evento = await _eventoService.GetEventoByIdAsync(createInscricaoDto.EventoId);
        if (evento == null)
        {
            return NotFound("Evento não encontrado.");
        }
        if (evento.IsFinalizado)
        {
            return BadRequest("Não é possível se inscrever em um evento que já foi finalizado.");
        }

        var inscricao = _mapper.Map<Inscricao>(createInscricaoDto);
        inscricao.DataInscricao = DateTime.Now;

        await _inscricaoService.AddAsync(inscricao); // Use service to add inscricao

        return CreatedAtAction(nameof(GetInscricaoById), new { id = inscricao.Id }, _mapper.Map<ReadInscricaoDto>(inscricao));
    }

       [HttpGet("{participanteId}/eventos")]
    public async Task<IEnumerable<EventoSimplificadoDto>> GetEventosByParticipanteIdAsync(int participanteId)
    {
        var eventos = await _inscricaoRepository.GetEventosByParticipanteIdAsync(participanteId);

        // Retrieve all the counts in a single query if possible
        var eventoIds = eventos.Select(e => e.Id).ToList();
        var inscricoesCounts = await _inscricaoRepository.GetInscricoesCountsByEventoIdsAsync(eventoIds); // Ensure this matches

        var eventoDtos = eventos.Select(evento => new EventoSimplificadoDto
        {
            EventoId = evento.Id,
            Titulo = evento.Titulo,
            Descricao = evento.Descricao,
            DataHora = evento.DataHora,
            Local = evento.Local,
            CapacidadeMaxima = evento.CapacidadeMaxima,
            IsAtivo = evento.IsAtivo,
            NumeroInscricoes = inscricoesCounts.FirstOrDefault(ic => ic.EventoId == evento.Id)?.NumeroInscricoes ?? 0
        });

        return eventoDtos.ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadInscricaoDto>> GetInscricaoById(int id)
    {
        var inscricao = await _inscricaoService.GetByIdAsync(id); // Use service to get inscricao
        if (inscricao == null)
        {
            return NotFound();
        }
        return _mapper.Map<ReadInscricaoDto>(inscricao);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadInscricaoDto>>> GetAllInscricoes()
    {
        var inscricoes = await _inscricaoService.GetAllAsync(); // Use service to get all inscricoes
        return _mapper.Map<List<ReadInscricaoDto>>(inscricoes);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetInscricoesCount()
    {
        var count = await _inscricaoService.GetInscricoesCountAsync(); // Use service to get count
        return Ok(count);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInscricao(int id)
    {
        await _inscricaoService.DeleteAsync(id); // Don't assign to a variable
        return NoContent();
    }

    [HttpDelete("{participanteId}/evento/{eventoId}")]
    public async Task<IActionResult> DeleteInscricao(int participanteId, int eventoId)
    {
        var result = await _inscricaoService.DeleteInscricaoByParticipanteAndEventoAsync(participanteId, eventoId); // Use service
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("evento/{eventoId}/download")]
    public async Task<IActionResult> DownloadReport(int eventoId)
    {
        var memoryStream = new MemoryStream();
        await _inscricaoService.GenerateReportAsync(eventoId, memoryStream);

        memoryStream.Position = 0; // Resete a posição para o início do fluxo

        return File(memoryStream, "application/pdf", $"Relatorio_Evento_{eventoId}.pdf");
    }
}
