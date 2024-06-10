namespace Helper.Core.GptModels;

public abstract class GptModel
{
    public string? Name { get; protected set; }
    public string? ApiEndpoint { get; protected set; }
    public int MaxTokenSize { get; protected set; }
    public bool IsChat { get; protected set; }
    public decimal PricePerMillionInputTokens { get; protected set; }
    public decimal PricePerMillionOutputTokens { get; protected set; }
    public static List<GptModel> StaticModels { get; } = [new TextDavinci003(), new Gpt35TurboPreview(), new Gpt41106Preview(), new Gpt4TurboPreview(), new Gpt4O(),];
}