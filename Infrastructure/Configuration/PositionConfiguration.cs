using Domain.GameAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configuration;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions");

        builder.HasKey(x => x.Fen);
        builder.Property(x => x.Fen)
            .HasComment("Позиция в стандарте FEN");

        builder.Property(x => x.YourMove)
            .HasComment("Сделанный в партии ход");

        builder.Property(x => x.WhoseMove)
            .HasConversion(new EnumToNumberConverter<Color, byte>())
            .HasComment("Чей ход в позиции");

        //builder.HasOne(x => x.PositionEvaluation) // проверить что не отвалился mapper!! после добавления навигационного поля
        //    .WithOne(x => x.Position);

        builder.Property(x => x.GameId)
            .IsRequired(false)
            .HasComment("Идентификатор игры");

        builder.HasOne(x => x.Game)
            .WithMany(x => x.Positions)
            .HasForeignKey(x => x.GameId);

        builder.Property(x => x.IsMistake)
            .HasComment("Является ли ход в партии ошибкой");
    }
}