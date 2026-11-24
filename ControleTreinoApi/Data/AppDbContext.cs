using Microsoft.EntityFrameworkCore;
using ControleTreinoApi.Models;

namespace ControleTreinoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Usuário 
        mb.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        mb.Entity<Usuario>().Property(u => u.Email).HasMaxLength(200);
        mb.Entity<Usuario>().Property(u => u.Nome).HasMaxLength(100);

        // A ordem das séries é única dentro do treino
        mb.Entity<TreinoItem>()
            .HasIndex(i => new { i.TreinoId, i.Ordem })
            .IsUnique();

        // Excluir o treino apaga só os itens do plano
        mb.Entity<TreinoItem>()
            .HasOne(i => i.Treino)
            .WithMany()
            .HasForeignKey(i => i.TreinoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Exercício usado em treino NÃO pode ser excluído
        mb.Entity<TreinoItem>()
            .HasOne(i => i.Exercicio)
            .WithMany()
            .HasForeignKey(i => i.ExercicioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sessão e registros (a execução) 
        // Excluir treino NÃO apaga as sessões: TreinoId vira null
        mb.Entity<Sessao>()
            .HasOne(s => s.Treino)
            .WithMany()
            .HasForeignKey(s => s.TreinoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Excluir a sessão apaga os registros dela
        mb.Entity<RegistroSerieExecutada>()
            .HasOne(r => r.Sessao)
            .WithMany()
            .HasForeignKey(r => r.SessaoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Exercício com histórico não pode ser excluído
        mb.Entity<RegistroSerieExecutada>()
            .HasOne(r => r.Exercicio)
            .WithMany()
            .HasForeignKey(r => r.ExercicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Exercicio> Exercicios { get; set; }
    public DbSet<Treino> Treinos { get; set; }
    public DbSet<TreinoItem> TreinoItems { get; set; }
    public DbSet<Sessao> Sessoes { get; set; }
    public DbSet<RegistroSerieExecutada> RegistrosSerieExecutadas { get; set; }
}