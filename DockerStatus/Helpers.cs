namespace DockerStatus;

public static class Helpers
{
    public static string ToLength(this string input, int length)
    {
        if (input.Length < length)
        {
            return input.PadRight(length);
        }

        return $"{input.Substring(0, length - 1)} ";
    }
}
