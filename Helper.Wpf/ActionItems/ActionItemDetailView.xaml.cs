using System.Windows;

namespace Helper.Wpf.ActionItems;
public partial class ActionItemDetailView : Window
{
    public ActionItemDetailView(ActionItemDetailViewModel actionItemDetailViewModel)
    {
        DataContext = actionItemDetailViewModel;
        InitializeComponent();
    }
}