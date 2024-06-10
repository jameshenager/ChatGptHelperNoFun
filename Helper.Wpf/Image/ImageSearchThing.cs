using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Helper.ServiceGateways.Models;

namespace Helper.Wpf.Image;

public class ImageSearchThing
{
    public string QueryText { get; private init; } = null!;
    public List<BitmapImage> Images { get; set; } = [];

    public static ImageSearchThing Get(ImageQuery iq)
    {
        var imageSearchThing = new ImageSearchThing() { QueryText = iq.ImageQueryText, };
        foreach (var imageData in iq.ImageResults)
        {
            if (imageData.ImageBlob == null) continue;
            using var ms = new MemoryStream(imageData.ImageBlob);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze(); // Important for use in a multithreaded environment
            imageSearchThing.Images.Add(image);
        }

        return imageSearchThing;
    }
}