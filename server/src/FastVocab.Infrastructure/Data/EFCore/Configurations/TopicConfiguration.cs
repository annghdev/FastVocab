using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        // Table name
        builder.ToTable("Topics");

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.VnText)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ImageUrl)
            .HasMaxLength(500);

        builder.Property(t => t.IsHiding)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.HasIndex(t => t.IsHiding);

        builder.HasIndex(t => t.IsDeleted);

        // Relationships
        builder.HasMany(t => t.Words)
            .WithOne(wt => wt.Topic)
            .HasForeignKey(wt => wt.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filter for Soft Delete
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}

