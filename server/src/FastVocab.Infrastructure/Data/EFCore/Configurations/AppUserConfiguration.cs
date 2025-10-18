using FastVocab.Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastVocab.Infrastructure.Data.EFCore.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        // Table name
        builder.ToTable("AppUsers");

        // Primary Key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.FullName)
            .HasMaxLength(100);

        builder.Property(u => u.SessionId)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(u => u.AccountId)
            .IsUnique()
            .HasFilter("[AccountId] IS NOT NULL");

        builder.HasIndex(u => u.SessionId);

        builder.HasIndex(u => u.IsDeleted);

        // Query Filter for Soft Delete
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}

