using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helper.Core;
using Helper.Core.Utils;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;

namespace Helper.ServiceGateways.Services;

public sealed class EmbedService : IEmbedService
{
    public readonly GptContext GptContext;
    private static string ProgramName => "ChatGptHelper";

    public EmbedService(GptContext gptContext)
    {
        GptContext = gptContext;
        CreateDatabaseIfNeeded();
    }

    public bool HasApiKey(string keyType) => GptContext.ApiInformations.Any(i => i.ApiType == keyType);
    public string GetApiKey(string keyType) => GptContext.ApiInformations.First(api => api.ApiType == keyType).ApiKey;
    public int GetEmbedCount() => GptContext.EmbedResults.Count();

    public void SetApiKey(string newApiKey, string keyType)
    {
        GptContext.ApiInformations.Add(new() { ApiKey = newApiKey, ApiType = keyType, });
        GptContext.SaveChanges();
    }

    public List<EmbedPiece> GetBestEmbeds(float[] vector, int desiredCount = 100)
    {
        var totalEmbedCount = GetEmbedCount();
        var batchSize = 1000;
        var currentThing = 0;

        var top100 = new List<EmbedPiece>(desiredCount * 2);

        while (currentThing < totalEmbedCount)
        {
            var tempThing = GptContext.EmbedResults
                .OrderByDescending(er => er.EmbedThingId)
                .Take(batchSize)
                .ToList()
                .Select(er => new EmbedPiece
                {
                    EmbedThingId = er.EmbedThingId,
                    Text = er.Text,
                    Vector = EmbedPiece.DeserializeVector(er.Vector),
                }).OrderByDescending(ep => MathHelper.DotProduct(ep.Vector, vector))
                .Take(desiredCount);
            top100.AddRange(tempThing);
            top100 = top100
                .OrderByDescending(ep => MathHelper.DotProduct(ep.Vector, vector))
                .Take(desiredCount)
                .ToList();
            currentThing += batchSize;
        }
        return top100;
    }

    public async Task<List<EmbedPiece>> GetSomeEmbeds(int skip = 0, int takeLimit = 100) =>
        await GptContext.EmbedResults
            .OrderByDescending(er => er.EmbedThingId)
            .Skip(skip)
            .Take(takeLimit)
            .OrderBy(er => er.EmbedThingId)
            .Select(er => new EmbedPiece
            {
                EmbedThingId = er.EmbedThingId,
                Text = er.Text,
                Vector = EmbedPiece.DeserializeVector(er.Vector),
            })
            .ToListAsync();

    public static string CreateDatabaseIfNeeded()
    {
        var backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramName);
        if (!Directory.Exists(backupFolder)) { Directory.CreateDirectory(backupFolder); }
        return backupFolder;
    }

    public async Task StoreEmbed(EmbedResult e)
    {
        GptContext.EmbedResults.Add(e);
        await GptContext.SaveChangesAsync();
    }
}