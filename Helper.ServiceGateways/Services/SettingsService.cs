using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;

namespace Helper.ServiceGateways.Services;

public class SettingsService(GptContext gptContext) : ISettingsService
{
    public readonly GptContext GptContext = gptContext;

    public const string Ffmpeg = "ffmpeg";
    public const string MessagingMasterCode = "messagingMasterCode";
    public const string MessagingUrl = "messagingUrl";

    public string GetDatabaseFolder() => GptContext.GetDatabaseFolder();

    public Task<List<Category>> GetCategories() => GptContext.Categories.ToListAsync();

    public async Task<bool> AddCategory(Category newCategory)
    {
        if (GptContext.Categories.Any(c => c.Name == newCategory.Name)) { return false; }
        GptContext.Categories.Add(newCategory);
        await GptContext.SaveChangesAsync();
        return true;
    }

    public async Task SetFfmpegLocation(string fileLocation) => await UpsertApiKey(Ffmpeg, fileLocation);
    public async Task SetMessagingMasterCode(string masterCode) => await UpsertApiKey(MessagingMasterCode, masterCode);
    public async Task SetMessagingUrl(string url) => await UpsertApiKey(MessagingUrl, url);
    private async Task UpsertApiKey(string apiType, string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) { return; }
        if (string.IsNullOrEmpty(apiType)) { return; }

        var existing = await GptContext.ApiInformations.FirstOrDefaultAsync(i => i.ApiType == apiType);
        if (existing is not null) { existing.ApiKey = apiKey; }
        else { await GptContext.ApiInformations.AddAsync(new() { ApiKey = apiKey, ApiType = apiType, }); }
        await GptContext.SaveChangesAsync();
    }

    public async Task<string> GetFfmpegLocation() => (await GptContext.ApiInformations.FirstOrDefaultAsync(i => i.ApiType == Ffmpeg))?.ApiKey ?? "";
    public async Task<string> GetMessagingUrl() => (await GptContext.ApiInformations.FirstOrDefaultAsync(i => i.ApiType == MessagingUrl))?.ApiKey ?? "";
    public async Task<string> GetMessagingMasterCode() => (await GptContext.ApiInformations.FirstOrDefaultAsync(i => i.ApiType == MessagingMasterCode))?.ApiKey ?? "";
}