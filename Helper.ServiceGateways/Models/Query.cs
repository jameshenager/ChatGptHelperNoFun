using System.Collections.Generic;

namespace Helper.ServiceGateways.Models;

public class Query
{
    public int QueryId { get; set; }
    public required string Text { get; set; }
    public int TokenCount { get; set; }
    public int? ResponseId { get; set; }
    public Response? Response { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<QueryActionItem> QueryActionItems { get; } = [];
}

public class QueryActionItem
{
    public int QueryId { get; set; }
    public required Query Query { get; set; }

    public int ActionItemId { get; set; }
    public required ActionItem ActionItem { get; set; }
}