using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class WordTopicConfiguration : IEntityTypeConfiguration<WordTopic>
{
    public void Configure(EntityTypeBuilder<WordTopic> builder)
    {
        // Table name
        builder.ToTable("WordTopics");

        // Primary Key
        builder.HasKey(wt => wt.Id);

        // Properties
        builder.Property(wt => wt.WordId)
            .IsRequired();

        builder.Property(wt => wt.TopicId)
            .IsRequired();

        // Indexes
        builder.HasIndex(wt => wt.WordId);

        builder.HasIndex(wt => wt.TopicId);

        // Composite unique index (A word can only be associated with a topic once)
        builder.HasIndex(wt => new { wt.WordId, wt.TopicId })
            .IsUnique();

        // Relationships
        builder.HasOne(wt => wt.Word)
            .WithMany(w => w.Topics)
            .HasForeignKey(wt => wt.WordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wt => wt.Topic)
            .WithMany(t => t.Words)
            .HasForeignKey(wt => wt.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

