using FER.InfSus.Time.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<UserTaskboardAssociation> UserTaskboardAssociations { get; set; } = null!;
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<TaskItemHistoryLog> TaskItemHistoryLogs { get; set; } = null!;
        public DbSet<Taskboard> Taskboards { get; set; } = null!;
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
