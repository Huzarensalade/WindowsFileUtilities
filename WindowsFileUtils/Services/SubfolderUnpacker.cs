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
        var rootDirectory = Path.GetFullPath(Console.ReadLine() ?? string.Empty);
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
    }

    private void CountFilesAndFolders(string directory)
    {
        try
        {
            _folderCount++;
            _fileCount += Directory.GetFiles(directory).Length;

            string[] subdirectories = Directory.GetDirectories(directory);
            foreach (string folder in subdirectories)
                CountFilesAndFolders(folder);
        }
        catch (UnauthorizedAccessException)
        {
            _errorCount++;
        }
    }
}