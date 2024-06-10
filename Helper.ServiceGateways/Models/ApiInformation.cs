namespace Helper.ServiceGateways.Models;

public class ApiInformation
{
    public int ApiInformationId { get; set; }
    public string ApiKey { get; set; } = null!;
    public string ApiType { get; set; } = null!;
}