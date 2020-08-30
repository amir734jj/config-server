using System;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Dal
{
    public class EntityDbContext : DbContext
    {
        public DbSet<Config> Configs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Config>()
                .HasIndex(x => x.Key)
                .IsUnique();
        }
    }
}