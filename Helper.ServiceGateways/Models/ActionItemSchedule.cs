using System;

namespace Helper.ServiceGateways.Models;

public class ActionItemSchedule
{
    public int ActionItemScheduleId { get; set; }
    public int ActionItemId { get; set; }
    public required ActionItem ActionItem { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public TimeSpan? Duration { get; set; }
}