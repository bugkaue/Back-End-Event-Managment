using Microsoft.AspNetCore.Identity;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Services;
public class EventoService(IEventoRepository _eventoRepository, IInscricaoRepository _inscricaoRepository, IAtividadeService _atividadeService, UserManager<Usuario> _userManager) : IEventoService, IGenericRepository<Evento>
{
    public async Task<IEnumerable<Evento>> GetAllAsync()
    {
        return await _eventoRepository.GetAllAsync();
    }

    public async Task<Evento> GetByIdAsync(int id)
    {
        return await _eventoRepository.GetByIdAsync(id);
    }

    public async Task<Evento> AddAsync(Evento entity)
    {
        return await _eventoRepository.AddAsync(entity);
    }

    public async Task UpdateAsync(Evento entity)
    {
        await _eventoRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
        {
            throw new KeyNotFoundException("Evento não encontrado.");
        }

        var atividadeRecente = new AtividadeRecente
        {
            Descricao = $"Evento excluído: {evento.Titulo}",
            DataHora = DateTime.Now,
        };

        await _atividadeService.AdicionarAtividadeAsync(atividadeRecente);
        await _eventoRepository.DeleteAsync(id);
    }
          public async Task<IEnumerable<Evento>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _eventoRepository.GetByIdsAsync(ids);
    }

       public async Task<int> GetEventosCountAsync()
    {
        var eventos = await _eventoRepository.GetAllAsync();
        return eventos.Count();
    }

    public async Task<int> GetEventosCountAsync(bool? isFinalizado)
    {
        var eventos = await _eventoRepository.GetAllAsync();
        return eventos.Count(e => e.IsFinalizado == isFinalizado);
    }

    public async Task<List<EventoSimplificadoDto>> GetInscricoesPorParticipanteId(int participanteId)
    {
        var eventos = await _inscricaoRepository.GetEventosByParticipanteIdAsync(participanteId);
        return eventos.Select(e => new EventoSimplificadoDto { EventoId = e.Id, Titulo = e.Titulo }).ToList();
    }

    public async Task<List<InscricaoCountDto>> GetInscricoesCountsByEventoIds(List<int> eventoIds)
    {
        var inscricoes = await _inscricaoRepository.GetAllAsync();
        var counts = inscricoes
            .Where(i => eventoIds.Contains(i.EventoId))
            .GroupBy(i => i.EventoId)
            .Select(g => new InscricaoCountDto { EventoId = g.Key, NumeroInscricoes = g.Count() })
            .ToList();

        return counts;
    }

    public async Task<List<Evento>> GetEventosByIdsAsync(List<int> eventoIds)
    {
        var eventos = await _eventoRepository.GetByIdsAsync(eventoIds);
        return eventos.ToList();
    }
}