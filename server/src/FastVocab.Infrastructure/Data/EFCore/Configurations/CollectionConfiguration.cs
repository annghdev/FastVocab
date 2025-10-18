using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        // Table name
        builder.ToTable("Collections");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.TargetAudience)
            .HasMaxLength(100);

        builder.Property(c => c.DifficultyLevel)
            .HasMaxLength(10);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.IsHiding)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.HasIndex(c => c.IsHiding);

        builder.HasIndex(c => c.DifficultyLevel);

        builder.HasIndex(c => c.IsDeleted);

        // Relationships
        builder.HasMany(c => c.WordLists)
            .WithOne(wl => wl.Collection)
            .HasForeignKey(wl => wl.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filter for Soft Delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

