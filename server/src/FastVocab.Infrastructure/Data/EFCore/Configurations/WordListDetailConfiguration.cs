using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class WordListDetailConfiguration : IEntityTypeConfiguration<WordListDetail>
{
    public void Configure(EntityTypeBuilder<WordListDetail> builder)
    {
        // Table name
        builder.ToTable("WordListDetails");

        // Primary Key
        builder.HasKey(wld => wld.Id);

        // Properties
        builder.Property(wld => wld.WordListId)
            .IsRequired();

        builder.Property(wld => wld.WordId)
            .IsRequired();

        // Indexes
        builder.HasIndex(wld => wld.WordListId);

        builder.HasIndex(wld => wld.WordId);

        // Composite unique index (A word can only appear once in a word list)
        builder.HasIndex(wld => new { wld.WordListId, wld.WordId })
            .IsUnique();

        // Relationships
        builder.HasOne(wld => wld.WordList)
            .WithMany(wl => wl.Words)
            .HasForeignKey(wld => wld.WordListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wld => wld.Word)
            .WithMany()
            .HasForeignKey(wld => wld.WordId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete word when removed from list
    }
}

