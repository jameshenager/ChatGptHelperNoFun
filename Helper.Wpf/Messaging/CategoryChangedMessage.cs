namespace Helper.Wpf.Messaging;

public class CategoryChangedMessage
{
    public int QueryId { get; init; }
    public int? NewCategoryId { get; init; }
}

public class CategoryUpdated;