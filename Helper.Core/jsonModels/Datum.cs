using System.Text.Json.Serialization;

namespace Helper.Core.jsonModels;

public class Datum
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}