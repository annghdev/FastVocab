using FastVocab.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
    public void Configure(EntityTypeBuilder<FileMetadata> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(f => f.Resource).HasMaxLength(50);
        builder.Property(f => f.EntityId).HasMaxLength(50);
        builder.Property(f => f.Folder).HasMaxLength(200);
        builder.Property(f => f.Format).HasMaxLength(20);
        builder.Property(f => f.ExternalId).HasMaxLength(50);
        builder.Property(f => f.FileName).HasMaxLength(50);
        builder.Property(f => f.Type).HasMaxLength(50);

        builder.HasIndex(x => x.Resource);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.Url);
    }
}
