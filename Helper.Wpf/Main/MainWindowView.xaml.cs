namespace Helper.Wpf.Main;

public partial class MainWindowView 
{
    public MainWindowView(MainWindowViewModel mainWindowViewModel)
    {
        DataContext = mainWindowViewModel;
        InitializeComponent();
    }
}