using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Helper.Core.GptModels;
using Helper.ServiceGateways;
using Helper.ServiceGateways.Models;
using Helper.ServiceGateways.Services;
using Helper.Web;
using Helper.Wpf.General;
using Helper.Wpf.Messaging;
using Wpf.Ui.Common.Classes;
using Wpf.Ui.Controls.Notifications;

namespace Helper.Wpf.Text;

/*
    ToDo: I could add a ToDo list or similar to the UI
    Maybe I could do a calendar view or something so I can see what's coming up. 
    Then I could also link together things, so some topological sort of the tasks for efficiency.
    I could also try to link together tasks and questions, so I can see what I need to do to answer a question.
        This would also help with general categorization of questions.
    It could also double as a bug tracker.
 */
//ToDo: Let's create buttons to back everything up.
//ToDo: I need to make the categories hierarchical.
//ToDo: I could add a screen where I could test out notifications. Also view logs and stuff.

public partial class TextViewModel : ObservableRecipient //ToDo: I need to adjust this to also record the dateTime when it was requested, so I can use it later for statistics and filtering.
{
    [ObservableProperty] private string _currentQuery = string.Empty;
    [ObservableProperty] private int _currentQueryTokenCount;
    [ObservableProperty] private int _currentResponseTokenCount;
    [ObservableProperty] private string _currentResponse = string.Empty;
    [ObservableProperty] private GptModel _selectedModel = null!;
    [ObservableProperty] private bool _isDeterministic = true;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _itemsPerPage = 100;
    [ObservableProperty] private bool _hasMoreItems = true;
    [ObservableProperty] private string _querySearch = string.Empty;
    [ObservableProperty] private string _responseSearch = string.Empty;
    [ObservableProperty] private bool _allowQuerying = true;

    private readonly ITextService _textDatabaseService;
    private readonly IDialogService _dialogService;
    private readonly ChatGptCaller _chatGptCaller;
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;
    private string? _apiKey;
    private readonly List<FullTextFilter> _fullTextFilters = [];
    public ObservableCollection<Category> Categories { get; set; } = [];
    public ObservableCollection<CategoryThing> CategoryCheckboxes { get; set; } = [];
    public ObservableCollectionEx<QuestionAnswerViewModel> QuestionAnswers { get; set; } = [];
    public ObservableCollection<GptModel> Models { get; set; } = new(StaticModels);
    public static List<GptModel> StaticModels = GptModel.StaticModels;

    public TextViewModel(ITextService textDatabaseService, IDialogService dialogService, ChatGptCaller chatGptCaller, ILogger logger, INotificationService notificationService)
    {
        _textDatabaseService = textDatabaseService;
        _dialogService = dialogService;
        _chatGptCaller = chatGptCaller;
        _logger = logger;
        _notificationService = notificationService;

        WeakReferenceMessenger.Default.Register<CategoryUpdated>(this, (_, _) => ReceiveCategoryUpdated());
        WeakReferenceMessenger.Default.Register<CategoryChangedMessage>(this, (_, m) => ReceiveCategoryChangedMessage(m));
    }

    [RelayCommand]
    public void OpenStatsWindow()
    {
        var vm = new TokenStatisticsViewModel(_textDatabaseService);
        var window = new TextStatisticsView(vm);
        window.Show();
    }

    private void ReceiveCategoryChangedMessage(CategoryChangedMessage message) => _textDatabaseService.UpdateCategoryForQuestionAnswer(message.QueryId, message.NewCategoryId);
    private async void ReceiveCategoryUpdated() { await LoadCategories(); }

    [RelayCommand]
    private async Task FullTextSearch()
    {
        var querySearch = string.IsNullOrEmpty(QuerySearch) ? null : QuerySearch;
        var responseSearch = string.IsNullOrEmpty(ResponseSearch) ? null : ResponseSearch;
        if (querySearch is not null || responseSearch is not null) { _fullTextFilters.Add(new FullTextFilter { QueryText = querySearch, ResponseText = responseSearch, }); }

        var x = await _textDatabaseService.GetQueriesFilteredFulltext(_fullTextFilters);
        UpdateQuestionAnswersCollection(x);
        QuerySearch = string.Empty;
        ResponseSearch = string.Empty;
    }

    [RelayCommand] private void ClearFullTextFilter() => _fullTextFilters.Clear();

    private async Task LoadQuestionAnswers(bool isFiltered = false)
    {
        var sw = Stopwatch.StartNew();
        if (isFiltered)
        {
            var selectedCategoryIds = CategoryCheckboxes.Where(c => c.IsSelected).Select(c => c.CategoryId).ToHashSet();
            var stuffs = await _textDatabaseService.GetQueriesFiltered(selectedCategoryIds, (CurrentPage - 1) * ItemsPerPage, ItemsPerPage);
            UpdateQuestionAnswersCollection(stuffs);
        }
        else
        {
            var selectedCategoryIds = CategoryCheckboxes.Where(c => c.IsSelected).Select(c => c.CategoryId).ToHashSet();
            var stuffs = await _textDatabaseService.GetQueriesFiltered(selectedCategoryIds, (CurrentPage - 1) * ItemsPerPage, ItemsPerPage);
            UpdateQuestionAnswersCollection(stuffs);
        }
        _notificationService.RaiseNotification(new TransientNotification($"Loaded {QuestionAnswers.Count} questions in {sw.ElapsedMilliseconds}ms", TimeSpan.FromSeconds(5)));
    }

    private void UpdateQuestionAnswersCollection(IEnumerable<Query> queries)
    {
        var questionAnswers = queries.Select(q => new QuestionAnswerViewModel(q)).ToList();
        Application.Current.Dispatcher.Invoke(() => { QuestionAnswers.ReplaceWith(questionAnswers); HasMoreItems = questionAnswers.Count == ItemsPerPage; /*This is a bug.*/ });
    }

    [RelayCommand]
    private async Task NextPage()
    {
        if (!HasMoreItems) { return; }
        CurrentPage++;
        await LoadQuestionAnswers(true);
    }

    [RelayCommand]
    private async Task PreviousPage()
    {
        if (CurrentPage <= 1) { return; }
        CurrentPage--;
        await LoadQuestionAnswers(true);
    }

    [RelayCommand]
    private async Task FilterCategories()
    {
        var selectedCategoryIds = CategoryCheckboxes.Where(c => c.IsSelected).Select(c => c.CategoryId).ToHashSet();

        await Task.Run(async () =>
        {
            var stuffs = await _textDatabaseService.GetQueriesFiltered(selectedCategoryIds);
            var questionAnswers = new List<QuestionAnswerViewModel>();
            foreach (var q in stuffs) { questionAnswers.Add(new QuestionAnswerViewModel(q)); }
            Application.Current.Dispatcher.Invoke(() => { QuestionAnswers.ReplaceWith(questionAnswers); });
        });
    }

    [RelayCommand]
    private async Task UserControlLoaded()
    {
        SelectedModel = Models.Last();
        _apiKey ??= GetApiKey();
        if (QuestionAnswers.Any()) { return; }

        var sw = Stopwatch.StartNew();
        await LoadCategories();
        await Task.Run(async () =>
        {
            var filteredCategories = CategoryCheckboxes.Where(c => c.IsSelected).Select(c => c.CategoryId).ToHashSet();
            var stuffs = await _textDatabaseService.GetQueriesFiltered(filteredCategories);
            var questionAnswers = new List<QuestionAnswerViewModel>();
            foreach (var q in stuffs) { questionAnswers.Add(new QuestionAnswerViewModel(q)); }
            Application.Current.Dispatcher.Invoke(() => { QuestionAnswers.ReplaceWith(questionAnswers); });
        });
        _notificationService.RaiseNotification(new TransientNotification($"Loaded {QuestionAnswers.Count} questions in {sw.ElapsedMilliseconds}ms", TimeSpan.FromSeconds(5)));
    }

    private async Task LoadCategories()
    {
        Categories.Clear();
        await Task.Run(async () =>
        {
            var categoriesFromDb = await _textDatabaseService.GetCategories();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var category in categoriesFromDb) { CategoryCheckboxes.Add(new CategoryThing() { Name = category.Name, CategoryId = category.CategoryId, IsSelected = category.Name != "Misc", }); }
                UpdateCategoriesCollection(categoriesFromDb);
            });
        });
    }

    private void UpdateCategoriesCollection(IList<Category> newCategories)
    {
        for (var i = Categories.Count - 1; i >= 0; i--)
        {
            //ToDo: This is an O(n) operation. I should use the new UniqueObservableCollection class instead. That class will probably need to change though since I don't accept a delegate for the equality comparison.
            if (newCategories.All(nc => nc.CategoryId != Categories[i].CategoryId)) { Categories.RemoveAt(i); }
        }
        foreach (var newCategory in newCategories)
        {
            var existingCategory = Categories.FirstOrDefault(c => c.CategoryId == newCategory.CategoryId);
            if (existingCategory != null)
            {
                if (existingCategory.Name != newCategory.Name) { existingCategory.Name = newCategory.Name; }
            }
            else { Categories.Add(newCategory); }
        }
    }

    [RelayCommand] private async Task GetTokenCount() { if (_apiKey != null) { CurrentQueryTokenCount = await _chatGptCaller.GetEmbedCost(_apiKey, CurrentQuery); } }

    [RelayCommand(CanExecute = nameof(AllowQuerying))]
    private async Task Query()
    {
        AllowQuerying = false;
        CurrentResponse = string.Empty;

        try { if (SelectedModel.IsChat) { await ChatQuery(); } else { await OrdinaryQuery(); } }
        catch (Exception ex) { await _logger.LogError(ex.Message); }
        AllowQuerying = true;
    }

    private async Task OrdinaryQuery()
    {
        CurrentResponse = string.Empty;
        if (_apiKey == null) { return; }
        var response = await _chatGptCaller.Query(_apiKey, CurrentQuery, SelectedModel, IsDeterministic);
        if (!response.Success) { _dialogService.ShowMessageBox($"an Error occurred! {response.Message ?? string.Empty}"); }
        else
        {
            CurrentResponseTokenCount = response.Result!.Usage.completion_tokens;
            CurrentQueryTokenCount = response.Result!.Usage.prompt_tokens;
            CurrentResponse = string.Join(Environment.NewLine, response.Result!.Choices.Select(c => c.Text)).Trim();

            await Task.Run(async () =>
            {
                var newQ = await _textDatabaseService.StoreQuery(CurrentQuery, CurrentQueryTokenCount, response.Result!, SelectedModel.Name ?? "unknown");
                Application.Current.Dispatcher.Invoke(() => { QuestionAnswers.Insert(0, new QuestionAnswerViewModel(newQ)); });
            });
        }
    }

    [RelayCommand] private void ImportChatGptQueries() => _dialogService.ShowMessageBox("This feature is not available in this version.");

    private async Task ChatQuery()
    {
        CurrentResponse = string.Empty;
        if (_apiKey == null) { return; }
        var backup = new string(CurrentQuery.ToCharArray());
        var response = await _chatGptCaller.ChatQuery(_apiKey, backup, SelectedModel, IsDeterministic);

        if (!response.Success) { _dialogService.ShowMessageBox($"an Error occurred! {response.Message ?? string.Empty}"); }
        else
        {
            CurrentResponseTokenCount = response.Result!.usage.completion_tokens;
            CurrentQueryTokenCount = response.Result!.usage.prompt_tokens;
            CurrentResponse = string.Join(Environment.NewLine, response.Result!.choices?.Select(c => c.Message?.content) ?? []).Trim();
            await Task.Run(async () =>
            {
                var newQ = await _textDatabaseService.StoreChatQuery(backup, CurrentQueryTokenCount, response.Result!, SelectedModel.Name ?? string.Empty);
                Application.Current.Dispatcher.Invoke(() => { QuestionAnswers.Insert(0, new QuestionAnswerViewModel(newQ)); });
            });
        }
    }

    private string GetApiKey()
    {
        if (_textDatabaseService.HasApiKey("OpenAi")) { return _textDatabaseService.GetApiKey("OpenAi"); }
        var apiKey = _dialogService.GetApiKey();
        if (apiKey is null) { Application.Current.Shutdown(); return string.Empty; }
        //ToDo: I should check if the API key is valid here.
        _textDatabaseService.SetApiKey(apiKey, "OpenAi");
        return apiKey;
    }

    [RelayCommand]
    private static void CopyRowToClipboard(object parameter)
    {
        if (parameter is not QuestionAnswerViewModel selectedItem) { return; }
        var clipboardText = $"Prompt: {Environment.NewLine}{selectedItem.Prompt}{Environment.NewLine}Model: {selectedItem.ModelUsed}{Environment.NewLine}Answer: {Environment.NewLine}{selectedItem.Answer}";
        Clipboard.SetText(clipboardText);
    }

    [RelayCommand] private static void ToggleRowExpansion(QuestionAnswerViewModel questionAnswerViewModel) => questionAnswerViewModel.ToggleExpansion();
    [RelayCommand] private void Stuff() => _dialogService.ShowMessageBox($"Hi! {_textDatabaseService.AnswerCount()}");

    [RelayCommand]
    private static void OpenTextDetailWindow(QuestionAnswerViewModel questionAnswerViewModel)
    {
        var vm = new TextDetailWindowViewModel(questionAnswerViewModel);
        var detailWindow = new TextDetailWindow(vm);
        detailWindow.Show();
    }
}