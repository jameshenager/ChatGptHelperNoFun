namespace Helper.ServiceGateways.Models;

public class StatisticsObject
{
    public string? Category { get; set; }
    public int InputTokenCount { get; set; }
    public int OutputTokenCount { get; set; }
    public required string ModelName { get; set; }
}