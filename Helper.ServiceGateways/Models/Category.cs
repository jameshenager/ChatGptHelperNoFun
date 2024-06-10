using System.Collections.Generic;

namespace Helper.ServiceGateways.Models;

public class Category
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public ICollection<Query> Queries { get; set; } = [];
}