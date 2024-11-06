using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Controllers;

[ApiController]
[Route("[controller]")]
public class InscricaoController(IMapper mapper, IEventoService eventoService, IInscricaoService inscricaoService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReadInscricaoDto>> CreateInscricao(CreateInscricaoDto createInscricaoDto)
    {
        var existingInscricao = await inscricaoService.GetInscricaoByParticipanteAndEventoIdAsync(createInscricaoDto.ParticipanteId, createInscricaoDto.EventoId);
        if (existingInscricao != null)
        {
            return BadRequest("O participante já está inscrito neste evento.");
        }

        var evento = await eventoService.GetByIdAsync(createInscricaoDto.EventoId);
        if (evento == null)
        {
            return NotFound("Evento não encontrado.");
        }

        if (evento.IsFinalizado)
        {
            return BadRequest("Não é possível se inscrever em um evento que já foi finalizado.");
        }

        var inscricao = mapper.Map<Inscricao>(createInscricaoDto);
        inscricao.DataInscricao = DateTime.Now;

        await inscricaoService.AddAsync(inscricao);

        return CreatedAtAction(nameof(GetInscricaoById), new { id = inscricao.Id }, mapper.Map<ReadInscricaoDto>(inscricao));
    }

    [HttpGet("{participanteId}/eventos")]
    public async Task<ActionResult<IEnumerable<ReadEventoDto>>> GetEventosByParticipanteIdAsync(int participanteId)
    {
        var readEventoDtos = await inscricaoService.GetEventosComInscricoesByParticipanteIdAsync(participanteId);
        if (!readEventoDtos.Any())
        {
            return NotFound("Nenhum evento encontrado para o participante.");
        }

        return Ok(readEventoDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadInscricaoDto>> GetInscricaoById(int id)
    {
        var inscricao = await inscricaoService.GetByIdAsync(id);
        if (inscricao == null)
        {
            return NotFound();
        }
        return mapper.Map<ReadInscricaoDto>(inscricao);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadInscricaoDto>>> GetAllInscricoes()
    {
        var inscricoes = await inscricaoService.GetAllAsync();
        return mapper.Map<List<ReadInscricaoDto>>(inscricoes);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInscricao(int id)
    {
        await inscricaoService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("{participanteId}/evento/{eventoId}")]
    public async Task<IActionResult> DeleteInscricao(int participanteId, int eventoId)
    {
        var result = await inscricaoService.DeleteInscricaoByParticipanteAndEventoAsync(participanteId, eventoId);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetInscricoesCount()
    {
        var count = await inscricaoService.GetInscricoesCountAsync();
        return Ok(count);
    }
}
