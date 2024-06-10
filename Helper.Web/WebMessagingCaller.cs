using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Helper.Core;

namespace Helper.Web;

public class WebMessagingCaller
{
    public async Task<InternetMessageRetrieval?> DeleteMessageByShortCode(string websiteUrl, string shortCode)
    {
        var client = new HttpClient();
        var response = await client.DeleteAsync($@"{websiteUrl}/InternetMessage/DeleteInternetMessageByShortCode?shortCode={shortCode}");

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"DELETE request successful");
            var jsonString = await response.Content.ReadAsStringAsync();
            return null;
        } else
        {
            Trace.WriteLine($"DELETE request failed with status code: {response.StatusCode}");
        }

        Console.WriteLine($"DELETE request failed with status code: {response.StatusCode}");
        return new InternetMessageRetrieval { ErrorMessage = "Failed to delete message", Success = false, Message = string.Empty, ShortCode = string.Empty, };
    }

    public async Task<List<InternetMessageRetrieval>?> GetAllMessages(string websiteUrl, string masterCode)
    {
        var client = new HttpClient();

        var response = await client.GetAsync($@"{websiteUrl}/InternetMessage/GetAllInternetMessages?masterCode={masterCode}");

        if (!response.IsSuccessStatusCode) { return []; }
        var jsonString = await response.Content.ReadAsStringAsync();
        var messages = JsonSerializer.Deserialize<List<InternetMessageRetrieval>>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, });
        return messages;
    }

    public async Task<InternetMessageRetrieval?> SendInternetMessage(string websiteUrl, string masterCode, string message)
    {
        var url = $"{websiteUrl}/InternetMessage/";

        var client = new HttpClient();

        var body = new InternetMessage
        {
            InternetMessageId = 0,
            MasterCode = masterCode,
            ShortCode = "string21",
            Message = message,
            TimeStamp = DateTime.Now,
            IsDeleted = false,
            File = null,
            FileName = null,
        };

        var jsonBody = JsonSerializer.Serialize(body);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        if (response.IsSuccessStatusCode) { Console.WriteLine($"POST request successful."); }
        else { Console.WriteLine($"POST request failed with status code {response.StatusCode}"); }

        var jsonString = await response.Content.ReadAsStringAsync();
        var myObject = JsonSerializer.Deserialize<InternetMessageRetrieval>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, });
        return myObject;

    }

    public async Task<InternetMessageRetrieval> GetInternetMessage(string websiteUrl, string shortCode)
    {
        var useLocal = false;
        var url = $"{websiteUrl}/InternetMessage/GetInternetMessage/";

        var client = new HttpClient();
        var completeUrl = url + $"?shortCode={shortCode}";
        var response = await client.GetAsync(completeUrl);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Get request successful");
            var jsonString = await response.Content.ReadAsStringAsync();
            var messages = JsonSerializer.Deserialize<InternetMessageRetrieval>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, });
            return messages ?? new InternetMessageRetrieval { ErrorMessage = "Failed to get message", Success = false, Message = string.Empty, ShortCode = string.Empty, };
        }

        Console.WriteLine($"Get request failed with status code: {response.StatusCode}");
        return new InternetMessageRetrieval { ErrorMessage = "Failed to get message", Success = false, Message = string.Empty, ShortCode = string.Empty, };
    }
}