using Domain.GameAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Configuration;

public class PositionEvaluationConfiguration : IEntityTypeConfiguration<PositionEvaluation>
{
    public void Configure(EntityTypeBuilder<PositionEvaluation> builder)
    {
        builder.ToTable("PositionEvaluations");

        builder.HasKey(x => x.Fen);
        builder.Property(x => x.Fen)
            .HasComment("Позиция в стандарте FEN");

        builder.Property(x => x.Depth)
          .HasComment("Глубина на которой была посчитана оценка");

        builder.Property(x => x.Cp)
            .HasComment("Оценка позиции");

        builder.Property(x => x.OneMove)
            .HasComment("Сильнейший ход на глубине Depth");
    }
}