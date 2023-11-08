using Docker.DotNet;
using Docker.DotNet.Models;
using DockerStatus;
using System.Globalization;

DockerClient client = new DockerClientConfiguration().CreateClient();

Console.OutputEncoding = System.Text.Encoding.UTF8;

while (true)
{
    var res = await client.Containers.ListContainersAsync(new ContainersListParameters { Limit = 100 });

    Console.Clear();
    Console.CursorTop = 0;
    Console.CursorLeft = 0;

    Write(res.ToList());

    await Task.Delay(750);
}

void Write(List<ContainerListResponse> items)
{
    var totalWidth = Console.WindowWidth % 2 == 0 ? Console.WindowWidth - 1 : Console.WindowWidth;

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("╭─ Docker containers ─".PadRight(totalWidth - 1, '─') + "╮");

    if(items.Count > 0)
    {
        var runningContainers = items.Where(i => i.State == "running").ToList();
        if (runningContainers.Count != 0)
        {
            WriteLineWithBorder(
                "╭─── Running containers ─".PadRight(totalWidth - 3, '─') + "╮",
                ConsoleColor.Green);

            foreach (var container in runningContainers)
            {
                WriteElement(container, ConsoleColor.Green, totalWidth);
            }

            WriteLineWithBorder(
                "╰─".PadRight(totalWidth - 3, '─') + "╯",
                ConsoleColor.Green);
        }

        var notRunningContainers = items.Where(i => i.State != "running").ToList();
        if (notRunningContainers.Count != 0)
        {
            WriteLineWithBorder("╭─── Other containers ─".PadRight(totalWidth - 3, '─') + "╮",
                ConsoleColor.DarkGray);

            foreach (var container in notRunningContainers)
            {
                WriteElement(container, ConsoleColor.DarkGray, totalWidth);
            }

            WriteLineWithBorder(
                "╰─".PadRight(totalWidth - 3, '─') + "╯",
                ConsoleColor.DarkGray);
        }
    }
    else
    {
        WriteLineWithBorder(GetEmptyMessage(totalWidth), ConsoleColor.Yellow);
    }

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("╰─".PadRight(totalWidth - 1, '─') + "╯");
}

void WriteElement(ContainerListResponse container, ConsoleColor borderColor, int totalwidth)
{
    var itemLength = (totalwidth - 59) / 2;

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("│");
    Console.ForegroundColor = borderColor;
    Console.Write("│ ");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(container.ID.Substring(0, 12).ToLength(17));

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(container.Image.ToLength(itemLength));

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write(container.Names[0].TrimStart('/').ToLength(itemLength));

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(container.Status.Split(' ')[0].ToLength(22));

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"{container.Created.ToString("MM/dd HH:mm:ss", CultureInfo.InvariantCulture)}");

    Console.ForegroundColor = borderColor;
    Console.Write(" │");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("│");
}

void WriteLineWithBorder(string message, ConsoleColor color)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("│");
    Console.ForegroundColor = color;
    Console.Write(message);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("│");
}

string GetEmptyMessage(int width)
{
    var shrug = "¯\\_(ツ)_/¯";
    var padding = new string(' ', (width - 12) / 2);

    return $"{padding}{shrug}{padding} ";
}
