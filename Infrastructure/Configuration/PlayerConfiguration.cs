using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.GameAggregate;

namespace Infrastructure.Configuration;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasComment("Имя/логин игрока");

        builder.HasMany(x => x.Games);

        builder.HasMany(x => x.Mistakes);
    }
}