using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Ui.Controls.CustomControls;

public class BindableMultiSelectDataGrid : DataGrid
{
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(BindableMultiSelectDataGrid), new PropertyMetadata(default(IList)));

    public new IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => throw new Exception("This property is read-only. To bind to it you must use 'Mode=OneWayToSource'.");
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        SetValue(SelectedItemsProperty, base.SelectedItems);
    }
}