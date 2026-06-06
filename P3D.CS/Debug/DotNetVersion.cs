namespace P3D;

/// <summary>Reports the current .NET runtime version (cross-platform replacement for the old registry-based approach).</summary>
internal static class DotNetVersion
{
    public static String GetInstalled()
    {
        return $".NET {Environment.Version} ({System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription})";
    }
}
