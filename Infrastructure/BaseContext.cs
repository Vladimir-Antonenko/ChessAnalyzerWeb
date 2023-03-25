using Domain.GameAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class BaseContext : DbContext
{

    // public BaseContext() => Database.EnsureCreated();
    // после регистрации контекста в program нужно определить конструктор с (DbContextOptions options) : base(options)
    public BaseContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseContext).Assembly);

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite($"Data Source={_baseName}");
    //}
}