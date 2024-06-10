using System;
using System.Collections.Generic;

namespace Helper.ServiceGateways.Models;

public class ActionItem : IAuditable
{
    public int ActionItemId { get; set; }
    public required string? Description { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool IsCompleted { get; set; }
    public int Priority { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsDeleted { get; set; }

    // Relationships
    public int? QueryId { get; set; }
    public Query? Query { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? ActionItemScheduleId { get; set; }
    public ActionItemSchedule? ActionItemSchedule { get; set; }

    public ICollection<ActionItemDependency> Dependencies { get; set; } = new List<ActionItemDependency>();
    public ICollection<ActionItemDependency> DependedOnBy { get; set; } = new List<ActionItemDependency>();
    public ICollection<QueryActionItem> QueryActionItems { get; set; } = new List<QueryActionItem>();
}

public class ActionItemsSearchFilter 
{
    public string? Description { get; set; }
    public bool? HasScheduledDate { get; set; }
    public DateTime? MinScheduledDate { get; set; }
    public DateTime? MaxScheduledDate { get; set; }
    public bool? HasDueDate { get; set; }
    public DateTime? MinDueDate { get; set; }
    public DateTime? MaxDueDate { get; set; }
    public bool? IsCompleted { get; set; }
    public int? MinPriority { get; set; } = 00;
    public int? MaxPriority { get; set; } = 99;
    public bool? IsDeleted { get; set; } = false;
}