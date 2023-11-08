using Docker.DotNet;
using Docker.DotNet.Models;
using DockerStatus;

DockerClient client = new DockerClientConfiguration().CreateClient();

Console.OutputEncoding = System.Text.Encoding.UTF8;

while (true)
{
    var res = await client.Containers.ListContainersAsync(new ContainersListParameters { Limit = 50 });

    Console.Clear();
    Console.CursorTop = 0;
    Console.CursorLeft = 0;

    Write(res.ToList());

    await Task.Delay(1000);
}

void Write(List<ContainerListResponse> items)
{
    var runningContainers = items.Where(i => i.State == "running").ToList();
    if(runningContainers.Count != 0)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╭─ Running containers ───────────────────────────────────────────────────────────────────────────────────────╮");

        foreach (var container in runningContainers)
        {
            WriteElement(container, ConsoleColor.Green);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╰────────────────────────────────────────────────────────────────────────────────────────────────────────────╯");

        Console.WriteLine();
    }

    var notRunningContainers = items.Where(i => i.State != "running").ToList();
    if(notRunningContainers.Count != 0)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("╭─ Other containers ─────────────────────────────────────────────────────────────────────────────────────────╮");

        foreach (var container in notRunningContainers)
        {
            WriteElement(container, ConsoleColor.DarkGray);
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("╰────────────────────────────────────────────────────────────────────────────────────────────────────────────╯");
    }
}

void WriteElement(ContainerListResponse container, ConsoleColor borderColor)
{
    Console.ForegroundColor = borderColor;
    Console.Write("│ ");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(container.ID.Substring(0, 12).ToLength(16));

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(container.Image.ToLength(25));

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write(container.Names[0].TrimStart('/').ToLength(28));

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(container.Status.Split(' ')[0].ToLength(21));

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"{container.Created.ToString("MM. dd. HH:mm:ss")}");

    Console.ForegroundColor = borderColor;
    Console.WriteLine(" │");
}
