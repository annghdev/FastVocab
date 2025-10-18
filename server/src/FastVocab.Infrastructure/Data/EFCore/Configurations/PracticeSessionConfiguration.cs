using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSesssion>
{
    public void Configure(EntityTypeBuilder<PracticeSesssion> builder)
    {
        // Table name
        builder.ToTable("PracticeSessions");

        // Primary Key
        builder.HasKey(ps => ps.Id);

        // Properties
        builder.Property(ps => ps.UserId)
            .IsRequired();

        builder.Property(ps => ps.ListId)
            .IsRequired();

        builder.Property(ps => ps.RepetitionCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ps => ps.LastReviewed)
            .HasColumnType("datetime2");

        builder.Property(ps => ps.NextReview)
            .HasColumnType("datetime2");

        // Indexes
        builder.HasIndex(ps => ps.UserId);

        builder.HasIndex(ps => ps.ListId);

        builder.HasIndex(ps => ps.NextReview);

        builder.HasIndex(ps => ps.IsDeleted);

        // Composite unique index (One session per user per list)
        builder.HasIndex(ps => new { ps.UserId, ps.ListId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Composite index for queries
        builder.HasIndex(ps => new { ps.UserId, ps.NextReview });

        // Relationships
        builder.HasOne(ps => ps.User)
            .WithMany()
            .HasForeignKey(ps => ps.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.List)
            .WithMany()
            .HasForeignKey(ps => ps.ListId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete session when list is deleted

        // Query Filter for Soft Delete
        builder.HasQueryFilter(ps => !ps.IsDeleted);
    }
}

