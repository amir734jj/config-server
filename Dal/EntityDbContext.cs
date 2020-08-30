using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal
{
    public sealed class EntityDbContext : DbContext
    {
        public DbSet<Config> Configs { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Constructor that will be called by startup.cs
        /// </summary>
        /// <param name="optionsBuilderOptions"></param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public EntityDbContext(DbContextOptions<EntityDbContext> optionsBuilderOptions) : base(optionsBuilderOptions)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure apiKey is unique
            modelBuilder.Entity<Config>()
                .HasIndex(x => x.Key)
                .IsUnique();
        }
    }
}
