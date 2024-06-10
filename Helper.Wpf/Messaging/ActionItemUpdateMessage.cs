using Helper.ServiceGateways.Models;

namespace Helper.Wpf.Messaging;

public class ActionItemUpdateMessage
{
    public required ActionItem ActionItem { get; init; }
}