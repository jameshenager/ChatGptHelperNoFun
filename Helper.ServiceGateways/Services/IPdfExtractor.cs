using System.Collections.Generic;
using Helper.Core;

namespace Helper.ServiceGateways.Services;

public interface IPdfExtractor
{
    List<PdfPageText> ExtractTextFromPdf(string filePath);
}