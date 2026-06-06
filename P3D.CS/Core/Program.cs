namespace P3D;

internal static class Program
{
    private static bool _gameCrashed;

    [STAThread]
    public static void Main(String[] args)
    {
        System.Diagnostics.Debug.Print(" ");
        System.Diagnostics.Debug.Print("PROGRAM EXECUTION STARTED");
        System.Diagnostics.Debug.Print("STACK TRACE ENTRY                   | MESSAGE");
        System.Diagnostics.Debug.Print("------------------------------------|------------------------------------");

        CommandLineArgHandler.Initialize(args);

        Logger.Debug("---Start game---");

        using GameController game = new GameController();
#if DEBUG
        game.Run();
#else
        try
        {
            game.Run();
        }
        catch (Exception ex)
        {
            _gameCrashed = true;
            Logger.ErrorInformation informationItem = new Logger.ErrorInformation(ex);
            Logger.LogCrash(ex);
            Logger.Log(Logger.LogTypes.ErrorMessage,
                $"The game crashed with error ID: {informationItem.ErrorIDString} ({ex.Message})");
        }
#endif
    }

    public static bool GameCrashed => _gameCrashed;
}
