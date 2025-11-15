using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UMS.Core.Entities.Academic;
using UMS.Core.Entities.Identity;

namespace UMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations except TenantConfiguration (which belongs to MasterDbContext only)
            var assembly = typeof(ApplicationDbContext).Assembly;
            var configTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType &&
                       t.GetInterfaces().Any(i => i.IsGenericType && 
                       i.GetGenericTypeDefinition() == typeof(Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<>)))
                .Where(t => t.Name != "TenantConfiguration"); // Exclude TenantConfiguration

            foreach (var configType in configTypes)
            {
                var configurationInstance = Activator.CreateInstance(configType);
                if (configurationInstance is null)
                {
                    continue;
                }

                modelBuilder.ApplyConfiguration((dynamic)configurationInstance);
            }
        }
    }
}