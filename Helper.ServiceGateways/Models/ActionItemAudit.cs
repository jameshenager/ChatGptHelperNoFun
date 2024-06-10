using System;

namespace Helper.ServiceGateways.Models;

public class ActionItemAudit
{
    public int ActionItemAuditId { get; set; }
    public int ActionItemId { get; set; } // Reference to the original ActionItem ID
    public string? Description { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool IsCompleted { get; set; }
    public int Priority { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DueDate { get; set; }
    public required string OperationType { get; set; } // E.g., "Update", "Delete"
    public DateTime OperationTimestamp { get; set; }
}