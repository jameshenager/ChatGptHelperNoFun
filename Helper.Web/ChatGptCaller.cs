using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Helper.Core.GptModels;
using Helper.Core.jsonModels;
using Helper.ServiceGateways;

namespace Helper.Web;

public class ChatGptCaller
{
    private readonly ILogger _logger;
    private static readonly HttpClient Client = new();

    public ChatGptCaller(ILogger logger)
    {
        _logger = logger;
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<int> GetEmbedCost(string apiKey, string question)
    {
        const string apiUrl = "https://api.openai.com/v1/embeddings";
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var modelToRun = "Text-embedding-ada-002";

        var requestBody = new
        {
            input = question,
            model = modelToRun,
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(apiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            var thing = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);
            return thing?.usage.total_tokens ?? 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError($"Failed to deserialize: {ex}");
            await _logger.LogError($"Request: {requestBody}");
            await _logger.LogError($"Response: {response}");
            await _logger.LogError($"Question: {question}");
            return 0;
        }
    }

    public async Task<ImageResponse?> ImageQuery(string apiKey, string question, int numberOfImages)
    {
        const string apiUrl = "https://api.openai.com/v1/images/generations";
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            prompt = $"{question}",
            n = numberOfImages,
        };
        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(apiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try { return JsonSerializer.Deserialize<ImageResponse>(jsonResponse); }
        catch (Exception ex)
        {
            await _logger.LogError($"Failed to deserialize: {ex}");
            await _logger.LogError($"Request: {requestBody}");
            await _logger.LogError($"Response: {response}");
            await _logger.LogError($"Question: {question}");
            return null;
        }
    }

    public async Task<OperationResult<QueryResponse>> Query(string apiKey, string question, GptModel modelToRun, bool deterministic)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var desiredAnswerCount = deterministic ? 1 : 5;

        var requestBody = new
        {
            prompt = $"{question}\nAnswer:",
            max_tokens = 2000,
            n = desiredAnswerCount,
            stop = "Answer:",
            temperature = deterministic ? 0 : 0.75,
            stream = false,
            model = modelToRun.Name,
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(modelToRun.ApiEndpoint, content);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            await _logger.LogError($"Status code: {response.StatusCode}");
            await _logger.LogError($"Failed: Request: {jsonRequest}");
            await _logger.LogError($"Question: {question}");
            return OperationResult<QueryResponse>.Failure($"Failed to get a valid response. Status code: {response.StatusCode}");
        }

        try
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<QueryResponse>(jsonResponse);
            return OperationResult<QueryResponse>.SuccessResult(result!);
        }
        catch (Exception ex)
        {
            await _logger.LogError($"Failed to deserialize: {ex.Message}");
            await _logger.LogError($"Request: {jsonRequest}");
            await _logger.LogError($"Response: {response}");
            await _logger.LogError($"Question: {question}");
            return OperationResult<QueryResponse>.Failure("Failed to deserialize the response.");
        }
    }

    public async Task<OperationResult<ChatResponse>> ChatQuery(string apiKey, string question, GptModel modelToRun, bool deterministic)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var desiredAnswerCount = deterministic ? 1 : 5;

        var requestBody = new
        {
            n = desiredAnswerCount,
            temperature = deterministic ? 0 : 0.75,
            stream = false,
            model = modelToRun.Name,
            messages = new List<Message> { new() { role = "user", content = question, }, },
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(modelToRun.ApiEndpoint, content);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            await _logger.LogError($"Status code: {response.StatusCode}");
            await _logger.LogError($"Failed: Request: {requestBody}");
            await _logger.LogError($"Question: {question}");
            return OperationResult<ChatResponse>.Failure($"Failed to get a valid response. Status code: {response.StatusCode}");
        }

        try
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);
            return OperationResult<ChatResponse>.SuccessResult(result!);
        }
        catch (Exception ex)
        {
            await _logger.LogError($"Failed to deserialize: {ex}");
            await _logger.LogError($"Request: {requestBody}");
            await _logger.LogError($"Response: {response}");
            await _logger.LogError($"Question: {question}");
            return OperationResult<ChatResponse>.Failure("Failed to deserialize the response.");
        }
    }

    public Task<string> TranscribeAudioAsync(string apiKey, string filePath, string model = "whisper-1", string? language = null, string? prompt = null, string responseFormat = "json", double? temperature = null) => Task.FromResult(Task.FromResult("").Result);
}