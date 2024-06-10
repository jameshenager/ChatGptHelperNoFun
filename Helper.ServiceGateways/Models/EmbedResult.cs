namespace Helper.ServiceGateways.Models;

public class EmbedResult
{
    public int EmbedThingId { get; set; }
    public required string Text { get; set; }
    public required byte[] Vector { get; set; }
}