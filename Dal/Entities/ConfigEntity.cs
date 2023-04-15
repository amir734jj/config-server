using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Dal.Entities;

public class ConfigEntity : IEntityTypeConfiguration<Config>
{
    public void Configure(EntityTypeBuilder<Config> modelBuilder)
    {
        // Ensure apiKey is unique
        modelBuilder
            .HasIndex(x => x.AuthKey)
            .IsUnique();
    }
}