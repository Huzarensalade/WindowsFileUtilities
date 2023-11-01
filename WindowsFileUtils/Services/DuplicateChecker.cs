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
        var foundText = $"Found {duplicates.Count} unique files in {_fileCount} checked files";
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

    private List<FileInfo> ScanDuplicates(DirectoryInfo folder)
    {
        var allFiles = folder.GetFiles();
        var uniqueList = new List<FileInfo>();

        foreach (var compareFile in allFiles)
        {
            _fileCount++;

            var sameSizeFiles = uniqueList.Where(x => x.Length == compareFile.Length);

            if (!sameSizeFiles.Any())
            {
                uniqueList.Add(compareFile);
                continue;
            }

            foreach (var sameSizeFile in sameSizeFiles)
            {
                try
                {
                    var checkFile = File.ReadAllBytes(sameSizeFile.FullName);
                    var originFile = File.ReadAllBytes(compareFile.FullName);

                    for (int i = 0; i < originFile.Length; i++)
                    {
                        if (checkFile[i] != originFile[i])
                        {
                            uniqueList.Add(compareFile);
                            goto FoundUniqueFileBreakpoint;
                        }
                    }
                }
                catch
                {
                    _errorCount++;
                }
            }

            FoundUniqueFileBreakpoint: ;
        }

        return uniqueList;
    }

    private void MoveDuplicates(List<FileInfo> uniqueFiles, DirectoryInfo root)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        var currentDate = DateTime.Now;
        var newFolder = Directory.CreateDirectory($@"{root.FullName}\Duplicates_{currentDate.Day}{currentDate.Month}{currentDate.Year}");
        var allFiles = root.GetFiles();
        foreach (var file in allFiles)
        {
            if (uniqueFiles.Any(x => x.FullName == file.FullName))
                continue;

            try
            {
                file.MoveTo($@"{newFolder.FullName}\{file.Name}");
                Console.WriteLine(file.FullName);
                _fileCount++;
            }
            catch
            {
                _errorCount++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(file.FullName);
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }
    }

    private void ResetCounters()
    {
        _errorCount = 0;
        _fileCount = 0;
    }
}