using System.Text.Json.Serialization;

namespace Helper.Core.jsonModels;

public class Choice
{
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("index")] public int Index { get; set; }
    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("logprobs")] public object? LogProblems { get; set; }
    [JsonPropertyName("finish_reason")] public string? FinishReason { get; set; }
    [JsonPropertyName("message")] public Message? Message { get; set; }
}