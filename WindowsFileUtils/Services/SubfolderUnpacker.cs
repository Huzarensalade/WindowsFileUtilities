namespace WindowsFileUtils.Services;

public class SubfolderUnpacker
{
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

        Console.Clear();

    }
}