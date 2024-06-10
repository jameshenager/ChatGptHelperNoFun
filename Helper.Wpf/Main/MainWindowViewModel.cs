using CommunityToolkit.Mvvm.ComponentModel;
using Helper.Wpf.ActionItems;
using Helper.Wpf.Clip;
using Helper.Wpf.Code;
using Helper.Wpf.Image;
using Helper.Wpf.Search;
using Helper.Wpf.Settings;
using Helper.Wpf.Text;
using Helper.Wpf.Transcribe;
using Helper.Wpf.WebMessage;
using Wpf.Ui.Controls.Notifications;

namespace Helper.Wpf.Main;

public partial class MainWindowViewModel(TextViewModel tvm, ImageViewModel ivm, TranscribeViewModel transcribeVm, SettingsViewModel settingsVm, SearchViewModel searchViewModel, NotificationManagerViewModel notificationManagerViewModel, ActionItemsOverviewViewModel taskOverviewViewModel, CodeHelperViewModel codeHelperViewModel, DllInvestigatorViewModel dllInvestigatorViewModel, ClipViewModel clipViewModel, MemeGeneratorViewModel memeGeneratorViewModel, WebMessageViewModel webMessageViewModel) : ObservableObject
{
    [ObservableProperty] private TextViewModel _tvm = tvm;
    [ObservableProperty] private ImageViewModel _ivm = ivm;
    [ObservableProperty] private TranscribeViewModel _transcribeVm = transcribeVm;
    [ObservableProperty] private SettingsViewModel _settingsVm = settingsVm;
    [ObservableProperty] private SearchViewModel _searchViewModel = searchViewModel;
    [ObservableProperty] private ActionItemsOverviewViewModel _taskOverviewViewModel = taskOverviewViewModel;
    [ObservableProperty] private CodeHelperViewModel _codeHelperViewModel = codeHelperViewModel;
    [ObservableProperty] private DllInvestigatorViewModel _dllInvestigatorViewModel = dllInvestigatorViewModel;
    [ObservableProperty] private ClipViewModel _clipViewModel = clipViewModel;
    [ObservableProperty] private MemeGeneratorViewModel _memeGeneratorViewModel = memeGeneratorViewModel;
    public NotificationManagerViewModel NotificationManagerViewModel { get; } = notificationManagerViewModel;
    public WebMessageViewModel WebMessageViewModel { get; } = webMessageViewModel;
}