using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Helper.ServiceGateways;

public class DateAddedSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetDateAdded(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetDateAdded(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetDateAdded(DbContext context)
    {
        var entries = context.ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added);

        var currentTime = DateTime.UtcNow; // or DateTime.Now

        foreach (var entry in entries)
        {
            var propertyInfo = entry.Entity.GetType().GetProperty("DateAdded");
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime))
            {
                propertyInfo.SetValue(entry.Entity, currentTime, null);
            }
        }
    }
}