using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsightHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightHub.Infrastructure.Data.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Dataset> Datasets => Set<Dataset>();

    public DbSet<DatasetColumn> DatasetColumns => Set<DatasetColumn>();

    public DbSet<DatasetColumnValue> DatasetColumnValues
    => Set<DatasetColumnValue>();

    public DbSet<DatasetRow> DatasetRows { get; set; }

    public DbSet<SavedAnalysis> SavedAnalyses => Set<SavedAnalysis>();

    public DbSet<Workspace> Workspaces => Set<Workspace>();

    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Dataset>()
            .HasOne(d => d.User)
            .WithMany(u => u.Datasets)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SavedAnalysis>()
            .HasOne(sa => sa.User)
            .WithMany(u => u.SavedAnalyses)
            .HasForeignKey(sa => sa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SavedAnalysis>()
            .HasOne(sa => sa.Dataset)
            .WithMany()
            .HasForeignKey(sa => sa.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}