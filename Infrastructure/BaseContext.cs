using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class BaseContext : DbContext
{
    // после регистрации контекста в DI контейнере нужно обязательно определить конструктор с (DbContextOptions options) : base(options)
    public BaseContext(DbContextOptions options) : base(options) => Database.EnsureCreated();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseContext).Assembly);

}