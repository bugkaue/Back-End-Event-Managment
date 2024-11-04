using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipanteController : ControllerBase
{
    private readonly EventoContext _context;
    private readonly IMapper _mapper;

    public ParticipanteController(EventoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<ReadParticipanteDto>> CreateParticipante(CreateParticipanteDto createParticipanteDto)
    {
        var participante = _mapper.Map<Participante>(createParticipanteDto);
        _context.Participantes.Add(participante);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetParticipanteById), new { id = participante.Id }, _mapper.Map<ReadParticipanteDto>(participante));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReadParticipanteDto>> GetParticipanteById(int id)
    {
        var participante = await _context.Participantes.FindAsync(id);
        if (participante == null)
        {
            return NotFound();
        }
        return _mapper.Map<ReadParticipanteDto>(participante);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadParticipanteDto>>> GetAllParticipantes()
    {
        var participantes = await _context.Participantes.ToListAsync();
        return _mapper.Map<List<ReadParticipanteDto>>(participantes);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipante(int id, UpdateParticipanteDto updateParticipanteDto)
    {
        if (id != updateParticipanteDto.Id)
        {
            return BadRequest();
        }

        var participante = await _context.Participantes.FindAsync(id);
        if (participante == null)
        {
            return NotFound();
        }

        _mapper.Map(updateParticipanteDto, participante);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipante(int id)
    {
        var participante = await _context.Participantes.FindAsync(id);
        if (participante == null)
        {
            return NotFound();
        }

        _context.Participantes.Remove(participante);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("count")] // Nova rota para contar participantes
    public async Task<ActionResult<int>> GetParticipanteCount()
    {
        var count = await _context.Participantes.CountAsync(); // Conta o número total de participantes
        return Ok(count); // Retorna a contagem em um formato de sucesso
    }
}
