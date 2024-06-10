using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helper.Wpf.General;

public interface IFileIo
{
    byte[] SerializeVector(List<float> vector);
    string[] GetPdfFilesInDirectory(string directory);
    double? GetDirectorySize(string directory);
    bool IsValidFileName(string proposedFileName);
    bool FileExists(string path);
    bool DirectoryExists(string path);
    string PathCombine(params string[] paths);
    string ReadAllText(string path);
    Task<string[]> ReadAllLines(string path);
    Task WriteAllTextAsync(string path, string text);
    string GetTempPath();
    string GetMyDocumentsPath();
    void CreateDirectory(string path);
    string[] GetFiles(string directory);
    string[] GetFiles(string directory, string searchPattern);
    void Delete(string path);
    void DeletePathsWithExtensions(string path, List<string> searchPatterns);
    void AppendAllText(string path, string text);
    void AppendLines(string path, string[] lines);
    string? CreateTempSubdirectory(string parentDirectory);
    void DeleteDirectory(string path);
}