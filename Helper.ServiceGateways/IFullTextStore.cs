using System.Collections.Generic;
using Helper.Core;

namespace Helper.ServiceGateways;

public interface IFullTextStore
{
    void StoreBook(string filePath, List<PdfPageText> pdfPageTexts);
    List<SearchResult> ReadStuff(string searchWords, int desiredResults);
    string GetBasePath();
    string GetFullTextStoreName();
}