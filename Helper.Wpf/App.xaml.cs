using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Helper.ServiceGateways;
using Helper.Wpf.General;
using Helper.Wpf.Image;
using Helper.Wpf.Text;
using Helper.Wpf.Main;
using Helper.Wpf.Transcribe;
using Helper.Web;
using Helper.Wpf.Settings;
using Helper.Wpf.Search;
using Helper.Wpf.ActionItems;
using Wpf.Ui.Controls.Notifications;
using Wpf.Ui.Common.Classes;
using Helper.ServiceGateways.ActionItems;
using Helper.ServiceGateways.Services;
using Helper.Core;
using Helper.Wpf.Clip;
using Helper.Wpf.Code;
using System.Linq;
using Helper.Wpf.WebMessage;
using IFileIo = Helper.Wpf.General.IFileIo;

namespace Helper.Wpf;

//ToDo: Implement https://github.com/lepoco/wpfui/

public partial class App
{
    public ServiceProvider Services { get; }

    public static App CurrentApplication => (App)Current;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        CreateJumpList();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        RegisterServices(services);
        RegisterViewModels(services);
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<ILogger, Logger>();
        services.AddSingleton<ChatGptCaller>();
        services.AddSingleton<WebMessagingCaller>();
        services.AddSingleton<GptContext>();
        services.AddSingleton<IProjectCodeHelper, ProjectCodeHelper>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IHttpService, HttpService>();
        services.AddSingleton<IImageSaver, ImageSaver>();
        services.AddSingleton<IFileIo, FileIo>();
        services.AddSingleton<IActionItemService, ActionItemService>();
        services.AddSingleton<IActionItemOverviewService, ActionItemOverviewService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ITextService, TextService>();
        services.AddSingleton<MemeTemplateService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddSingleton<CodeHelperViewModel>();
        services.AddSingleton<WebMessageViewModel>();
        services.AddSingleton<ActionItemsOverviewViewModel>();
        services.AddSingleton<NotificationManagerViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<TranscribeViewModel>();
        services.AddSingleton<TranscribeViewModel>();
        services.AddSingleton<ClipViewModel>();
        services.AddSingleton<TextViewModel>();
        services.AddSingleton<ImageViewModel>();
        services.AddSingleton<SearchViewModel>();
        services.AddSingleton<ClipView>();
        services.AddSingleton<DllInvestigatorViewModel>();
        services.AddSingleton<MemeGeneratorViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindowView>();
    }

    private static void CreateJumpList() { }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var wnd = Services.GetService<MainWindowView>();
        try
        {
            if (wnd is null) { return; }

            if (e.Args.Contains("/top")) { wnd.Topmost = true; }
            wnd.Show();
        }
        catch (System.Exception) { wnd!.Show(); }
    }
}