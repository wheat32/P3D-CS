namespace P3D;

public static class Localization
{
    public static String LanguageSuffix { get; set; } = "en";
    public static Dictionary<String, Token> LocalizationTokens { get; } = [];

    public static void Load(String languageSuffix)
    {
        LocalizationTokens.Clear();
        LanguageSuffix = languageSuffix;

        Logger.Debug("Loaded language [" + languageSuffix + "]");

        LoadTokenFile(GameMode.DefaultLocalizationsPath, false);

        if (GameModeManager.GameModeCount > 0)
        {
            String gameModeLocPath = GameModeManager.ActiveGameMode.LocalizationsPath;
            if (gameModeLocPath != GameMode.DefaultLocalizationsPath)
            {
                LoadTokenFile(gameModeLocPath, true);
            }
        }
    }

    public static void ReloadGameModeTokens()
    {
        LocalizationTokens.Clear();

        LoadTokenFile(GameMode.DefaultLocalizationsPath, false);

        if (GameModeManager.GameModeCount > 0)
        {
            String gameModeLocPath = GameModeManager.ActiveGameMode.LocalizationsPath;
            if (gameModeLocPath != GameMode.DefaultLocalizationsPath)
            {
                LoadTokenFile(gameModeLocPath, true);
            }
        }

        Logger.Debug("---Reloaded GameMode Tokens---");
    }

    public static void LoadTokenFile(String path, bool isGameModeFile)
    {
        String[] pathParts = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        String fullPath = Path.Combine([GameController.GamePath, ..pathParts]);
        String tokenFullPath = Path.Combine(fullPath, "Tokens_" + LanguageSuffix + ".dat");

        Logger.Debug("Token filepath: " + tokenFullPath);

        if (Directory.Exists(fullPath) == true && Directory.GetFiles(fullPath).Length > 0)
        {
            if (File.Exists(tokenFullPath) == false)
            {
                Logger.Debug("Did NOT find token file for suffix: " + LanguageSuffix);
            }

            if (File.Exists(tokenFullPath) == true)
            {
                Logger.Debug("Found token file for suffix: " + LanguageSuffix);
                String[] tokensFile = File.ReadAllLines(tokenFullPath);
                foreach (String tokenLine in tokensFile)
                {
                    if (tokenLine.Contains(",") == false)
                    {
                        continue;
                    }

                    int splitIdx = tokenLine.IndexOf(',');
                    String tokenName = tokenLine[..splitIdx];
                    String tokenContent = tokenLine.Length > tokenName.Length + 1
                        ? tokenLine[(splitIdx + 1)..]
                        : "";

                    if (LocalizationTokens.ContainsKey(tokenName) == false)
                    {
                        LocalizationTokens.Add(tokenName, new Token(tokenContent, LanguageSuffix, isGameModeFile));
                    }
                    else
                    {
                        LocalizationTokens.Remove(tokenName);
                        LocalizationTokens.Add(tokenName, new Token(tokenContent, LanguageSuffix, isGameModeFile));
                    }
                }
            }

            if (LanguageSuffix.Equals("en") == false)
            {
                String fallbackPath = fullPath + "Tokens_en.dat";
                if (File.Exists(fallbackPath) == true)
                {
                    String[] fallbackFile = File.ReadAllLines(fallbackPath);
                    foreach (String tokenLine in fallbackFile)
                    {
                        if (tokenLine.Contains(",") == false)
                        {
                            continue;
                        }

                        int splitIdx = tokenLine.IndexOf(',');
                        String tokenName = tokenLine[..splitIdx];
                        String tokenContent = tokenLine.Length > tokenName.Length + 1
                            ? tokenLine[(splitIdx + 1)..]
                            : "";

                        if (LocalizationTokens.ContainsKey(tokenName) == false)
                        {
                            LocalizationTokens.Add(tokenName, new Token(tokenContent, "en", isGameModeFile));
                        }
                        else if (LocalizationTokens[tokenName].IsGameModeToken == false &&
                                 isGameModeFile == true)
                        {
                            LocalizationTokens.Remove(tokenName);
                            LocalizationTokens.Add(tokenName, new Token(tokenContent, LanguageSuffix, isGameModeFile));
                        }
                    }
                }
            }
        }
    }

    public static String GetString(String s, String defaultValue = "")
    {
        String result;

        if (LocalizationTokens.ContainsKey(s) == true)
        {
            if (LocalizationTokens.TryGetValue(s, out Token? token) == false)
            {
                return s;
            }
            result = token!.TokenContent;
        }
        else
        {
            result = String.IsNullOrEmpty(defaultValue) == true ? s : defaultValue;
        }

        if (Core.Player != null)
        {
            result = result.Replace("<playername>", Core.Player.Name);
            result = result.Replace("<player.name>", Core.Player.Name);
            result = result.Replace("<rivalname>", Core.Player.RivalName);
            result = result.Replace("<rival.name>", Core.Player.RivalName);
        }
        if (result.Contains("<name>") == true)
        {
            result = result.Replace("<name>", "[POKEMONNAME]");
        }
        if (result.Contains("<newitem>") == true)
        {
            result = result.Replace("<newitem>", "[NEWITEM]");
        }
        if (result.Contains("<olditem>") == true)
        {
            result = result.Replace("<olditem>", "[OLDITEM]");
        }
        if (result.Contains("<item>") == true)
        {
            result = result.Replace("<item>", "[ITEM]");
        }
        if (result.Contains('<') == true)
        {
            result = ScriptVersion2.ScriptComparer.EvaluateConstruct(result).ToString() ?? result;
        }
        return result;
    }

    public static bool TokenExists(String tokenName)
    {
        return LocalizationTokens.ContainsKey(tokenName);
    }
}

public class Token
{
    public String TokenContent { get; set; }
    public String TokenLanguageSuffix { get; set; }
    public bool IsGameModeToken { get; set; }

    public Token(String tokenContent, String tokenLanguageSuffix, bool isGameModeToken)
    {
        TokenContent = tokenContent;
        TokenLanguageSuffix = tokenLanguageSuffix;
        IsGameModeToken = isGameModeToken;
    }
}
