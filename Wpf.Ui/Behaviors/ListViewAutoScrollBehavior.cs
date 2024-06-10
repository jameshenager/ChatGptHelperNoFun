using System.Collections.Specialized;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Wpf.Ui.Common.Behaviors;

public class ListViewAutoScrollBehavior : Behavior<ListView>
{
    protected override void OnAttached()
    {
        ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnCollectionChanged;
    }

    protected override void OnDetaching()
    {
        ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnCollectionChanged;
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var items = AssociatedObject.Items;
            if (items.Count > 0)
            {
                var lastItem = items[items.Count - 1];
                AssociatedObject.ScrollIntoView(lastItem);
            }
        }
    }
}