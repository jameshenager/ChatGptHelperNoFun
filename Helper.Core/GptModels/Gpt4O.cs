namespace Helper.Core.GptModels;

public class Gpt4O : GptModel
{
    public Gpt4O()
    {
        Name = "gpt-4o";
        ApiEndpoint = "https://api.openai.com/v1/chat/completions";
        MaxTokenSize = 120_000;
        IsChat = true;
        PricePerMillionInputTokens = 5m;
        PricePerMillionOutputTokens = 15m;
    }
}