using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Generic;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories;

public class InscricaoRepository : GenericRepository<Inscricao>, IInscricaoRepository
{
    private readonly EventoContext _context;

    public InscricaoRepository(EventoContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Inscricao> GetInscricaoByIdAsync(int id)
    {
        return await _context.Inscricoes.FindAsync(id);
    }

    public async Task<IEnumerable<Inscricao>> GetAllInscricoesAsync()
    {
        return await _context.Inscricoes.ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetEventosByParticipanteIdAsync(int participanteId)
    {
        return await _context.Inscricoes
            .Where(i => i.ParticipanteId == participanteId)
            .Select(i => i.Evento)
            .ToListAsync();
    }
    public async Task<int> GetInscricoesCountByEventoIdAsync(int eventoId)
    {
        return await _context.Inscricoes.CountAsync(i => i.EventoId == eventoId);
    }

    public async Task<int> GetInscricoesCountAsync()
    {
        return await _context.Inscricoes.CountAsync();
    }

    public async Task AddInscricaoAsync(Inscricao inscricao)
    {
        await _context.Inscricoes.AddAsync(inscricao);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInscricaoAsync(Inscricao inscricao)
    {
        _context.Inscricoes.Remove(inscricao);
        await _context.SaveChangesAsync();
    }

    public async Task<Inscricao> GetInscricaoByParticipanteAndEventoIdAsync(int participanteId, int eventoId)
    {
        return await _context.Inscricoes
            .FirstOrDefaultAsync(i => i.ParticipanteId == participanteId && i.EventoId == eventoId);
    }

    public async Task<IEnumerable<Participante>> GetParticipantesPorEventoId(int eventoId)
    {
        return await _context.Inscricoes
            .Where(i => i.EventoId == eventoId)
            .Select(i => i.Participante)
            .ToListAsync();
    }

    public async Task GenerateReportAsync(int eventoId, string outputStream)
    {
        var inscricoes = await _context.Inscricoes
            .Where(i => i.EventoId == eventoId)
            .ToListAsync();

        using (var writer = new StreamWriter(outputStream))
        {
            foreach (var inscricao in inscricoes)
            {
                await writer.WriteLineAsync($"Inscricao ID: {inscricao.Id}, Participante ID: {inscricao.ParticipanteId}, Data: {inscricao.DataInscricao}");
            }
        }
    }

    public async Task<bool> DeleteInscricaoByParticipanteAndEventoAsync(int participanteId, int eventoId)
    {
        var inscricao = await _context.Inscricoes
            .FirstOrDefaultAsync(i => i.ParticipanteId == participanteId && i.EventoId == eventoId);

        if (inscricao == null)
        {
            return false;
        }

        _context.Inscricoes.Remove(inscricao);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<InscricaoCountDto>> GetInscricoesCountsByEventoIdsAsync(List<int> eventoIds)
    {
        return await _context.Inscricoes
            .Where(i => eventoIds.Contains(i.EventoId))
            .GroupBy(i => i.EventoId)
            .Select(g => new InscricaoCountDto
            {
                EventoId = g.Key,
                NumeroInscricoes = g.Count()
            })
            .ToListAsync();
    }
}
