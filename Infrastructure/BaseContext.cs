using Domain.GameAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class BaseContext : DbContext
{
    private readonly string _baseName = "BaseAnalyzeGames.db";
    // закомментил попробую так
    //public DbSet<Game> Games => Set<Game>();
    //public DbSet<NonParsedGame> NonParsedGames => Set<NonParsedGame>();
    //public DbSet<Position> Positions => Set<Position>();
    //public DbSet<PositionEvaluation> PositionEvaluations => Set<PositionEvaluation>();
    //public DbSet<Mistake> Mistakes => Set<Mistake>();
    //public DbSet<Player> Players => Set<Player>();

    public BaseContext() => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseContext).Assembly);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_baseName}");
    }
}