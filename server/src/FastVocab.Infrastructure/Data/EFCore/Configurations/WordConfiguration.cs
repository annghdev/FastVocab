using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class WordConfiguration : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        // Table name
        builder.ToTable("Words");

        // Primary Key
        builder.HasKey(w => w.Id);

        // Properties
        builder.Property(w => w.Text)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Meaning)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.Definition)
            .HasMaxLength(1000);

        builder.Property(w => w.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Level)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(w => w.Example1)
            .HasMaxLength(200);

        builder.Property(w => w.Example2)
            .HasMaxLength(200);

        builder.Property(w => w.Example3)
            .HasMaxLength(200);

        builder.Property(w => w.ImageUrl)
            .HasMaxLength(500);

        builder.Property(w => w.AudioUrl)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(w => w.Text)
            .IsUnique();

        builder.HasIndex(w => w.Type);

        builder.HasIndex(w => w.Level);

        builder.HasIndex(w => w.IsDeleted);

        // Composite index for common queries
        builder.HasIndex(w => new { w.Type, w.Level });

        // Relationships
        builder.HasMany(w => w.Topics)
            .WithOne(wt => wt.Word)
            .HasForeignKey(wt => wt.WordId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filter for Soft Delete
        builder.HasQueryFilter(w => !w.IsDeleted);
    }
}

