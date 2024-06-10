namespace Helper.ServiceGateways.Models;

public class ActionItemDependency
{
    public int ActionItemId { get; set; }
    public ActionItem ActionItem { get; set; } = null!;

    public int DependsOnActionItemId { get; set; }
    public ActionItem DependsOnActionItem { get; set; } = null!;
}