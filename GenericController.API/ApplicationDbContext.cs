using GenericController.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenericController.API
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entityTypes = GetType()
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
                var entity = modelBuilder
                    .Entity(entityType.Name);

                entity
                    .HasKey(nameof(IApplicationEntity.Id));

                entity
                    .Property<Guid>(nameof(IApplicationEntity.Id))
                    .ValueGeneratedOnAdd();

                entity
                    .Property<DateTime>("CreatedOn")
                    .IsRequired();

                entity
                    .Property<DateTime>("ModifiedOn")
                    .IsRequired();

                entity
                    .Property<byte[]>("RowVersion")
                    .IsRowVersion();
            }
        }
    }
}
