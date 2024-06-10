using System.Security.Cryptography;
using System.Text;

namespace Helper.Core;

public class PdfPageText
{
    public required string Text { get; init; }
    public required string Hash { get; init; }

    public static string ComputeHash(string text)
    {
        using var sha256 = SHA256.Create();
        var textBytes = Encoding.UTF8.GetBytes(text);
        var hashBytes = SHA256.HashData(textBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}

public class SearchResult
{
    public required string Content { get; init; }
    public required string BookName { get; init; }
    public required int PageNumber { get; init; }
    public float Score { get; init; }
    public required string FilePath { get; init; }
}