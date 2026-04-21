using IdentityProvider.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityProvider.Infrastructure.Persistence.Configurations;

public class PasswordPartialConfiguration : IEntityTypeConfiguration<PasswordPartial>
{
    public void Configure(EntityTypeBuilder<PasswordPartial> builder)
    {
        builder.ToTable("PasswordPartials");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnType("uniqueidentifier");

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(p => p.PositionIndexes)
            .IsRequired()
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                new ValueComparer<int[]>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray()))
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(p => p.Hash)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        builder.HasIndex(p => p.UserId);
    }
}