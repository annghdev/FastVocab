using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class TakedWordConfiguration : IEntityTypeConfiguration<TakedWord>
{
    public void Configure(EntityTypeBuilder<TakedWord> builder)
    {
        // Table name
        builder.ToTable("TakedWords");

        // Primary Key
        builder.HasKey(tw => tw.Id);

        // Properties
        builder.Property(tw => tw.UserId)
            .IsRequired();

        builder.Property(tw => tw.WordId)
            .IsRequired();

        builder.Property(tw => tw.RepetitionCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(tw => tw.EasinessFactor)
            .IsRequired()
            .HasDefaultValue(2.5)
            .HasPrecision(3, 2); // e.g., 2.50

        builder.Property(tw => tw.LastReviewed)
            .HasColumnType("datetime2");

        builder.Property(tw => tw.NextReview)
            .HasColumnType("datetime2");

        builder.Property(tw => tw.LastGrade)
            .HasConversion<int?>(); // Store enum as int

        // Indexes
        builder.HasIndex(tw => tw.UserId);

        builder.HasIndex(tw => tw.WordId);

        builder.HasIndex(tw => tw.NextReview);

        builder.HasIndex(tw => tw.LastGrade);

        // Composite unique index (One record per user per word)
        builder.HasIndex(tw => new { tw.UserId, tw.WordId })
            .IsUnique();

        // Composite index for SRS queries
        builder.HasIndex(tw => new { tw.UserId, tw.NextReview });

        builder.HasIndex(tw => new { tw.UserId, tw.LastGrade });

        // Relationships
        builder.HasOne(tw => tw.User)
            .WithMany()
            .HasForeignKey(tw => tw.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tw => tw.Word)
            .WithMany()
            .HasForeignKey(tw => tw.WordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

