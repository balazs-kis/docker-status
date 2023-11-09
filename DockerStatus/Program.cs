using Docker.DotNet;
using Docker.DotNet.Models;
using DockerStatus;

const ConsoleColor white = ConsoleColor.White;
const ConsoleColor red = ConsoleColor.Red;
const ConsoleColor yellow = ConsoleColor.Yellow;

// Set encoding to display special characters correctly.
Console.OutputEncoding = System.Text.Encoding.UTF8;

DockerClient? client = null;
var previousWidth = 0;
IList<ContainerListResponse> previousResponse = new List<ContainerListResponse>();

while (true)
{
    try
    {
        if (client is null)
        {
            await Initialize();
        }
        
        await Update(client!, previousWidth);
        
        await Task.Delay(750);
    }
    catch (Exception ex)
    {
        ConsoleHelpers.ResetConsole();
        
        "Ooops... something went wrong.".WriteLineWith(red);
        "Details: ".WriteWith(white);
        $"{ex.GetType().FullName}: {ex.Message}".WriteLineWith(yellow);

        // Clear client to trigger re-initialization in the next iteration.
        client = null;
        // Set width to 0 to trigger re-rendering if the next response is successful,
        // but the same as the last successful result was.
        previousWidth = 0;
        
        await Task.Delay(5000);
    }
}

async Task Initialize()
{
    client = new DockerClientConfiguration().CreateClient();

    // Get 1 container to check if the docker host is available.
    await client.Containers.ListContainersAsync(new ContainersListParameters { Limit = 1 });
}

async Task Update(IDockerClient dockerClient, int oldWidth)
{
    var width = Console.WindowWidth;
    var response = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { Limit = 1000 });

    // Only redraw the console if the size or the response data has changed.
    if (width != oldWidth || HasResponseChanged(previousResponse, response))
    {
        ConsoleHelpers.ResetConsole();

        Renderer.RenderResult(response);

        previousWidth = width;
        previousResponse = response;
    }
}

bool HasResponseChanged(IList<ContainerListResponse> oldList, IList<ContainerListResponse> newList)
{
    if (oldList.Count != newList.Count)
    {
        return true;
    }

    for (var i = 0; i < oldList.Count; i++)
    {
        var a = oldList[i];
        var b = newList[i];

        if (string.Equals(a.ID, b.ID, StringComparison.OrdinalIgnoreCase) == false)
        {
            return true;
        }
        
        if (string.Equals(a.State, b.State, StringComparison.OrdinalIgnoreCase) == false)
        {
            return true;
        }

        if (a.State.IsRunning() &&
            string.Equals(a.Status, b.Status, StringComparison.OrdinalIgnoreCase) == false)
        {
            return true;
        }

        if (a.State.IsRunning() == false &&
            string.Equals(a.Status.Split(' ')[0], b.Status.Split(' ')[0], StringComparison.OrdinalIgnoreCase) == false)
        {
            return true;
        }
    }

    return false;
}
