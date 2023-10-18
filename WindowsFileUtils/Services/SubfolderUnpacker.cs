using System.Runtime.InteropServices.ComTypes;

namespace WindowsFileUtils.Services;

public class SubfolderUnpacker
{
    private int _fileCount = 0;
    private int _folderCount = 0;
    private int _errorCount = 0;

    public void Run()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Subfolder Unpacker Tool");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Please enter a directory to completely unpack all subfolders:");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        var rootDirectory = new DirectoryInfo(Path.GetFullPath(Console.ReadLine() ?? string.Empty));
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("Scanning Directories");
        CountFilesAndFolders(rootDirectory);
        Thread.Sleep(1000);
        var foundText = $"Found {_fileCount} files in {_folderCount} folders.";
        if (_errorCount > 0)
            foundText += $" {_errorCount} folders were inaccessible";
        Console.WriteLine(foundText);
        Console.WriteLine();
        Console.WriteLine("Press enter to continue.");
        Console.ReadLine();

        _errorCount = 0;
        _folderCount = 0;
        _fileCount = 0;

        Console.Clear();
        UnpackSubfoldersToRoot(rootDirectory, rootDirectory);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Moved {_fileCount} files to root directory.");
        if (_errorCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{_errorCount} files failed.");
        }
        Console.ResetColor();
        Console.WriteLine();
        Console.ReadLine();
    }

    private void CountFilesAndFolders(DirectoryInfo directory)
    {
        try
        {
            _folderCount++;
            _fileCount += directory.GetFiles().Length;

            var subdirectories = directory.GetDirectories();
            foreach (DirectoryInfo folder in subdirectories)
                CountFilesAndFolders(folder);
        }
        catch (Exception)
        {
            _errorCount++;
        }
    }

    private void UnpackSubfoldersToRoot(DirectoryInfo root, DirectoryInfo directory)
    {
        foreach (var file in directory.GetFiles())
        {
            try
            {
                Console.WriteLine($"{file.FullName}");
                file.MoveTo($@"{root.FullName}\{file.Name}");
                _fileCount++;
            }
            catch
            {
                _errorCount++;
            }
        }

        foreach (var folder in directory.GetDirectories())
        {
            try
            {
                UnpackSubfoldersToRoot(root, folder);
                _folderCount++;
                folder.Delete();
            }
            catch
            {
                // ignored
            }
        }
    }
}