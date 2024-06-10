using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Helper.Core.GptModels;
using Helper.ServiceGateways.Models;
using Helper.ServiceGateways.Services;
using Wpf.Ui.Common.Classes;

namespace Helper.Wpf.Text;

public partial class TokenStatisticsViewModel(ITextService textDatabaseService) : ObservableObject
{
    public ObservableCollectionEx<CategoryCostDisplay> Statistics { get; set; } = [];

    [RelayCommand]
    public async Task GetStatistics()
    {
        var statistics = await textDatabaseService.GetStatisticsByCategory();
        var categoryDictionary = statistics.Select(stat => stat.Category).Distinct().Where(s => s is not null).Cast<string>().ToDictionary(category => category, category => new CategoryCostDisplay() { Category = category, });

        foreach (var stat in statistics)
        {
            var currentDictionary = categoryDictionary[stat.Category!];
            var model = GptModel.StaticModels.FirstOrDefault(m => m.Name == stat.ModelName);
            if (model is null) { continue; }
            var exact = (model.PricePerMillionOutputTokens * stat.OutputTokenCount + model.PricePerMillionInputTokens * stat.InputTokenCount) / 1_000_000m;
            currentDictionary.Cost += Math.Ceiling(exact * 100) / 100;
            currentDictionary.InputTokenCount += stat.InputTokenCount > 0 ? stat.InputTokenCount : 0;
            currentDictionary.OutputTokenCount += stat.OutputTokenCount > 0 ? stat.OutputTokenCount : 0;
        }
        Statistics.ReplaceWith(categoryDictionary.Values);
    }
}