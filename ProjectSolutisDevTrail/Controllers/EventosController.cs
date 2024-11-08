using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Services.Interfaces;

[Route("[controller]")]
[ApiController]
public class EventosController(IEventoService _eventoService, IMapper _mapper) : ControllerBase 
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadEventoDto>>> GetAllEventos()
    {
        var eventos = await _eventoService.GetAllAsync();
        if (eventos == null)
        {
            return NotFound(new { message = "Eventos não encontrados." });
        }

        var eventoIds = eventos.Select(e => e.Id).ToList();
        var inscricoesCounts = await _eventoService.GetInscricoesCountsByEventoIds(eventoIds);

        var readEventoDtos = eventos.Select(evento =>
        {
            var dto = _mapper.Map<ReadEventoDto>(evento);
            dto.NumeroInscricoes = inscricoesCounts.FirstOrDefault(ic => ic.EventoId == evento.Id)?.NumeroInscricoes ?? 0;
            return dto;
        }).ToList();

        return Ok(readEventoDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadEventoDto>> GetEventoById(int id)
    {
        var evento = await _eventoService.GetByIdAsync(id);
        if (evento == null)
        {
            return NotFound(new { message = "Evento não encontrado." });
        }

        var inscricoesCounts = await _eventoService.GetInscricoesCountsByEventoIds(new List<int> { id });
        var readEventoDto = _mapper.Map<ReadEventoDto>(evento);
        readEventoDto.NumeroInscricoes = inscricoesCounts.FirstOrDefault(ic => ic.EventoId == evento.Id)?.NumeroInscricoes ?? 0;

        return Ok(readEventoDto);
    }

    [HttpPost]
    public async Task<ActionResult<ReadEventoDto>> AddEvento([FromBody] CreateEventoDto createEventoDto)
    {
        var evento = _mapper.Map<Evento>(createEventoDto);
        ReadEventoDto readEventoDto = _mapper.Map<ReadEventoDto>(await _eventoService.AddAsync(evento));
        return CreatedAtAction(nameof(GetEventoById), new { id = readEventoDto.Id }, readEventoDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvento(int id, [FromBody] UpdateEventoDto updateEventoDto)
    {
        var evento = await _eventoService.GetByIdAsync(id);
        if (evento == null)
        {
            return NotFound(new { message = "Evento não encontrado." });
        }

        _mapper.Map(updateEventoDto, evento);
        await _eventoService.UpdateAsync(evento);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvento(int id)
    {
        var evento = await _eventoService.GetByIdAsync(id);
        if (evento == null)
        {
            return NotFound(new { message = "Evento não encontrado." });
        }

        await _eventoService.DeleteAsync(id);
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
}