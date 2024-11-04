using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;
using System.Security.Claims;

namespace ProjectSolutisDevTrail.Controllers;

[Route("[controller]")]
[ApiController]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;
    private readonly IMapper _mapper;
    private readonly IEventoRepository _eventoRepository;
    private readonly IAtividadeService _atividadeService;

    public EventosController(IEventoService eventoService, IAtividadeService atividadeService, IMapper mapper, IEventoRepository eventoRepository)
    {
        _eventoService = eventoService;
        _mapper = mapper;
        _eventoRepository = eventoRepository;
        _atividadeService = atividadeService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ReadEventoDto>> CreateEvento(CreateEventoDto createEventoDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized("Usuário não identificado.");
        }

        var evento = _mapper.Map<Evento>(createEventoDto);
        await _eventoService.AddEventoAsync(evento);

        var atividadeRecente = new AtividadeRecente
        {
            Descricao = "Novo evento criado",
            DataHora = DateTime.Now,
            UsuarioId = usuarioId // Atribui o ID do usuário logado
        };
        await _atividadeService.AdicionarAtividadeAsync(atividadeRecente);

        var readEventoDto = _mapper.Map<ReadEventoDto>(evento);
        return CreatedAtAction(nameof(GetEventoById), new { id = evento.Id }, readEventoDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadEventoDto>> GetEventoById(int id)
    {
        // Fetch the event using the service
        var evento = await _eventoService.GetEventoByIdAsync(id);
        if (evento == null)
        {
            return NotFound(new { message = "Evento não encontrado." });
        }

        // Fetch the registration count for the specific event
        var inscricoesCounts = await _eventoRepository.GetInscricoesCountsByEventoIds(new List<int> { id });

        // Create ReadEventoDto with registration count
        var readEventoDto = new ReadEventoDto
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

        return Ok(readEventoDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadEventoDto>>> GetAllEventos()
    {
        var eventos = await _eventoService.GetAllEventosAsync();
        var eventoIds = eventos.Select(e => e.Id).ToList();

        // Use the method that retrieves counts for multiple event IDs
        var inscricoesCounts = await _eventoRepository.GetInscricoesCountsByEventoIds(eventoIds);

        var readEventoDtos = eventos.Select(evento => new ReadEventoDto
        {
            Id = evento.Id,
            Titulo = evento.Titulo,
            Descricao = evento.Descricao,
            DataHora = evento.DataHora,
            Local = evento.Local,
            CapacidadeMaxima = evento.CapacidadeMaxima,
            IsAtivo = evento.IsAtivo,
            NumeroInscricoes = inscricoesCounts.FirstOrDefault(ic => ic.EventoId == evento.Id)?.NumeroInscricoes ?? 0
        }).ToList();

        return Ok(readEventoDtos);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvento(int id, UpdateEventoDto updateEventoDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized("Usuário não identificado.");
        }

        var evento = await _eventoService.GetEventoByIdAsync(id);
        if (evento == null)
        {
            return NotFound(new { message = "Evento não encontrado." });
        }

        _mapper.Map(updateEventoDto, evento);
        await _eventoService.UpdateEventoAsync(evento);

        var atividadeRecente = new AtividadeRecente
        {
            Descricao = $"Evento atualizado: {evento.Titulo}",
            DataHora = DateTime.Now,
            UsuarioId = usuarioId
        };
        await _atividadeService.AdicionarAtividadeAsync(atividadeRecente);

        return NoContent();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetEventosCount()
    {
        var count = await _eventoService.GetEventosCountAsync();
        return Ok(count);
    }

    [HttpGet("count/finalizados")]
    public async Task<ActionResult<int>> GetEventosFinalizadosCount()
    {
        var count = await _eventoService.GetEventosCountAsync(true);
        return Ok(count);
    }

    [HttpGet("participante/{participanteId}/inscricoes")]
    public async Task<IActionResult> GetInscricoesPorParticipante(int participanteId)
    {
        var inscricoes = await _eventoRepository.GetInscricoesPorParticipanteId(participanteId);
        if (inscricoes == null || !inscricoes.Any())
        {
            return NotFound("Nenhuma inscrição encontrada para este participante.");
        }

        return Ok(inscricoes);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvento(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized("Usuário não identificado.");
        }

        var evento = await _eventoService.GetEventoByIdAsync(id);
        if (evento == null)
        {
            return NotFound(new { message = "Evento não encontrado." });
        }

        await _eventoService.DeleteEventoAsync(id);

        var atividadeRecente = new AtividadeRecente
        {
            Descricao = $"Evento excluído: {evento.Titulo}",
            DataHora = DateTime.Now,
            UsuarioId = usuarioId
        };
        await _atividadeService.AdicionarAtividadeAsync(atividadeRecente);

        return NoContent();
    }
}
