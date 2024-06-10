using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Helper.ServiceGateways;

public interface ILogger
{
    Task LogError(string errorMessage);
    Task LogInfo(string message);
}

public class Logger : ILogger
{
    private static string ProgramName => "ChatGptHelper";

    public static string CreateDatabaseIfNeeded()
    {
        var backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramName);
        if (!Directory.Exists(backupFolder)) { Directory.CreateDirectory(backupFolder); }
        return backupFolder;
    }

    public async Task LogError(string errorMessage)
    {
        var logFilePath = Path.Combine(CreateDatabaseIfNeeded(), "ErrorLog.txt");
        try { await File.AppendAllTextAsync(logFilePath, $"{DateTime.Now}: {errorMessage}{Environment.NewLine}"); }
        catch (Exception ex) { Console.WriteLine($"Failed to log Error: {ex}"); }
    }

    public async Task LogInfo(string message)
    {
        var logFilePath = Path.Combine(CreateDatabaseIfNeeded(), "InfoLog.txt");
        try { await File.AppendAllTextAsync(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}"); }
        catch (Exception ex) { Console.WriteLine($"Failed to log Info: {ex}"); }
    }
}

public class ClassInfo
{
    public required string? ClassName { get; set; }
    public required string AccessModifier { get; set; }
    public List<MyMethodInfo> Methods { get; set; } = [];
}

public class MyMethodInfo
{
    public required string Name { get; set; }
    public string? Signature { get; set; }
    public string? AccessModifier { get; set; }
    public DocumentationInfo? Documentation { get; set; }
}

public class DocumentationInfo
{
    public required string? Summary { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];
    public required string Returns { get; set; }
}