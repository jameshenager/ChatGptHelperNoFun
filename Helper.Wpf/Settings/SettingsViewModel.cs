using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Helper.ServiceGateways.Models;
using Helper.Wpf.General;
using Helper.Wpf.Messaging;
using Wpf.Ui.Controls.Notifications;
using Helper.ServiceGateways.Services;
using Wpf.Ui.Common.Classes;

namespace Helper.Wpf.Settings;

public partial class SettingsViewModel(ISettingsService sg, IDialogService dialogService, IFileIo fileIo, INotificationService notificationService) : ObservableObject
{
    public ObservableCollection<Category> Categories { get; set; } = [];
    [ObservableProperty] private string _newCategoryName = "";
    [ObservableProperty] private Category? _selectedCategory;
    [ObservableProperty] private double? _databaseFolderSize;
    [ObservableProperty] private double? _fullTextFolderSize;
    [ObservableProperty] private string _fullTextStoreName = "";
    [ObservableProperty] private string _ffmpegLocation = "";
    [ObservableProperty] private string _messagingUrl = "";
    [ObservableProperty] private string _messagingMasterCode = "";

    [RelayCommand]
    private void GetDatabaseFolderSize()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            DatabaseFolderSize = fileIo.GetDirectorySize(sg.GetDatabaseFolder());
        }
        catch {/* ignored */}

        notificationService.RaiseNotification(new TransientNotification($"Database folder size calculated in {sw.ElapsedMilliseconds} ms.", TimeSpan.FromSeconds(3)));
    }

    partial void OnMessagingUrlChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) { return; }
        sg.SetMessagingUrl(value);
    }
    partial void OnMessagingMasterCodeChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) { return; }
        sg.SetMessagingMasterCode(value);
    }

    partial void OnFfmpegLocationChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) { return; }
        sg.SetFfmpegLocation(value);
    }

    [RelayCommand]
    private async Task UserControlLoaded()
    {
        Categories.Clear();
        NewCategoryName = "";
        var categories = await sg.GetCategories();
        foreach (var category in categories) { Categories.Add(category); }
        FfmpegLocation = await sg.GetFfmpegLocation();
        MessagingUrl = await sg.GetMessagingUrl();
        MessagingMasterCode = await sg.GetMessagingMasterCode();
        GetDatabaseFolderSize();
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
            dialogService.ShowMessageBox("Category name cannot be empty.");
            return;
        }

        //make sure the category name is unique
        if (Categories.Any(c => string.Equals(c.Name, NewCategoryName, StringComparison.OrdinalIgnoreCase)))
        {
            dialogService.ShowMessageBox("Category name already exists.");
            return;
        }

        var newCategory = new Category { Name = NewCategoryName, };
        var added = await sg.AddCategory(newCategory);
        if (added)
        {
            Categories.Add(newCategory);
            NewCategoryName = string.Empty;
            WeakReferenceMessenger.Default.Send(new CategoryUpdated());
        }
        else { dialogService.ShowMessageBox("Failed to add new category."); }
    }
}