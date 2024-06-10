using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;

namespace Helper.ServiceGateways.Services;

public sealed class ImageService : IImageService
{
    public readonly GptContext GptContext;
    private static string ProgramName => "ChatGptHelper";

    public ImageService(GptContext gptContext)
    {
        GptContext = gptContext;
        CreateDatabaseIfNeeded();
    }

    public bool HasApiKey(string keyType) => GptContext.ApiInformations.Any(i => i.ApiType == keyType);

    public void SetApiKey(string newApiKey, string keyType)
    {
        GptContext.ApiInformations.Add(new() { ApiKey = newApiKey, ApiType = keyType, });
        GptContext.SaveChanges();
    }

    public string GetApiKey(string keyType) => GptContext.ApiInformations.First(api => api.ApiType == keyType).ApiKey;

    public async Task<List<ImageQuery>> GetImageQueries(int skip = 0, int takeLimit = 100) =>
        await GptContext.ImageQueries
            .OrderByDescending(iq => iq.ImageQueryId)
            .Skip(skip)
            .Take(takeLimit)
            .Include(iq => iq.ImageResults)
            .OrderBy(iq => iq.ImageQueryId)
            .ToListAsync();

    public static string CreateDatabaseIfNeeded()
    {
        var backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramName);
        if (!Directory.Exists(backupFolder)) { Directory.CreateDirectory(backupFolder); }
        return backupFolder;
    }

    public async Task<ImageQuery> StoreStuff(ImageQuery iq)
    {
        GptContext.ImageQueries.Add(iq);
        await GptContext.SaveChangesAsync();
        return iq;
    }
}