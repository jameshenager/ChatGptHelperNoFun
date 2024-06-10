using System.Collections.Generic;
using System.Threading.Tasks;
using Helper.Core;
using Helper.ServiceGateways.Models;

namespace Helper.ServiceGateways.Services;

public interface IEmbedService
{
    bool HasApiKey(string keyType);
    void SetApiKey(string newApiKey, string keyType);
    string GetApiKey(string keyType);
    Task<List<EmbedPiece>> GetSomeEmbeds(int skip = 0, int takeLimit = 100);
    int GetEmbedCount();
    List<EmbedPiece> GetBestEmbeds(float[] vector, int desiredCount = 100);
    Task StoreEmbed(EmbedResult e);
}