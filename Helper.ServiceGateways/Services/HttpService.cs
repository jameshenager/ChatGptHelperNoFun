using System.Net.Http;
using System.Threading.Tasks;

namespace Helper.ServiceGateways.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient = new();
    public Task<byte[]> GetByteArrayAsync(string url) => _httpClient.GetByteArrayAsync(url);
}