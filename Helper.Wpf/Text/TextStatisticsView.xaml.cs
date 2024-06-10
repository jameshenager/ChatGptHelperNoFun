namespace Helper.Wpf.Text;
public partial class TextStatisticsView
{
    public TextStatisticsView(TokenStatisticsViewModel tokenStatisticsViewModel)
    {
        DataContext = tokenStatisticsViewModel;
        InitializeComponent();
    }
}
