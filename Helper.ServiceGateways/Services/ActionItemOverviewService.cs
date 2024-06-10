using System;
using System.Collections.Generic;
using System.Linq;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace Helper.ServiceGateways.Services;

public class ActionItemOverviewService(GptContext gptContext) : IActionItemOverviewService
{
    public async Task<IEnumerable<ActionItem>> GetActionItemsAsync(ActionItemsSearchFilter filter)
    {
        return await gptContext.ActionItems
            .ConditionalIfNotNull(filter.IsDeleted, ai => ai.IsDeleted == filter.IsDeleted)
            .ConditionalIfNotNull(filter.HasScheduledDate, ai => ai.ScheduledDate.HasValue == filter.HasScheduledDate)
            .ConditionalIfNotNull(filter.MinScheduledDate, ai => ai.ScheduledDate >= filter.MinScheduledDate)
            .ConditionalIfNotNull(filter.MaxScheduledDate, ai => ai.ScheduledDate <= filter.MaxScheduledDate)
            .ConditionalIfNotNull(filter.HasDueDate, ai => ai.DueDate.HasValue == filter.HasDueDate)
            .ConditionalIfNotNull(filter.MinDueDate, ai => ai.DueDate >= filter.MinDueDate)
            .ConditionalIfNotNull(filter.MaxDueDate, ai => ai.DueDate <= filter.MaxDueDate)
            .ConditionalIfNotNull(filter.IsCompleted, ai => ai.IsCompleted == filter.IsCompleted)
            .ConditionalIfNotNull(filter.MinPriority, ai => ai.Priority >= filter.MinPriority)
            .ConditionalIfNotNull(filter.MaxPriority, ai => ai.Priority <= filter.MaxPriority)
            .ConditionalWhere(!string.IsNullOrWhiteSpace(filter.Description), ai => ai.Description!.Contains(filter.Description!)) //put it last since it's the most expensive
            .Include(ai => ai.Category)
            .Include(ai => ai.Query)
            .Include(ai => ai.ActionItemSchedule)
            .Include(ai => ai.Dependencies)
            .Include(ai => ai.DependedOnBy)
            .Include(ai => ai.QueryActionItems)
            .ToListAsync();
    }

    public EntityEntry<ActionItem> AddActionItem(ActionItem actionItem)
    {
        var entity = gptContext.ActionItems.Add(actionItem);
        gptContext.SaveChanges();
        return entity;
    }

    public void UpdateActionItem(ActionItem actionItem)
    {
        gptContext.ActionItems.Update(actionItem);
        gptContext.SaveChanges();
    }

    public async Task<List<ActionItem>> GetTasksForMonthAsync(DateTime currentMonth)
    {
        var firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        return await gptContext.ActionItems
            .Where(ai => ai.ScheduledDate >= firstDayOfMonth && ai.ScheduledDate <= lastDayOfMonth)
            .Include(ai => ai.Category)
            .Include(ai => ai.Query)
            .Include(ai => ai.ActionItemSchedule)
            .Include(ai => ai.Dependencies)
            .Include(ai => ai.DependedOnBy)
            .Include(ai => ai.QueryActionItems)
            .ToListAsync();
    }

    public void DeleteActionItem(ActionItem actionItem)
    {
        actionItem.IsDeleted = true;
        gptContext.SaveChanges();
    }
}