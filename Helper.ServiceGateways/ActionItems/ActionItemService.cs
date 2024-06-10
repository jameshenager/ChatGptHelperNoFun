using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;

namespace Helper.ServiceGateways.ActionItems;

public class ActionItemService(GptContext dbContext) : IActionItemService
{
    public async Task<BoolMessage> AddDependencyAsync(int actionItemActionItemId, int actionItemId)
    {
        //ToDo: this code needs to be testable. Right now, it's not.

        if (actionItemActionItemId == actionItemId)
        {
            return BoolMessage.False("An action item cannot be dependent on itself.");
        }
        if (dbContext.ActionItemDependencies.Any(d => d.ActionItemId == actionItemActionItemId && d.DependsOnActionItemId == actionItemId))
        {
            return BoolMessage.False("Dependency already existed.");
        }
        if (dbContext.ActionItemDependencies.Any(d => d.ActionItemId == actionItemId && d.DependsOnActionItemId == actionItemActionItemId))
        {
            return BoolMessage.False("Reverse dependency already existed.");
        }

        if (await HasCircularDependency(actionItemActionItemId, actionItemId))
        {
            return BoolMessage.False("Adding this dependency would create a circular dependency.");
        }

        var dependency = new ActionItemDependency
        {
            ActionItemId = actionItemActionItemId,
            DependsOnActionItemId = actionItemId,
        };
        dbContext.ActionItemDependencies.Add(dependency);
        var recordsAdjusted = await dbContext.SaveChangesAsync();
        return BoolMessage.True($"Dependency successfully added. {recordsAdjusted} records adjusted.");
    }
    public async Task<IEnumerable<ActionItem>> GetActionItemsAsync(ActionItemsSearchFilter filter)
    {
        return await dbContext.ActionItems
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
    public async Task<List<ActionItem>> GetPrerequisitesAsync(int actionItemActionItemId)
    {
        return await dbContext.ActionItemDependencies.Where(d => d.ActionItemId == actionItemActionItemId)
            .Select(d => d.DependsOnActionItem)
            //.Include(ai => ai.Category)
            //.Include(ai => ai.Query)
            //.Include(ai => ai.ActionItemSchedule)
            //.Include(ai => ai.Dependencies)
            //.Include(ai => ai.DependedOnBy)
            //.Include(ai => ai.QueryActionItems)
            .ToListAsync();
    }

    public async Task<List<ActionItem>> GetDependedOnByAsync(int actionItemActionItemId)
    {
        return await dbContext.ActionItemDependencies.Where(d => d.DependsOnActionItemId == actionItemActionItemId)
            .Select(d => d.ActionItem)
            //.Include(ai => ai.Category)
            //.Include(ai => ai.Query)
            //.Include(ai => ai.ActionItemSchedule)
            //.Include(ai => ai.Dependencies)
            //.Include(ai => ai.DependedOnBy)
            //.Include(ai => ai.QueryActionItems)
            .ToListAsync();
    }
    public async Task<bool> HasCircularDependency(int startActionItemId, int newDependencyId, HashSet<int>? visited = null)
    {
        visited ??= [];

        if (!visited.Add(newDependencyId)) { return true; }

        var directDependencies = await dbContext.ActionItemDependencies
            .Where(d => d.ActionItemId == newDependencyId)
            .Select(d => d.DependsOnActionItemId)
            .ToListAsync();

        foreach (var dependencyId in directDependencies)
        {
            if (dependencyId == startActionItemId || await HasCircularDependency(startActionItemId, dependencyId, new HashSet<int>(visited))) { return true; }
        }

        return false;
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051
    private async Task<bool> HasCircularDependencyNonRecursive(int startActionItemId, int newDependencyId) //To benchmark against HasCircularDependency
#pragma warning restore IDE0051
    {
        var visited = new HashSet<int>();
        var toCheck = new Queue<int>();
        toCheck.Enqueue(newDependencyId);

        while (toCheck.Count > 0)
        {
            var currentId = toCheck.Dequeue();
            if (currentId == startActionItemId) { return true; }// Circular dependency found
            if (!visited.Add(currentId)) { continue; }// Already visited this node

            var dependencies = await dbContext.ActionItemDependencies
                .Where(d => d.ActionItemId == currentId)
                .Where(d => d.ActionItem.IsCompleted == false) //circular dependencies are only meaningful with incomplete tasks
                .Select(d => d.DependsOnActionItemId)
                .ToListAsync();

            foreach (var dependencyId in dependencies)
            {
                if (!visited.Contains(dependencyId)) { toCheck.Enqueue(dependencyId); }
            }
        }

        return false;

        /*
            Why not try to maintain a table showing all of the actionItems in a certain lineage and then grab all of those at once from the database, then traverse? 
            The point is to minimize db queries. Maintaining the table is linear. Breaking/joining "loops"/lineages might be some edge case, but it should be manageable. 
        */
    }
}