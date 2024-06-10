using System.Collections.Generic;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;

namespace Helper.ServiceGateways.ActionItems;

public interface IActionItemService
{
    Task<BoolMessage> AddDependencyAsync(int actionItemActionItemId, int actionItemId);
    Task<List<ActionItem>> GetDependedOnByAsync(int actionItemActionItemId);
    Task<List<ActionItem>> GetPrerequisitesAsync(int actionItemActionItemId);
    Task<IEnumerable<ActionItem>> GetActionItemsAsync(ActionItemsSearchFilter filter);
}