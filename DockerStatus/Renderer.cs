using System.Globalization;
using Docker.DotNet.Models;

namespace DockerStatus;

public static class Renderer
{
    private const ConsoleColor White = ConsoleColor.White;
    private const ConsoleColor Blue = ConsoleColor.Blue;
    private const ConsoleColor Gray = ConsoleColor.DarkGray;
    private const ConsoleColor Green = ConsoleColor.Green;
    private const ConsoleColor Yellow = ConsoleColor.Yellow;
    
    public static void RenderResult(IList<ContainerListResponse> items)
    {
        var totalWidth = Console.WindowWidth % 2 == 0
            ? Console.WindowWidth - 1
            : Console.WindowWidth;

        if (totalWidth < 80)
        {
            "《 REQUIRED WIDTH: at least 80 characters 》".WriteLineWith(Yellow);
            return;
        }

        var header = $"╭─ Docker containers ({items.Count}) ─".PadRight(totalWidth - 1, '─') + "╮";
        header.WriteLineWith(White);

        if (items.Count > 0)
        {
            var runningContainers = items.Where(i => i.State.IsRunning()).Take(25).ToList();
            if (runningContainers.Count != 0)
            {
                ConsoleHelpers.DoWithBorder(() =>
                {
                    var x = $"╭─── Running containers ({runningContainers.Count}) ─".PadRight(totalWidth - 3, '─') +
                            "╮";
                    x.WriteWith(Green);
                }, White);

                foreach (var container in runningContainers)
                {
                    RenderElement(container, Green, totalWidth, shortenStatusMessage: false);
                }

                ConsoleHelpers.DoWithBorder(() =>
                {
                    var x = "╰─".PadRight(totalWidth - 3, '─') + "╯";
                    x.WriteWith(Green);
                }, White);
            }

            var notRunningContainers = items.Where(i => i.State.IsRunning() == false).Take(25).ToList();
            if (notRunningContainers.Count != 0)
            {
                ConsoleHelpers.DoWithBorder(() =>
                {
                    var x = $"╭─── Other containers ({notRunningContainers.Count}) ─".PadRight(totalWidth - 3, '─') +
                            "╮";
                    x.WriteWith(Gray);
                }, White);

                foreach (var container in notRunningContainers)
                {
                    RenderElement(container, ConsoleColor.DarkGray, totalWidth);
                }

                ConsoleHelpers.DoWithBorder(() =>
                {
                    var x = "╰─".PadRight(totalWidth - 3, '─') + "╯";
                    x.WriteWith(Gray);
                }, White);
            }
        }
        else
        {
            ConsoleHelpers.DoWithBorder(() =>
            {
                const string shrug = "¯\\_(ツ)_/¯";
                var padding = new string(' ', (totalWidth - 12) / 2);
                $"{padding}{shrug}{padding} ".WriteWith(Yellow);
            }, White);
        }

        var footer = "╰─".PadRight(totalWidth - 1, '─') + "╯";
        footer.WriteLineWith(White);
    }

    private static void RenderElement(
        ContainerListResponse container,
        ConsoleColor borderColor,
        int totalWidth,
        bool shortenStatusMessage = true)
    {
        var itemLength = (totalWidth - 59) / 2;

        ConsoleHelpers.DoWithBorder(() =>
        {
            ConsoleHelpers.DoWithBorder(() =>
            {
                $" {container.ID[..12].ToLength(17)}".WriteWith(Gray);
                container.Image.ToLength(itemLength).WriteWith(White);
                container.Names[0].TrimStart('/').ToLength(itemLength).WriteWith(Blue);
                (shortenStatusMessage ? container.Status.Split(' ')[0] : container.Status).ToLength(22).WriteWith(Gray);
                container.Created.ToString("MM/dd HH:mm:ss ", CultureInfo.InvariantCulture).WriteWith(Gray);
            }, borderColor, useNewLine: false);
        }, White);
    }
}
