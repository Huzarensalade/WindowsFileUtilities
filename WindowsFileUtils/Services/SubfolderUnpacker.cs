namespace WindowsFileUtils.Services;

public class SubfolderUnpacker
{
    private int _fileCount = 0;
    private int _folderCount = 0;
    private int _errorCount = 0;
    private int _renameCount = 0;

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

        Console.WriteLine("Scanning directories");
        CountFilesAndFolders(rootDirectory);
        var foundText = $"Found {_fileCount} files in {_folderCount} folders.";
        if (_errorCount > 0)
            foundText += $" {_errorCount} folders were inaccessible";
        Console.WriteLine(foundText);
        Console.WriteLine();
        Console.WriteLine("Press enter to continue.");
        Console.ReadLine();

        ResetCounters();

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
        if (_renameCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{_renameCount} duplicate files renamed.");
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
        Console.ForegroundColor = ConsoleColor.Green;

        foreach (var file in directory.GetFiles())
        {
            if (file.DirectoryName == root.FullName)
                continue;

            try
            {
                var foundDuplicate = true;
                var newFileName = file.Name;
                var incrementalSuffix = 0;

                while (foundDuplicate)
                {
                    if (root.GetFiles().Any(x => x.Name == newFileName))
                    {
                        incrementalSuffix++;
                        newFileName = $"{file.Name.Replace(file.Extension, string.Empty)}_{incrementalSuffix}{file.Extension}";
                    }
                    else
                    {
                        foundDuplicate = false;
                    }
                }

                if (newFileName != root.Name)
                    _renameCount++;

                file.MoveTo($@"{root.FullName}\{newFileName}");
                Console.WriteLine($"{file.FullName}");
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

    private void ResetCounters()
    {
        _fileCount = 0;
        _folderCount = 0;
        _errorCount = 0;
        _renameCount = 0;
    }
}