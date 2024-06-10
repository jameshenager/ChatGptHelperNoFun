using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Helper.Core;
using Helper.ServiceGateways;

namespace Helper.Wpf.Search;

public partial class PdfPageViewerWindow
{
    public PdfPageViewerWindow(PdfPageViewerWindowViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
    }
}

public partial class PdfPageViewerWindowViewModel : ObservableObject
{
    private readonly IPdfRenderer _pdfRenderer;
    private readonly SearchResult _selectedSearchResult;
    public ObservableCollection<BitmapImage> PdfImages { get; } = [];
    public PdfPageViewerWindowViewModel(SearchResult selectedSearchResult, IPdfRenderer pdfRenderer)
    {
        _pdfRenderer = pdfRenderer;
        _selectedSearchResult = selectedSearchResult;
        LoadPdfPageImages();
    }

    private void LoadPdfPageImages()
    {
        var images = _pdfRenderer.GetPdfPageImage(_selectedSearchResult.FilePath, [
            _selectedSearchResult.PageNumber - 1,
            _selectedSearchResult.PageNumber,
            _selectedSearchResult.PageNumber + 1,
        ]);

        foreach (var image in images)
        {
            var thing = ByteArrayToBitmapImage(image);
            PdfImages.Add(thing);
        }
    }

    private BitmapImage ByteArrayToBitmapImage(byte[] imageData)
    {
        var bitmapImage = new BitmapImage();
        using var memStream = new MemoryStream(imageData);
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = memStream;
        bitmapImage.EndInit();
        bitmapImage.Freeze(); // Freeze the image for use in the UI thread
        return bitmapImage;
    }
}
