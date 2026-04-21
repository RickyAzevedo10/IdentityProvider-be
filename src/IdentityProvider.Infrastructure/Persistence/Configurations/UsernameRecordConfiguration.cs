using IdentityProvider.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityProvider.Infrastructure.Persistence.Configurations;

public class UsernameRecordConfiguration : IEntityTypeConfiguration<UsernameRecord>
{
    public void Configure(EntityTypeBuilder<UsernameRecord> builder)
    {
        builder.ToTable("UsernameRecords");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("uniqueidentifier");

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}