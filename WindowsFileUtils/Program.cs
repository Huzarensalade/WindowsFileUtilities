using System.Text;
using WindowsFileUtils.Services;

Console.Clear();
Console.OutputEncoding = Encoding.UTF8;
Console.CursorVisible = false;
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Welcome to Windows File Utilities");
Console.ResetColor();
Console.WriteLine("\nUse ⬆️ and ⬇️ to navigate and press \u001b[32mEnter/Return\u001b[0m to select:");
(int left, int top) = Console.GetCursorPosition();
var option = 1;
var decorator = "✅ \u001b[32m";
ConsoleKeyInfo key;
bool isSelected = false;

while (!isSelected)
{
    Console.SetCursorPosition(left, top);

    Console.WriteLine($"{(option == 1 ? decorator : "   ")}Folder Unpacking tool\u001b[0m");
    Console.WriteLine($"{(option == 2 ? decorator : "   ")}Duplicate File Detector tool\u001b[0m");

    key = Console.ReadKey(false);

    switch (key.Key)
    {
        case ConsoleKey.UpArrow:
            option = option == 1 ? 2 : option - 1;
            break;

        case ConsoleKey.DownArrow:
            option = option == 2 ? 1 : option + 1;
            break;

        case ConsoleKey.Enter:
            isSelected = true;
            break;
    }
}

switch (option)
{
    case 1:
        var subfolderUnpacker = new SubfolderUnpacker();
        subfolderUnpacker.Run();
        break;
    case 2:
        var duplicateChecker = new DuplicateChecker();
        duplicateChecker.Run();
        break;
}