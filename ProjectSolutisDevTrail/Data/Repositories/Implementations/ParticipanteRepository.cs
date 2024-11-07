using EllipticCurve;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data.Repositories.Generic;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Implementations;

public class ParticipanteRepository : IParticipanteRepository
{
    private readonly EventoContext _context;

    public ParticipanteRepository(EventoContext context)
    {
        _context = context;
    }

    public async Task<Participante> AddAsync(Participante participante)
    {
        _context.Participantes.Add(participante);
        await _context.SaveChangesAsync();
        return participante;
    }

    public async Task<Participante> GetByIdAsync(int id)
    {
        return await _context.Participantes.FindAsync(id);
    }

    public async Task<List<Participante>> GetAllAsync()
    {
        return await _context.Participantes.ToListAsync();
    }

    public async Task UpdateAsync(Participante participante)
    {
        _context.Participantes.Update(participante);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Participante participante)
    {
        _context.Participantes.Remove(participante);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Participantes.CountAsync();
    }
}