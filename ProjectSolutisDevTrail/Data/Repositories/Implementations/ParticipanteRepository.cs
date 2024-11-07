using EllipticCurve;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data.Repositories.Generic;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data.Repositories.Implementations;

public class ParticipanteRepository(EventoContext context) : GenericRepository<Participante>(context), IParticipanteRepository
{
    public async Task<Participante> AddAsync(Participante participante)
    {
        context.Participantes.Add(participante);
        await context.SaveChangesAsync();
        return participante;
    }

    public async Task<Participante> GetByIdAsync(int id)
    {
        return await context.Participantes.FindAsync(id);
    }

    public async Task<List<Participante>> GetAllAsync()
    {
        return await context.Participantes.ToListAsync();
    }

    public async Task UpdateAsync(Participante participante)
    {
        context.Participantes.Update(participante);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Participante participante)
    {
        context.Participantes.Remove(participante);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await context.Participantes.CountAsync();
    }
}