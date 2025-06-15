using Microsoft.EntityFrameworkCore;
using InvestimentosApi.Models;

namespace InvestimentosApi.Data;

public class InvestimentosDbContext : DbContext
{
    public InvestimentosDbContext(DbContextOptions<InvestimentosDbContext> options): base(options)
    {
    }

    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<Ativo> Ativo { get; set; }
    public DbSet<Operacao> Operacao { get; set; }
    public DbSet<Cotacao> Cotacao { get; set; }
    public DbSet<Posicao> Posicao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Ativo>().ToTable("ativo");
        modelBuilder.Entity<Operacao>().ToTable("operacoes");
        modelBuilder.Entity<Cotacao>().ToTable("cotacao");
        modelBuilder.Entity<Posicao>().ToTable("posicao");
    }
}