using System.Threading.Tasks;

namespace Helper.ServiceGateways.Services;

public interface IHttpService
{
    Task<byte[]> GetByteArrayAsync(string url);
}