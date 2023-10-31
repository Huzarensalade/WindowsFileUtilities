using System;

namespace WindowsFileUtils.Services;

public class DuplicateChecker
{
    private int _fileCount = 0;
    private int _errorCount = 0;

    public void Run()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Duplicate File Detector Tool");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Please enter a directory to check for duplicates:");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        var selectedFolder = new DirectoryInfo(Path.GetFullPath(Console.ReadLine() ?? string.Empty));
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("Scanning for duplicates");
        var duplicates = ScanDuplicates(selectedFolder);
        var foundText = duplicates.Count > 0 ? $"Found {duplicates.Count} duplicates." : $"No duplicates where found";
        Console.WriteLine(foundText);
        if (_errorCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{_errorCount} errors encountered.");
        }
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Press enter to continue.");
        Console.ReadLine();

        ResetCounters();

        Console.Clear();
        MoveDuplicates(duplicates, selectedFolder);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Moved {_fileCount} duplicates to separate folder");
        if (_errorCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{_errorCount} files failed.");
        }
        Console.ResetColor();
        Console.WriteLine();
        Console.ReadLine();
    }

    private List<string> ScanDuplicates(DirectoryInfo folder)
    {
        var allFiles = folder.GetFiles();
        var duplicateList = new List<string>();
        foreach (var compareFile in allFiles)
        {
            var sameSizeFiles = folder.GetFiles().Where(x => x.Name != compareFile.Name && x.Length == compareFile.Length);
            if (!sameSizeFiles.Any())
                return duplicateList;

            foreach (var sameSizeFile in sameSizeFiles)
            {
                try
                {
                    var checkFile = File.ReadAllBytes(sameSizeFile.FullName);
                    var originFile = File.ReadAllBytes(compareFile.FullName);
                    var foundDiffChar = false;

                    for (int i = 0; i < originFile.Length; i++)
                    {
                        if (checkFile[i] != originFile[i])
                        {
                            foundDiffChar = true;
                            i = originFile.Length;
                        }
                    }

                    if (!foundDiffChar)
                        if (!duplicateList.Any(x => x == sameSizeFile.FullName))
                            duplicateList.Add(sameSizeFile.FullName);
                }
                catch
                {
                    _errorCount++;
                }
            }
        }

        return duplicateList;
    }

    private void MoveDuplicates(List<string> duplicateFilePaths, DirectoryInfo root)
    {
        var newFolder = Directory.CreateDirectory($@"{root.FullName}\{Guid.NewGuid()}");
        foreach (var filePath in duplicateFilePaths)
        {
            var file = new FileInfo(filePath);
            
            try
            {
                file.MoveTo($@"{newFolder.FullName}\{file.Name}");
                _fileCount++;
            }
            catch
            {
                _errorCount++;
            }
        }
    }

    private void ResetCounters()
    {
        _errorCount = 0;
        _fileCount = 0;
    }
}