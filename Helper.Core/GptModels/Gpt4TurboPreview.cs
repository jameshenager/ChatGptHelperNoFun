namespace Helper.Core.GptModels;

public class Gpt4TurboPreview : GptModel
{
    public Gpt4TurboPreview()
    {
        Name = "gpt-4-turbo-preview";
        ApiEndpoint = "https://api.openai.com/v1/chat/completions";
        MaxTokenSize = 10000;
        IsChat = true;
        PricePerMillionInputTokens = 10m;
        PricePerMillionOutputTokens = 30m;
    }
}

public class Gpt35TurboPreview : GptModel
{
    public Gpt35TurboPreview()
    {
        Name = "gpt-3.5-turbo-0125";
        ApiEndpoint = "https://api.openai.com/v1/chat/completions";
        MaxTokenSize = 10000;
        IsChat = true;
        PricePerMillionInputTokens = 10m;
        PricePerMillionOutputTokens = 30m;
    }
}