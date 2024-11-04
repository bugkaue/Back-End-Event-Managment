using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Generic;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Implementations;

public class EventoRepository : GenericRepository<Evento>, IEventoRepository
{
    private readonly EventoContext _context;

    public EventoRepository(EventoContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Evento>> GetAllEventosAsync()
    {
        return await _context.Eventos
            .Include(e => e.Inscricoes)                
            .ToListAsync();
    }

    public async Task<int> GetEventosCountAsync()
    {
        return await _context.Eventos.CountAsync();
    }

    public async Task<int> GetEventosCountAsync(bool? isFinalizado)
    {
        if (isFinalizado.HasValue)
        {
            var eventos = await _context.Eventos.ToListAsync();
            return eventos.Count(e => isFinalizado.Value && e.DataHora <= DateTime.Now ||
                                       !isFinalizado.Value && e.DataHora > DateTime.Now);
        }

        return await GetEventosCountAsync(); 
    }
    public async Task<List<EventoSimplificadoDto>> GetInscricoesPorParticipanteId(int participanteId)
    {
        return await _context.Inscricoes
            .Where(i => i.ParticipanteId == participanteId)
            .Select(i => new EventoSimplificadoDto
            {
                EventoId = i.EventoId, // Acessando o EventoId da inscrição
                Titulo = i.Evento.Titulo, // Acessando os dados do evento
                Descricao = i.Evento.Descricao,
                DataHora = i.Evento.DataHora,
                Local = i.Evento.Local,
                CapacidadeMaxima = i.Evento.CapacidadeMaxima
            })
            .ToListAsync();
    }
    public async Task<Evento> GetEventoByIdAsync(int eventoId)
    {
        return await _context.Eventos
            .Include(e => e.Inscricoes) // Incluindo inscrições
            .FirstOrDefaultAsync(e => e.Id == eventoId);
    }

    public async Task AddEventoAsync(Evento evento)
    {
        await _context.Eventos.AddAsync(evento);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEventoAsync(Evento evento)
    { 
        _context.Eventos.Update(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<List<InscricaoCountDto>> GetInscricoesCountsByEventoIds(List<int> eventoIds)
    {
        var inscricoesCounts = await _context.Inscricoes
            .Where(i => eventoIds.Contains(i.EventoId))
            .GroupBy(i => i.EventoId)
            .Select(g => new InscricaoCountDto
            {
                EventoId = g.Key,
                NumeroInscricoes = g.Count()
            })
            .ToListAsync();

        return inscricoesCounts;
    }

    public async Task DeleteEventoAsync(int id)
    {
        var evento = await GetEventoByIdAsync(id);
        if (evento != null)
        {
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }
    }

}
