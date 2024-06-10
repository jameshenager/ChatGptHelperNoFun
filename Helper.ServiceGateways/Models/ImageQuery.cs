using System.Collections.Generic;

namespace Helper.ServiceGateways.Models;

public class ImageQuery
{
    public int ImageQueryId { get; set; }
    public string ImageQueryText { get; set; } = null!; //ToDo: Address possible unlimited string lengths in database
    public ICollection<ImageResult> ImageResults { get; } = new List<ImageResult>();
}