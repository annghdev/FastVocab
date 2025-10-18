using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class WordListConfiguration : IEntityTypeConfiguration<WordList>
{
    public void Configure(EntityTypeBuilder<WordList> builder)
    {
        // Table name
        builder.ToTable("WordLists");

        // Primary Key
        builder.HasKey(wl => wl.Id);

        // Properties
        builder.Property(wl => wl.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wl => wl.ImageUrl)
            .HasMaxLength(500);

        builder.Property(wl => wl.CollectionId)
            .IsRequired();

        // Indexes
        builder.HasIndex(wl => wl.CollectionId);

        builder.HasIndex(wl => wl.IsDeleted);

        // Composite unique index (Name must be unique within a Collection)
        builder.HasIndex(wl => new { wl.Name, wl.CollectionId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasOne(wl => wl.Collection)
            .WithMany(c => c.WordLists)
            .HasForeignKey(wl => wl.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(wl => wl.Words)
            .WithOne(wld => wld.WordList)
            .HasForeignKey(wld => wld.WordListId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filter for Soft Delete
        builder.HasQueryFilter(wl => !wl.IsDeleted);
    }
}

