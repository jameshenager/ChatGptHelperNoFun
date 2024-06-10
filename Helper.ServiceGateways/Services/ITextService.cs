using System.Collections.Generic;
using System.Threading.Tasks;
using Helper.Core.jsonModels;
using Helper.ServiceGateways.Models;

namespace Helper.ServiceGateways.Services;

public interface ITextService
{
    bool HasApiKey(string keyType);
    void SetApiKey(string newApiKey, string keyType);
    string GetApiKey(string keyType);
    Task<List<Category>> GetCategories();
    int AnswerCount();
    Task<List<Query>> GetQueriesFiltered(HashSet<int> categoryIds, int skip = 0, int takeLimit = 100);
    Task<List<StatisticsObject>> GetStatisticsByCategory();
    Task<List<Query>> GetQueries(int? categoryId = null, int skip = 0, int takeLimit = 100);
    Task<Query> StoreQuery(string text, int tokenCount, QueryResponse qr, string modelUsed);
    Task<Query> StoreChatQuery(string text, int tokenCount, ChatResponse qr, string modelUsed);
    Task<bool> UpdateCategoryForQuestionAnswer(int questionAnswerId, int? newCategoryId);
    Task<List<Query>> GetQueriesFilteredFulltext(IEnumerable<FullTextFilter> filters, int skip = 0, int takeLimit = 100);
    Task<Query> StoreQuestionAnswer(Query query);
}