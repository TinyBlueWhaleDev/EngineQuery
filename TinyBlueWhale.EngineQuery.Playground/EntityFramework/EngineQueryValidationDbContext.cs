using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Playground.Models;

namespace TinyBlueWhale.EngineQuery.Playground.EntityFramework
{
    /// <summary>
    /// Provides an Entity Framework model used to validate EngineQuery metadata resolution.
    /// </summary>
    /// <param name="options">
    /// Database context options.
    /// </param>
    public sealed class EngineQueryValidationDbContext(
        DbContextOptions<EngineQueryValidationDbContext> options)
        : DbContext(options)
    {
        /// <summary>
        /// Gets the join orders set.
        /// </summary>
        public DbSet<JoinOrder> Orders => Set<JoinOrder>();

        /// <summary>
        /// Gets the join users set.
        /// </summary>
        public DbSet<JoinUser> Users => Set<JoinUser>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JoinOrder>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(order => order.Id);

                entity.Property(order => order.Id);

                entity.Property(order => order.UserId);

                entity.Property(order => order.Total);
            });

            modelBuilder.Entity<JoinUser>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(user => user.Id);

                entity.Property(user => user.Id);

                entity.Property(user => user.Email);
            });
        }
    }
}
