namespace Wpf.Ui.Common.Classes;

public class UniqueObservableCollection<T> : ObservableCollectionEx<T>
{
    private readonly HashSet<T> _itemsSet = [];

    protected override void InsertItem(int index, T item)
    {
        if (_itemsSet.Add(item)) { base.InsertItem(index, item); }
    }

    protected override void ClearItems()
    {
        _itemsSet.Clear();
        base.ClearItems();
    }

    protected override void RemoveItem(int index)
    {
        _itemsSet.Remove(this[index]);
        base.RemoveItem(index);
    }

    public new void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            if (_itemsSet.Add(item)) { base.InsertItem(Count, item); }
        }
    }

}