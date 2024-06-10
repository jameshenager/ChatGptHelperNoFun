namespace Helper.ServiceGateways.Models;

public class ImageResult
{
    public int ImageResultId { get; set; }
    public ImageQuery ImageQuery { get; set; } = null!;
    public byte[]? ImageBlob { get; set; }
}