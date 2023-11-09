namespace DockerStatus;

public static class Constants
{
    public const string RunningState = "running";
}

public static class ConstantHelpers
{
    public static bool IsRunning(this string state) =>
        string.Equals(state, Constants.RunningState, StringComparison.OrdinalIgnoreCase);
}
