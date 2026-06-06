namespace P3D;

internal static class CommandLineArgHandler
{
    private static bool _forceGraphics;
    private static bool _noSplash;

    public static void Initialize(String[] args)
    {
        if (args.Length > 0)
        {
            if (args.Any(a => a.Equals("-forcegraphics")))
            {
                _forceGraphics = true;
            }
            if (args.Any(a => a.Equals("-nosplash")))
            {
                _noSplash = true;
            }
        }

        foreach (String arg in args)
        {
            if (arg.Contains(':') == false)
            {
                continue;
            }

            String identifier = arg[..arg.IndexOf(':')];
            String value = arg[(arg.IndexOf(':') + 1)..];

            switch (identifier)
            {
                case "MAP":
                    MapPreviewScreen.DetectMapPath(value);
                    break;

                default:
                    break;
            }
        }
    }

    public static bool ForceGraphics => _forceGraphics;
    public static bool NoSplash => _noSplash;
}
