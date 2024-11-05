﻿using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;

namespace ProjectSolutisDevTrail.Data.Repositories.Generic;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly EventoContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(EventoContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _dbSet.Where(e => ids.Contains((int)typeof(T).GetProperty("Id").GetValue(e))).ToListAsync();
    }
}
