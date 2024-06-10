using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Helper.ServiceGateways.ModelBuilder;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace Helper.ServiceGateways;

public sealed class GptContext : DbContext
{
    private const string DatabaseFilename = "chatGPT.db";
    public static string ProgramName => "ChatGptHelper";

    public DbSet<Query> Queries { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<ApiInformation> ApiInformations { get; set; }
    public DbSet<ImageQuery> ImageQueries { get; set; }
    public DbSet<ImageResult> ImageResults { get; set; }
    public DbSet<EmbedResult> EmbedResults { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<ActionItem> ActionItems { get; set; }
    public DbSet<ActionItemAudit> ActionItemAudits { get; set; }
    public DbSet<ActionItemDependency> ActionItemDependencies { get; set; }
    public DbSet<ActionItemSchedule> ActionItemSchedules { get; set; }

    private string DbPath { get; set; }

    public GptContext()
    {
        EnsureFolderExists();
        Database.EnsureCreated(); //Comment this line out when creating migrations
    }

    public string GetDatabaseFolder()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var backupFolder = Path.Combine(appDataFolder, "ChatGptHelper");
        return backupFolder;
    }

    private void EnsureFolderExists()
    {
        var backupFolder = GetDatabaseFolder();
        DbPath = Path.Combine(backupFolder, DatabaseFilename);
        if (!Directory.Exists(backupFolder)) { Directory.CreateDirectory(backupFolder); }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
        options.AddInterceptors(new DateAddedSaveChangesInterceptor());
    }

    public void PreprocessSave()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditable auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.DateCreated = utcNow;
                        auditable.DateUpdated = utcNow;
                        break;
                    case EntityState.Modified:
                        auditable.DateUpdated = utcNow;
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<ActionItem>().Where(e => e.State == EntityState.Modified))
        {
            Set<ActionItemAudit>().Add(CreateAuditEntry(entry.Entity, "Update"));
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PreprocessSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        PreprocessSave();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private static ActionItemAudit CreateAuditEntry(ActionItem entity, string operationType)
    {
        return new ActionItemAudit
        {
            ActionItemId = entity.ActionItemId,
            Description = entity.Description,
            DateCreated = entity.DateCreated,
            DateUpdated = DateTime.UtcNow, // Capture the current time as the operation time
            IsCompleted = entity.IsCompleted,
            Priority = entity.Priority,
            ScheduledDate = entity.ScheduledDate,
            DueDate = entity.DueDate,
            OperationType = operationType,
            OperationTimestamp = DateTime.UtcNow,
        };
    }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder mb)
    {
        mb.ApplyConfiguration(new ActionItemConfiguration());
        mb.ApplyConfiguration(new ActionItemDependencyConfiguration());
        mb.ApplyConfiguration(new CategoryConfiguration());
        mb.ApplyConfiguration(new QueryActionItemConfiguration());
        mb.ApplyConfiguration(new QueryConfiguration());

        mb.Entity<Answer>().Navigation(a => a.Response).UsePropertyAccessMode(PropertyAccessMode.Property);
        mb.Entity<ApiInformation>()
            .Property(a => a.ApiType)
            .HasMaxLength(255);
        mb.Entity<EmbedResult>().HasKey(er => er.EmbedThingId);
        mb.Entity<ImageResult>().Navigation(ir => ir.ImageQuery).UsePropertyAccessMode(PropertyAccessMode.Property);
        mb.Entity<ImageQuery>().Navigation(iq => iq.ImageResults).UsePropertyAccessMode(PropertyAccessMode.Property);
        mb.Entity<Response>().Navigation(r => r.Answers).UsePropertyAccessMode(PropertyAccessMode.Property);
        mb.Entity<ActionItemSchedule>()
            .HasKey(ais => ais.ActionItemScheduleId);

        //dotnet ef migrations add Initial
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update
    }
}