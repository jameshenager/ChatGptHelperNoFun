namespace Helper.Core.GptModels;

public class TextDavinci003 : GptModel
{
    public TextDavinci003()
    {
        Name = "text-davinci-003";
        ApiEndpoint = "https://api.openai.com/v1/completions";
        MaxTokenSize = 20_000;
        PricePerMillionInputTokens = 2m;
        PricePerMillionOutputTokens = 0m;
    }
}