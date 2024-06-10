using System.Collections.Generic;

namespace Helper.ServiceGateways;

public interface IPdfRenderer
{
    List<byte[]> GetPdfPageImage(string filePath, List<int> pages, float scale = 2.0f);
}
