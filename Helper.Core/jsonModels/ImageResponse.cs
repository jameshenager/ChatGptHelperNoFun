using System.Text.Json.Serialization;

namespace Helper.Core.jsonModels;

public class ImageResponse
{
    [JsonPropertyName("created")] public int Created { get; set; }
    [JsonPropertyName("Data")] public List<Datum> Data { get; set; }
}