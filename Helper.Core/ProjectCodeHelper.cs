using System.Text;
using System.Text.RegularExpressions;
// ReSharper disable StringLiteralTypo

namespace Helper.Core;

public interface IProjectCodeHelper
{
    List<FileWithContents> GetFileContents(List<string> desiredFiles);
    List<ClassRecord> GetClassNamesFromDirectory(string? directory);
}

public class ProjectCodeHelper : IProjectCodeHelper
{
    public string GetClassContents(List<ClassRecord> classFiles)
    {
        //not the right way to solve the problem. I should use dynamic parsing or something.
        // https://stackoverflow.com/questions/11643801/regex-c-sharp-how-to-match-methods-and-properties-names-of-a-class ?
        //I'll attempt to do this later, or never at all.
        var classContents = new Dictionary<string, string>();

        foreach (var classFile in classFiles)
        {
            var fileContent = File.ReadAllText(classFile.FilePath);
            var classPattern = $@"(public|private|protected|internal)?\s+partial\s+class\s+{classFile.ClassName}\b.*?{{
                                (?> 
                                    {{ (?<c>) 
                                    | }} (?<-c>) 
                                    | (?!{{|}}). 
                                )*
                                (?(c)(?!))
                                }}";

            var match = Regex.Match(fileContent, classPattern, RegexOptions.Singleline);

            if (match.Success)
            {
                if (classContents.ContainsKey(classFile.ClassName)) { classContents[classFile.ClassName] += match.Value;/* Concatenate if the class already exists (for partial classes) */			}
                else { classContents.Add(classFile.ClassName, match.Value); }
            }
            else { Console.WriteLine("regex failed"); }
        }

        return string.Join(Environment.NewLine, classContents.Values);
    }

    public List<FileWithContents> GetFileContents(List<string> desiredFiles)
    {
        var results = new List<FileWithContents>();
        foreach (var filePath in desiredFiles)
        {
            var fileContents = File.ReadAllText(filePath);
            var classes = GetClassNamesFromFileContent(fileContents); // If it's a .cs file, try to get all of the class names it contains
            var fileWithContents = new FileWithContents
            {
                Name = Path.GetFileName(filePath),
                FullPath = filePath,
                Contents = fileContents,
                Length = fileContents.Length,
                Selected = false,
                Classes = classes,
                Hash = ComputeSha256Hash(fileContents),
                TimeObserved = DateTime.Now,
            };
            results.Add(fileWithContents);
        }
        return results;
    }

    public List<ClassRecord> GetClassNamesFromDirectory(string? directory)
    {
        if (directory is null) { return []; }
        var classes = new List<ClassRecord>();
        var csFiles = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith("AssemblyInfo.cs") && !f.EndsWith(".g.cs") && !f.EndsWith(".i.cs") && !f.EndsWith(".xaml.cs") && !f.Contains($"{Path.DirectorySeparatorChar}Migrations{Path.DirectorySeparatorChar}"));

        foreach (var file in csFiles)
        {
            var fileContent = File.ReadAllText(file);
            var classNames = GetClassNamesFromFileContent(fileContent);
            foreach (var className in classNames) { classes.Add(new ClassRecord { ClassName = className, FilePath = file, }); }
        }
        return classes;
    }

    // ReSharper disable once UnusedMember.Global
    public List<ClassRecord> GetViewNamesFromDirectory(string directory)
    {
        //This class will be useful when I'm trying to get information from classes that don't compile.
        var classes = new List<ClassRecord>();
        // Search all *.xaml.cs files excluding App.xaml.cs
        var viewFiles = Directory
            .GetFiles(directory, "*.xaml.cs", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith("App.xaml.cs"));

        foreach (var file in viewFiles)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)); // Double call to remove both .xaml and .cs
            var fileContent = File.ReadAllText(file);
            // Assuming class names follow the convention of the file name
            // Might also be nice to get interfaces
            var classNamePattern = $@"\bclass\s+{fileNameWithoutExtension}\b";
            var matches = Regex.Matches(fileContent, classNamePattern);
            if (matches.Count > 0) { classes.Add(new ClassRecord { ClassName = fileNameWithoutExtension, FilePath = file, }); }
        }
        return classes;
    }

    public List<string> GetClassNamesFromFileContent(string fileContent)
    {
        var classes = new List<string>();
        var classPattern = @"\bclass\s+([^\s:{]+)"; // This regular expression looks for class declarations. It captures the class name in the first group. It handles basic class declarations but might not capture all possible C# class declaration nuances.
        var matches = Regex.Matches(fileContent, classPattern);
        foreach (Match match in matches) { if (match.Success) { classes.Add(match.Groups[1].Value); } }
        return classes;
    }

    // ReSharper disable once UnusedMember.Global
    public List<string> GetMatchingFiles(List<string> goodExtensions, string directoryToSearch)
    {
        //Needed for getting files in assemblies that don't compile
        var matchingFiles = new HashSet<string>();

        if (goodExtensions.Contains("App.xaml.cs"))
        {
            var appXamlCsFile = Directory.GetFiles(directoryToSearch, "App.xaml.cs", SearchOption.AllDirectories).FirstOrDefault();
            if (appXamlCsFile != null) { matchingFiles.Add(appXamlCsFile); }
        }

        foreach (var extension in goodExtensions)
        {
            var filterOutXamlCsFiles = (goodExtensions.Contains("*.cs") && !goodExtensions.Contains("*.xaml"));

            var files = Directory.GetFiles(directoryToSearch, extension, SearchOption.AllDirectories)
                .Where(f => !f.EndsWith("AssemblyInfo.cs") && !f.EndsWith(".g.cs") && !f.EndsWith(".i.cs"))
                .Where(f => !filterOutXamlCsFiles || !f.EndsWith(".xaml.cs"));

            foreach (var file in files)
            {
                if (file.Contains(@"\obj\Debug\") || file.Contains(@"\obj\Release\")) { continue; }
                matchingFiles.Add(file);
            }

            if (extension == "*.xaml")
            {
                var xamlCsFiles = Directory.GetFiles(directoryToSearch, "*.xaml.cs", SearchOption.AllDirectories)
                    .Where(file => !file.EndsWith("AssemblyInfo.cs") && !file.EndsWith(".g.cs"));
                foreach (var file in xamlCsFiles) { matchingFiles.Add(file); }
            }
        }
        return matchingFiles.ToList();
    }

    public string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (var t in bytes) { builder.Append(t.ToString("x2")); }
        return builder.ToString();
    }
}
public class ClassRecord
{
    // If I include the namespace, then it might be easier to deal with conflicts later on
    public required string ClassName { get; init; }
    public required string FilePath { get; init; } // for linking to FileWithContents
}

public record FileWithContents
{
    // It might also be nice to get the namespace
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public required string Contents { get; init; }
    public long Length { get; init; }
    public bool Selected { get; set; }
    public List<string> Classes { get; set; } = [];
    public required string Hash { get; set; }
    public DateTime TimeObserved { get; set; }
}