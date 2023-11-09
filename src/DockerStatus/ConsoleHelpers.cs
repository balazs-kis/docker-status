namespace DockerStatus;

public static class ConsoleHelpers
{
    public static string ToLength(this string input, int length) =>
        input.Length < length
            ? input.PadRight(length)
            : $"{input.Substring(0, length - 4)}... ";
    
    public static void WriteWith(this string input, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(input);
    }
    
    public static void WriteLineWith(this string input, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(input);
    }

    public static void DoWithBorder(Action action, ConsoleColor borderColor, bool useNewLine = true)
    {
        Console.ForegroundColor = borderColor;
        Console.Write('│');
        
        action();
        
        Console.ForegroundColor = borderColor;
        if(useNewLine) Console.WriteLine('│');
        else Console.Write('│');
    }
    
    public static void ResetConsole()
    {
        Console.CursorTop = 0;
        Console.CursorLeft = 0;
        Console.Clear();
    }
}
