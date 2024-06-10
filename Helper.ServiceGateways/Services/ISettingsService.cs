using System.Collections.Generic;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;

namespace Helper.ServiceGateways.Services;

public interface ISettingsService
{
    string GetDatabaseFolder();
    Task<List<Category>> GetCategories();
    Task<bool> AddCategory(Category newCategory);
    Task SetFfmpegLocation(string fileLocation);
    Task SetMessagingMasterCode(string masterCode);
    Task SetMessagingUrl(string url);
    Task<string> GetFfmpegLocation();
    Task<string> GetMessagingUrl();
    Task<string> GetMessagingMasterCode();
}