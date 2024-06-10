namespace Helper.Core.GptModels;

public class Gpt41106Preview : GptModel
{
    public Gpt41106Preview()
    {
        Name = "gpt-4-1106-preview";
        ApiEndpoint = "https://api.openai.com/v1/chat/completions";
        MaxTokenSize = 1000; // Example value
        IsChat = true;
        PricePerMillionInputTokens = 10m;
        PricePerMillionOutputTokens = 30m;
    }
}