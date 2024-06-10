using System.Collections.Generic;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;

namespace Helper.ServiceGateways.Services;

public interface IImageService
{
    bool HasApiKey(string keyType);
    void SetApiKey(string newApiKey, string keyType);
    string GetApiKey(string keyType);
    Task<List<ImageQuery>> GetImageQueries(int skip = 0, int takeLimit = 100);
    Task<ImageQuery> StoreStuff(ImageQuery iq);
}