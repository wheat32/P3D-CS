using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class Logger
{
    public enum LogTypes
    {
        Message,
        Debug,
        ErrorMessage,
        Warning,
        Entry
    }

    private const String CRASHLOG_SEPARATOR =
        "---------------------------------------------------------------------------------";
    private const String LOG_VERSION = "2.4";
    private const int ERROR_ID_PAD_LENGTH = 3;
    private const int LOG_FONT_SCALE_NUMERATOR = 51;
    private const int LOG_FONT_SCALE_DENOMINATOR = 100;
    private const float LOG_FONT_SCALE = LOG_FONT_SCALE_NUMERATOR / (float)LOG_FONT_SCALE_DENOMINATOR;
    private const int LOG_ITEM_COUNT_BASE = 10;
    private const int LOG_ITEM_HEIGHT = 16;
    private const int LOG_ITEM_HEIGHT_THRESHOLD = 680;
    private const int LOG_PADDING_X = 5;
    private const int LOG_PADDING_Y = 2;
    private const int LOG_BG_ALPHA = 150;
    private const int LOG_BG_PADDING = 10;

    private static readonly List<String> _history = [];
    private static String _longestStackEntry = "GameModeManager.SetGameModePointer";

    public static bool DisplayLog { get; set; }

    private static readonly String[] ErrorHeaders =
    {
        "I AM ERROR!",
        "Minecraft crashed.",
        "Missingno.",
        "1 ERROR",
        "GET TO DA CHOPPA",
        "Fire attacks might be super effective...",
        "Does this help?",
        "Work! Pleeeeeeeease?",
        "WHAT IS THIS?",
        "I find your lack of [ERROR] disturbing.",
        "Blame Darkfire.",
        "RTFM",
        "FEZ II announced.",
        "At least it's not a Blue Screen.",
        "Kernel PANIC",
        "I'm sorry, Dave, I'm afraid I can't do that.",
        "Never gonna give you up ~",
        "Wouldn't have happened with Swift.",
        "Team Rocket blasting off again!",
        "Snorlax just sat on your computer!",
        "Wut?",
        "Mojang buys Microsoft! Get your new Mojang operating system now. With more blocks and scrolls.",
        "HλLF-LIFE 2 confirmed",
        "(╯°□°）╯︵ ┻━┻"
    };

    public static void Log(LogTypes logType, String message)
    {
        try
        {
            String currentTime = GetLogTime(DateTime.Now);
            String logString = logType == LogTypes.Entry
                ? "]" + message
                : $"{logType} ({currentTime}): {message}";

            Debug("Logger: " + logString);

            String logPath = Path.Combine(GameController.GamePath, "log.dat");
            String existing = File.Exists(logPath) == true
                ? File.ReadAllText(logPath)
                : "";

            String newContent = existing.Equals("")
                ? logString
                : existing + Environment.NewLine + logString;

            File.WriteAllText(logPath, newContent);
        }
        catch { }
    }

    public static String LogCrash(Exception ex)
    {
        try
        {
            DateTime now = DateTime.Now;
            String logName = $"{now.Year}-{now.Month:D2}-{now.Day:D2}_{now.Hour:D2}.{now.Minute:D2}.{now.Second:D2}_crash.dat";

            String contentPacks = Core.GameOptions != null
                ? Core.GameOptions.ContentPackNames.ArrayToString()
                : "{}";

            String gameMode = GameModeManager.ActiveGameMode != null
                ? GameModeManager.ActiveGameMode.DirectoryName
                : "[No GameMode loaded]";

            String onlineInfo = "GameJolt Account: FALSE";
            if (Core.Player != null)
            {
                onlineInfo = "GameJolt Account: " + Core.Player.IsGameJoltSave.ToString().ToUpper();
                if (Core.Player.IsGameJoltSave == true)
                {
                    onlineInfo += " (" + Core.GameJoltSave.GameJoltID + ")";
                }
            }

            String scriptInfo = "Actionscript: No script running";
            if (Core.CurrentScreen != null)
            {
                if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
                {
                    OverworldScreen overworldScreen = (OverworldScreen)Core.CurrentScreen;
                    if (overworldScreen.ActionScript.IsReady == false)
                    {
                        scriptInfo = "Actionscript: " + ActionScript.CSL().ScriptName +
                                     "; Line: " + ActionScript.CSL().CurrentLine;
                    }
                }
            }

            String serverInfo = "FALSE";
            if (ConnectScreen.Connected == true)
            {
                serverInfo = "TRUE (" +
                    JoinServerScreen.SelectedServer.GetName() + "/" +
                    JoinServerScreen.SelectedServer.GetAddressString() + ")";
            }

            String gameEnvironment = Core.CurrentScreen != null
                ? Core.CurrentScreen.Identification.ToString()
                : "[No Game Environment loaded]";

            String sandboxMode = Core.Player != null
                ? Core.Player.SandBoxMode.ToString()
                : "False";

            String gameInformation =
                $"{GameController.GAMENAME} {GameController.GAMEDEVELOPMENTSTAGE} version: {GameController.GAMEVERSION} ({GameController.RELEASEVERSION}){Environment.NewLine}" +
                $"Content Packs: {contentPacks}{Environment.NewLine}" +
                $"Active GameMode: {gameMode}{Environment.NewLine}" +
                $"{onlineInfo}{Environment.NewLine}" +
                $"Playing on Servers: {serverInfo}{Environment.NewLine}" +
                $"Game Environment: {gameEnvironment}{Environment.NewLine}" +
                $"{scriptInfo}{Environment.NewLine}" +
                $"File Validation: {Security.FileValidation.IsValid(true)}{Environment.NewLine}" +
                $"Sandboxmode: {sandboxMode}";

            String screenState = Core.CurrentScreen != null
                ? $"Screen state for the current screen ({Core.CurrentScreen.Identification}){Environment.NewLine}{Environment.NewLine}{Core.CurrentScreen.GetScreenStatus()}"
                : "[Screen state object not available]";

            String architecture = Environment.Is64BitOperatingSystem == true ? "64 Bit" : "32 Bit";

            String graphicsCardName = "[Unknown]";
            if (Core.GraphicsDevice != null)
            {
                graphicsCardName = Core.GraphicsDevice.Adapter?.Description ?? "[Unknown]";
            }

            long totalMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            double totalMemoryGb = Math.Round(totalMemoryBytes / Math.Pow(1024, 3), 2);

            String specs =
                $"Operating system: {Environment.OSVersion}{Environment.NewLine}" +
                $"Core architecture: {architecture}{Environment.NewLine}" +
                $"System time: {DateTime.Now}{Environment.NewLine}" +
                $"System language: {CultureInfo.CurrentCulture.EnglishName} ({CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName}) / Loaded game language: {Localization.LanguageSuffix}{Environment.NewLine}" +
                $"Decimal separator: {GameController.DecSeparator}{Environment.NewLine}" +
                $"Available physical memory: {totalMemoryGb} Gigabyte{Environment.NewLine}" +
                $"Available logical processors: {Environment.ProcessorCount}";

            String innerException = ex.InnerException != null ? ex.InnerException.Message : "NOTHING";
            String message = ex.Message ?? "NOTHING";
            String source = ex.Source ?? "NOTHING";
            String stackTrace = ex.StackTrace ?? "NOTHING";
            String helpLink = ex.HelpLink ?? "No helplink available.";

            String data = "NOTHING";
            if (ex.Data != null && ex.Data.Count > 0)
            {
                List<String> dataLines = [];
                int i = 0;
                foreach (System.Collections.DictionaryEntry entry in ex.Data)
                {
                    dataLines.Add($"[{entry.Key}: \"{entry.Value}\"]");
                    i++;
                }
                data = String.Join(Environment.NewLine, dataLines);
            }

            ErrorInformation informationItem = new ErrorInformation(ex);
            ObjectDump objDump = new ObjectDump(Core.CurrentScreen);
            String screenDump = objDump.Dump;

            String headerQuote = ErrorHeaders[Core.Random.Next(0, ErrorHeaders.Length)];

            String content =
                $"Kolben Games Crash Log V {LOG_VERSION}{Environment.NewLine}" +
                $"{GameController.GAMENAME} has crashed!{Environment.NewLine}" +
                $"// {headerQuote}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"Game information:{Environment.NewLine}{Environment.NewLine}" +
                $"{gameInformation}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"{screenState}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"System specifications:{Environment.NewLine}{Environment.NewLine}" +
                $"{specs}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $".NET installation information:{Environment.NewLine}{Environment.NewLine}" +
                $"{DotNetVersion.GetInstalled()}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"Graphics Card information:{Environment.NewLine}{Environment.NewLine}" +
                $"[CardName(s): \"{graphicsCardName}\"]{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"Error information:{Environment.NewLine}" +
                $"{Environment.NewLine}Message: {message}" +
                $"{Environment.NewLine}InnerException: {innerException}" +
                $"{Environment.NewLine}BaseException: {ex.GetBaseException().Message}" +
                $"{Environment.NewLine}HelpLink: {helpLink}" +
                $"{Environment.NewLine}Data: {data}" +
                $"{Environment.NewLine}Source: {source}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"{informationItem}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"CallStack:{Environment.NewLine}{Environment.NewLine}" +
                $"{stackTrace}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"Environment dump:{Environment.NewLine}{Environment.NewLine}" +
                $"{screenDump}{Environment.NewLine}{Environment.NewLine}" +
                $"{CRASHLOG_SEPARATOR}{Environment.NewLine}{Environment.NewLine}" +
                $"You should report this error.{Environment.NewLine}{Environment.NewLine}" +
                "Go to \"http://pokemon3d.net/forum/forums/6/create-thread\" to report this crash there.";

            String logPath = Path.Combine(GameController.GamePath, logName);
            File.WriteAllText(logPath, content);

            // Cross-platform: write to stderr instead of showing a message box
            Console.Error.WriteLine($"{GameController.GAMENAME} has crashed!");
            Console.Error.WriteLine($"Message: {ex.Message}");
            Console.Error.WriteLine($"Crash log written to: {logPath}");

            return $"\"CRASHLOG_{logPath}\" " +
                   $"\"ERRORTYPE_{informationItem.ErrorType}\" " +
                   $"\"ERRORID_{informationItem.ErrorID}\" " +
                   $"\"GAMEVERSION_{GameController.GAMEDEVELOPMENTSTAGE} {GameController.GAMEVERSION}\" " +
                   $"\"CODESOURCE_{ex.Source}\" " +
                   $"\"TOPSTACK_{ErrorInformation.GetStackItem(ex.StackTrace ?? "", 0)}\"";
        }
        catch (Exception exs)
        {
            Console.Error.WriteLine(exs.Message);
            Console.Error.WriteLine(exs.StackTrace);
        }

        return "";
    }

    public static void Debug(String message)
    {
        String stackTraceEntry = Environment.StackTrace.SplitAtNewline()[3];

        stackTraceEntry = stackTraceEntry.TrimStart();
        stackTraceEntry = stackTraceEntry[(stackTraceEntry.IndexOf(' ') + 1)..];
        stackTraceEntry = stackTraceEntry[..stackTraceEntry.IndexOf('(')];
        String pointString = stackTraceEntry[..stackTraceEntry.LastIndexOf('.')];
        stackTraceEntry = stackTraceEntry[(pointString.LastIndexOf('.') + 1)..];

        if (stackTraceEntry.Length > _longestStackEntry.Length)
        {
            _longestStackEntry = stackTraceEntry;
        }
        else
        {
            while (stackTraceEntry.Length < _longestStackEntry.Length)
            {
                stackTraceEntry += " ";
            }
        }

        System.Diagnostics.Debug.Print(stackTraceEntry + StringHelper.Tab + "| " + message);
        _history.Add("(" + GetLogTime(DateTime.Now) + ") " + message);
    }

    public static void DrawLog()
    {
        if (DisplayLog == false || _history.Count == 0 || FontManager.ChatFont == null)
        {
            return;
        }

        int max = _history.Count - 1;
        int itemCount = LOG_ITEM_COUNT_BASE;
        if (Core.windowSize.Height > LOG_ITEM_HEIGHT_THRESHOLD)
        {
            itemCount += (int)Math.Floor(
                (Core.windowSize.Height - LOG_ITEM_HEIGHT_THRESHOLD) / (double)LOG_ITEM_HEIGHT);
        }

        int min = Math.Max(0, max - itemCount);

        int maxWidth = 0;
        for (int i = min; i <= max; i++)
        {
            int w = (int)(FontManager.ChatFont.MeasureString(_history[i]).X * LOG_FONT_SCALE);
            if (w > maxWidth)
            {
                maxWidth = w;
            }
        }

        Canvas.DrawRectangle(
            new Rectangle(0, 0, maxWidth + LOG_BG_PADDING, (itemCount + 1) * LOG_ITEM_HEIGHT + 2),
            new Color(0, 0, 0, LOG_BG_ALPHA));

        int c = 0;
        for (int i = min; i <= max; i++)
        {
            Core.SpriteBatch.DrawString(
                FontManager.ChatFont,
                _history[i],
                new Vector2(LOG_PADDING_X, LOG_PADDING_Y + c * LOG_ITEM_HEIGHT),
                Color.White, 0f, Vector2.Zero, LOG_FONT_SCALE, SpriteEffects.None, 0f);
            c++;
        }
    }

    private static String GetLogTime(DateTime d)
    {
        return $"{d.Hour:D2}:{d.Minute:D2}:{d.Second:D2}";
    }

    public class ErrorInformation
    {
        public int ErrorID { get; private set; } = -1;
        public String ErrorType { get; private set; } = "";
        public String ErrorDescription { get; private set; } = "";
        public String ErrorSolution { get; private set; } = "";
        public String ErrorIDString { get; private set; } = "-1";

        public ErrorInformation(Exception ex)
        {
            String? stackTrace = ex.StackTrace;
            if (stackTrace == null)
            {
                return;
            }

            int currentIndex = 0;
            while (true)
            {
                String callSub = GetStackItem(stackTrace, currentIndex);

                switch (callSub)
                {
                    case "Microsoft.Xna.Framework.Content.ContentManager.OpenStream":
                        ErrorID = 1;
                        ErrorDescription = "The game was unable to load an asset (a Texture, a Sound or Music).";
                        ErrorSolution = "Make sure the file requested exists on your system.";
                        break;

                    case "P3D.MusicManager.PlayMusic":
                        ErrorID = 2;
                        ErrorDescription = "The game was unable to play a music file.";
                        ErrorSolution = "Make sure the file requested exists on your system. This might be caused by an invalid file in a ContentPack.";
                        break;

                    case "Microsoft.Xna.Framework.Graphics.Texture.GetAndValidateRect":
                        ErrorID = 3;
                        ErrorDescription = "The game was unable to process a texture file.";
                        ErrorSolution = "Code composed by Microsoft caused this issue. This might be caused by an invalid file in a ContentPack.";
                        break;

                    case "Microsoft.Xna.Framework.Graphics.Texture2D.CopyData[T]":
                        ErrorID = 4;
                        ErrorDescription = "The game was unable to process a texture file.";
                        ErrorSolution = "Code composed by Microsoft caused this issue. This might be caused by an invalid file in a ContentPack. Try to update your Graphics Card drivers.";
                        break;

                    case "Microsoft.Xna.Framework.Media.MediaQueue.Play":
                        ErrorID = 5;
                        ErrorDescription = "The game was unable to load or play a music file.";
                        ErrorSolution = "Ensure that the audio output is configured correctly on your system.";
                        break;

                    case "P3D.GameJolt.APICall.SetStorageData":
                        ErrorID = 100;
                        ErrorDescription = "The game was unable to connect to a GameJolt server.";
                        ErrorSolution = "This happened because you got logged out from GameJolt due to connection problems. Ensure that your connection to the internet is constant.";
                        break;

                    case "P3D.ScriptCommander.DoNPC":
                        ErrorID = 200;
                        ErrorDescription = "The game crashed trying to execute an NPC related command (starting with @npc.)";
                        ErrorSolution = "If this happened during your GameMode, inspect the file mentioned under \"Actionscript\".";
                        break;

                    case "P3D.Trainer..ctor":
                        ErrorID = 201;
                        ErrorDescription = "The game was unable to initialize a new instance of a trainer class.";
                        ErrorSolution = "If this is caused by your GameMode, make sure the syntax in the trainer file is correct.";
                        break;

                    case "P3D.ScriptComparer.GetArgumentValue":
                        ErrorID = 202;
                        ErrorDescription = "The game crashed trying to process a script.";
                        ErrorSolution = "If this is caused by your GameMode, make sure the syntax in the script or map file is correct.";
                        break;

                    case "P3D.ForcedCrash.Crash":
                        ErrorID = 300;
                        ErrorDescription = "The game crashed on purpose.";
                        ErrorSolution = "Don't hold down F3 and C at the same time for a long time ;)";
                        break;

                    case "P3D.Security.ProcessValidation.ReportProcess":
                        ErrorID = 301;
                        ErrorDescription = "A malicious process was detected.";
                        ErrorSolution = "Close all processes with the details given in the Data of the crashlog.";
                        break;

                    case "P3D.Security.FileValidation.CheckFileValid":
                        ErrorID = 302;
                        ErrorDescription = "The game detected edited or missing files.";
                        ErrorSolution = "For online play, ensure that you are running the unmodded version of Pokémon3D. You can enable Content Packs.";
                        break;

                    case "Microsoft.Xna.Framework.Graphics.SpriteFont.GetIndexForCharacter":
                        ErrorID = 900;
                        ErrorDescription = "The game was unable to display a certain character which is not in the standard Latin alphabet.";
                        ErrorSolution = "Make sure the GameMode you are playing doesn't use any invalid characters in its scripts and maps.";
                        break;

                    case "P3D.Player.LoadPlayer":
                        ErrorID = 901;
                        ErrorDescription = "The game failed to load a save state.";
                        ErrorSolution = "There are multiple reasons for the game to fail at loading a save state. There could be a missing file in the player directory or corrupted files.";
                        break;

                    case "Microsoft.Xna.Framework.BoundingFrustum.ComputeIntersectionLine":
                        ErrorID = 902;
                        ErrorDescription = "The game failed to set up camera mechanics.";
                        ErrorSolution = "This error is getting produced by an internal Microsoft class. Please redownload the game if this error keeps appearing.";
                        break;

                    case "P3D.Pokemon.Wild":
                        ErrorID = 903;
                        ErrorDescription = "The game crashed while attempting to generate a new Pokémon.";
                        ErrorSolution = "This error could have multiple sources. If you made your own Pokémon data file for a GameMode, check it for invalid values.";
                        break;

                    case "-1":
                        ErrorID = -1;
                        ErrorDescription = "The error is undocumented in the error handling system.";
                        ErrorSolution = "NaN";
                        break;

                    default:
                        currentIndex++;
                        continue;
                }
                break;
            }

            if (ErrorID > -1)
            {
                ErrorIDString = ErrorID.ToString();
                while (ErrorIDString.Length < ERROR_ID_PAD_LENGTH)
                {
                    ErrorIDString = "0" + ErrorIDString;
                }
            }
        }

        public override String ToString()
        {
            if (ErrorID > -1 && ErrorID < 100)
            {
                ErrorType = "Assets";
            }
            else if (ErrorID > 99 && ErrorID < 200)
            {
                ErrorType = "GameJolt";
            }
            else if (ErrorID > 199 && ErrorID < 300)
            {
                ErrorType = "Scripts";
            }
            else if (ErrorID > 299 && ErrorID < 400)
            {
                ErrorType = "Forced Crash";
            }
            else if (ErrorID > 899 && ErrorID < 1000)
            {
                ErrorType = "Misc.";
            }
            else
            {
                ErrorType = "NaN";
            }

            return
                "Error solution:" + Environment.NewLine +
                "(The provided solution might not work for your problem)" + Environment.NewLine +
                Environment.NewLine +
                "Error ID: " + ErrorID + Environment.NewLine +
                "Error Type: " + ErrorType + Environment.NewLine +
                "Error Description: " + ErrorDescription + Environment.NewLine +
                "Error Solution: " + ErrorSolution;
        }

        public static String GetStackItem(String stack, int i)
        {
            String[] lines = stack.SplitAtNewline();
            if (i >= lines.Length)
            {
                return "-1";
            }

            String callSub = lines[i].TrimStart();
            if (callSub.IndexOf(' ') < 0 || callSub.IndexOf('(') < 0)
            {
                return "-1";
            }

            callSub = callSub[(callSub.IndexOf(' ') + 1)..];
            callSub = callSub[..callSub.IndexOf('(')];
            return callSub;
        }
    }
}
