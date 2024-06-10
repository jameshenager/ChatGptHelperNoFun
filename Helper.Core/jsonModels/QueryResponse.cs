using System.Text.Json.Serialization;

namespace Helper.Core.jsonModels;

public class QueryResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("object")]
    public string? Object { get; set; }
    [JsonPropertyName("created")]
    public int Created { get; set; }
    [JsonPropertyName("model")]
    public string? Model { get; set; }
    [JsonPropertyName("choices")]
    public required List<Choice> Choices { get; set; }
    [JsonPropertyName("usage")]
    public required Usage Usage { get; set; }
    [JsonPropertyName("error")]
    public Error? Error { get; set; }
}

public class Error
{
    [JsonPropertyName("Message")] public string? Message { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("param")] public object? Param { get; set; }
    [JsonPropertyName("code")] public object? Code { get; set; }
}