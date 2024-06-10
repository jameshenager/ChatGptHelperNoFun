using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Helper.Wpf.General;

public class FileIo : IFileIo
{
    public byte[] SerializeVector(List<float> vector)
    {
        using var memoryStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            foreach (var value in vector) { binaryWriter.Write(value); }
        }

        var serializedVector = memoryStream.ToArray();
        return serializedVector;
    }

    public string[] GetPdfFilesInDirectory(string directory) => Directory.GetFiles(directory, "*.pdf");
    public double? GetDirectorySize(string directory) => Directory.Exists(directory) ? new DirectoryInfo(directory).GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length) / (1024d * 1024d) : null;

    public bool FileExists(string path) => File.Exists(path);
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public bool IsValidFileName(string proposedFileName) => proposedFileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);
    public string PathCombine(string path1, string path2) => Path.Combine(path1, path2);
    public string PathCombine(params string[] paths) => Path.Combine(paths);

    public string ReadAllText(string path) => File.ReadAllText(path);
    public Task<string[]> ReadAllLines(string path) => File.ReadAllLinesAsync(path);
    public async Task WriteAllTextAsync(string path, string text) => await File.WriteAllTextAsync(path, text);
    public string GetTempPath() => Path.GetTempPath();
    public string GetMyDocumentsPath() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public string[] GetFiles(string directory) => Directory.GetFiles(directory);
    public string[] GetFiles(string directory, string searchPattern) => Directory.GetFiles(directory, searchPattern);
    public void Delete(string path) => File.Delete(path);
    public void DeleteDirectory(string path) => Directory.Delete(path, true);
    public void DeletePathsWithExtensions(string path, List<string> searchPatterns)
    {
        foreach (var pattern in searchPatterns)
        {
            foreach (var file in Directory.GetFiles(path, pattern)) { File.Delete(file); }
        }
    }
    public void AppendAllText(string path, string text) => File.AppendAllText(path, text);
    public void AppendLines(string path, string[] lines) => File.AppendAllLines(path, lines);
    public string? CreateTempSubdirectory(string parentDirectory)
    {
        if (!Directory.Exists(parentDirectory)) { return null; }
        //create random 5 character string
        var randomString = GetRandomString(5);
        while (Directory.Exists(Path.Combine(parentDirectory, randomString))) { randomString = GetRandomString(5); }

        var tempDirectory = Path.Combine(parentDirectory, randomString);
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    private string GetRandomString(int i)
    {
        const string chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var randomString = new string(Enumerable.Repeat(chars, i).Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;

    }
}