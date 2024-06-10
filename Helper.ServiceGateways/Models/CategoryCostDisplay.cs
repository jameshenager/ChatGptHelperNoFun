namespace Helper.ServiceGateways.Models;

public class CategoryCostDisplay
{
    public string? Category { get; set; }
    public int InputTokenCount { get; set; }
    public int OutputTokenCount { get; set; }
    public decimal Cost { get; set; }
}