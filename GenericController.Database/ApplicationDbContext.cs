using GenericController.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenericController.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityTypes = this.GetType()
                .GetProperties()
                .Select(p => p.PropertyType)
                .Where(pt => pt.IsGenericType
                    && pt.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(pt => pt.GetGenericArguments()[0])
                .Where(t => typeof(IApplicationEntity).IsAssignableFrom(t)
                    && t.IsClass
                    && !t.IsAbstract);

            foreach (var entityType in entityTypes)
            {
                modelBuilder
                    .Entity(entityType)
                    .HasKey(nameof(IApplicationEntity.Id));

                modelBuilder
                    .Entity(entityType)
                    .Property<Guid>(nameof(IApplicationEntity.Id))
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                modelBuilder
                    .Entity(entityType)
                    .Property<DateTime?>("CreatedOn")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                modelBuilder
                    .Entity(entityType)
                    .Property<DateTime?>("ModifiedOn")
                    .IsRequired()
                    .ValueGeneratedOnAddOrUpdate();

                modelBuilder
                    .Entity(entityType)
                    .Property<byte[]>("RowVersion")
                    .IsRequired()
                    .IsRowVersion();

                modelBuilder
                    .Entity(entityType)
                    .Property<bool?>("IsDeleted")
                    .IsRequired()
                    .HasDefaultValue(false);
            }
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            builder.UseSqlServer("");

            return new ApplicationDbContext(builder.Options);
        }
    }
}
