using Domain.GameAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Pgn)
            .HasComment("Pgn данные партии");

        builder.HasIndex(x => x.WhiteGamer)
            .IsUnique(false);
        builder.Property(x => x.WhiteGamer)
            .HasComment("Логин игрока с белыми фигурами");

        builder.HasIndex(x => x.BlackGamer)
           .IsUnique(false);
        builder.Property(x => x.BlackGamer)
            .HasComment("Логин игрока с черными фигурами");

        builder.Property(x => x.Result)
            .HasComment("Строка результата из Pgn");

        builder.Property(x => x.DateGame)
            .HasComment("Дата когда была сыграна партия");

        builder.HasMany(x => x.Positions)
            .WithOne(x => x.Game)
            .HasForeignKey(x => x.GameId);
    }
}