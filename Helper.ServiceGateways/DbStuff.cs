using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Helper.Core.jsonModels;
using Helper.ServiceGateways.Models;
using Helper.ServiceGateways.Services;
using Microsoft.EntityFrameworkCore;

namespace Helper.ServiceGateways;

public sealed class TextService : ITextService
{
    public readonly GptContext GptContext;
    private static string ProgramName => "ChatGptHelper";

    public TextService(GptContext gptContext)
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

    public int AnswerCount() => GptContext.Answers.ToList().Count;

    public Task<List<Category>> GetCategories() => GptContext.Categories.ToListAsync();

    public Task<List<StatisticsObject>> GetStatisticsByCategory()
    {
        return GptContext.Queries
            .Include(q => q.Response!.Answers)
            .Include(q => q.Category)
            .Where(q => q.CategoryId != null)
            .GroupBy(q => new { q.Category!.Name, q.Response!.ModelUsed, })
            .AsNoTracking()
            .Select(q => new StatisticsObject
            {
                Category = q.Key.Name,
                InputTokenCount = q.Sum(query => query.TokenCount),
                OutputTokenCount = q.Sum(query => query.Response!.TokenCount),
                ModelName = q.Key.ModelUsed,
            })
            .ToListAsync();
    }

    public async Task<List<Query>> GetQueriesFiltered(HashSet<int> categoryIds, int skip = 0, int takeLimit = 100)
    {
        return await GptContext.Queries
            .Where(q => q.CategoryId == null || categoryIds.Contains((int)q.CategoryId))
            .OrderByDescending(q => q.QueryId)
            .Skip(skip)
            .Take(takeLimit)
            .Include(q => q.Response!.Answers)
            .Include(q => q.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Query>> GetQueries(int? categoryId = null, int skip = 0, int takeLimit = 100)
    {
        return await GptContext.Queries
            .Where(q => !categoryId.HasValue || q.CategoryId == categoryId)
            .OrderByDescending(q => q.QueryId)
            .Skip(skip)
            .Take(takeLimit)
            .Include(q => q.Response!.Answers)
            .ToListAsync();
    }

    public async Task<List<Query>> GetQueriesFilteredFulltext(IEnumerable<FullTextFilter> filters, int skip = 0, int takeLimit = 100)
    {
        var query = GptContext.Queries.AsQueryable();
        foreach (var filter in filters)
        {
            if (!string.IsNullOrWhiteSpace(filter.QueryText)) { query = query.Where(q => q.Text.Contains(filter.QueryText)); }
            if (!string.IsNullOrWhiteSpace(filter.ResponseText)) { query = query.Where(q => q.Response!.Answers.Any(a => a.Text.Contains(filter.ResponseText))); }
        }

        return await query
            .OrderByDescending(q => q.QueryId)
            .Skip(skip)
            .Take(takeLimit)
            .Include(q => q.Response!.Answers)
            .ToListAsync();
    }

    private static void CreateDatabaseIfNeeded()
    {
        var backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramName);
        if (!Directory.Exists(backupFolder)) { Directory.CreateDirectory(backupFolder); }
    }

    public async Task<Query> StoreQuery(string text, int tokenCount, QueryResponse qr, string modelUsed)
    {
        var query = new Query() { Text = text.Trim(), TokenCount = tokenCount, };

        var r = new Response() { Query = query, TokenCount = qr.Usage.completion_tokens, ModelUsed = modelUsed, };
        foreach (var a in qr.Choices) { r.Answers.Add(new() { Text = a.Text?.Trim() ?? "", }); }

        query.Response = r;

        var q = GptContext.Queries.Add(query);
        await GptContext.SaveChangesAsync();
        return q.Entity;
    }

    public async Task<Query> StoreQuestionAnswer(Query query)
    {
        var q = GptContext.Queries.Add(query);
        await GptContext.SaveChangesAsync();
        return q.Entity;
    }

    public async Task<Query> StoreChatQuery(string text, int tokenCount, ChatResponse qr, string modelUsed)
    {
        //ToDo: This method should be deleted and we should just use StoreQuery above. Change `ChatResponse` to implement `IResponse`.
        var query = new Query() { Text = text.Trim(), TokenCount = tokenCount, };

        var r = new Response() { Query = query, TokenCount = qr.usage.completion_tokens, ModelUsed = modelUsed, };

        if (qr.choices != null)
        {
            foreach (var a in qr.choices.Select(c => c.Message?.content)) { r.Answers.Add(new() { Text = a?.Trim() ?? string.Empty, }); }
        }
        query.Response = r;

        var q = GptContext.Queries.Add(query);
        await GptContext.SaveChangesAsync();
        return q.Entity;
    }

    public async Task<bool> UpdateCategoryForQuestionAnswer(int questionAnswerId, int? newCategoryId)
    {
        var query = await GptContext.Queries.FindAsync(questionAnswerId);
        if (query == null || !GptContext.Categories.Any(c => c.CategoryId == newCategoryId)) { return false; }

        query.CategoryId = newCategoryId;
        await GptContext.SaveChangesAsync();
        return true;
    }
}

public class FullTextFilter
{
    public string? QueryText { get; set; }
    public string? ResponseText { get; set; }
}