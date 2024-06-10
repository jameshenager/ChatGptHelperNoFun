using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Helper.Wpf.ActionItems;
public partial class ActionItemsFilterControl : UserControl
{
    public ActionItemsFilterControl() => InitializeComponent();

    public static readonly DependencyProperty LoadDataCommandProperty = DependencyProperty.Register(
        nameof(LoadDataCommand),
        typeof(ICommand),
        typeof(ActionItemsFilterControl),
        new PropertyMetadata(null));

    public ICommand LoadDataCommand
    {
        get => (ICommand)GetValue(LoadDataCommandProperty);
        set => SetValue(LoadDataCommandProperty, value);
    }

    public static readonly DependencyProperty ShowHighlightProperty = DependencyProperty.Register(
        nameof(ShowHighlight),
        typeof(bool),
        typeof(ActionItemsFilterControl),
        new PropertyMetadata(false));

    public bool ShowHighlight
    {
        get => (bool)GetValue(ShowHighlightProperty);
        set => SetValue(ShowHighlightProperty, value);
    }
}