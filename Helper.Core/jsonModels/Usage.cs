// ReSharper disable InconsistentNaming
namespace Helper.Core.jsonModels;

#pragma warning disable IDE1006 // Naming Styles
public class Usage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class ChatResponse
{
    public Choice[]? choices { get; set; }
    public int created { get; set; }
    public required string id { get; set; }
    public required string model { get; set; }
    public required string @object { get; set; }
    public required string system_fingerprint { get; set; }
    public required Usage usage { get; init; }
    public Error? error { get; set; }

}

public class Message
{
    public required string content { get; set; }
    public required string role { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles