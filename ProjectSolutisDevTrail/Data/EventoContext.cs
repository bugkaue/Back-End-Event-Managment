using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectSolutisDevTrail.Models;

namespace ProjectSolutisDevTrail.Data;

public class EventoContext : IdentityDbContext<Usuario>
{
    public EventoContext(DbContextOptions<EventoContext> opts)
        : base(opts) { }

    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Inscricao> Inscricoes { get; set; }  
    public DbSet<Participante> Participantes { get; set; }
    public DbSet<AtividadeRecente> AtividadesRecentes { get; set; }

    // Configuração dos relacionamentos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        // Relacionamento entre Evento e Inscrição (1:N)
        modelBuilder.Entity<Inscricao>()
            .HasOne(i => i.Evento)
            .WithMany(e => e.Inscricoes)
            .HasForeignKey(i => i.EventoId);

        // Relacionamento entre Participante e Inscrição (1:N)
        modelBuilder.Entity<Inscricao>()
            .HasOne(i => i.Participante)
            .WithMany(p => p.Inscricoes)
            .HasForeignKey(i => i.ParticipanteId);

        // Relacionamento de 1:N entre Evento e Usuario
        modelBuilder.Entity<Evento>()
            .HasOne(e => e.Usuario)
            .WithMany(u => u.Eventos)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        // Definição da chave primária para Inscricao
        modelBuilder.Entity<Inscricao>()
            .HasKey(i => i.Id);
    }
}