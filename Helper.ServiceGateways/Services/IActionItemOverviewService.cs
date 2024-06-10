using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Helper.ServiceGateways.Services;

public interface IActionItemOverviewService
{
    Task<IEnumerable<ActionItem>> GetActionItemsAsync(ActionItemsSearchFilter filter);
    EntityEntry<ActionItem> AddActionItem(ActionItem actionItem);
    void UpdateActionItem(ActionItem actionItem);
    Task<List<ActionItem>> GetTasksForMonthAsync(DateTime currentMonth);
    void DeleteActionItem(ActionItem actionItem);
}