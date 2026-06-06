using System.Globalization;

namespace P3D;

public class ActionScript
{
    public class ScriptLevel
    {
        public List<bool> WaitingEndWhen = [];
        public List<bool> Switched = [];
        public List<bool> WaitingEndIf = [];
        public List<bool> CanTriggerElse = [];
        public int IfIndex;
        public int WhenIndex;

        public List<Script> WhileQuery = [];
        public bool WhileQueryInitialized;

        public int ScriptVersion;
        public int CurrentLine;
        public String ScriptName = "";
    }

    public static ScriptLevel CSL()
    {
        return ScriptLevels[ScriptLevelIndex]!;
    }

    public static ScriptLevel?[] ScriptLevels = new ScriptLevel?[100];
    public static int ScriptLevelIndex = -1;
    public static bool emAddToWhile;

    public float reDelay;

    private Level? _levelRef;

    public ActionScript(Level? levelRef)
    {
        _levelRef = levelRef;
    }

    public static int TempInputDirection = -1;
    public static bool TempSpin;

    public List<Script> Scripts = [];

    public static String ScriptTrigger
    {
        get => _scriptTrigger;
        set { _scriptTrigger = value; }
    }
    private static String _scriptTrigger = "";

    public static bool IsInSightScript;

    public bool IsReady => Scripts.Count == 0;

    public void Update()
    {
        bool shouldRestart = true;
        while (shouldRestart)
        {
            shouldRestart = false;
            bool unlock = IsReady;

            if (Scripts.Count > 0)
            {
                Scripts[0].Update();
            }

            for (int i = 0; i <= Scripts.Count - 1; i++)
            {
                if (i <= Scripts.Count - 1)
                {
                    Script s = Scripts[i];

                    if (s.IsReady == true)
                    {
                        i -= 1;

                        AddToWhileQuery(s);
                        Scripts.Remove(s);
                        ScriptLevels[s.Level]!.CurrentLine += 1;

                        if (IsReady == false && s.CanContinue == true)
                        {
                            shouldRestart = true;
                            break;
                        }
                    }
                }
            }

            if (shouldRestart == false)
            {
                if (IsReady == true)
                {
                    if (unlock == false)
                    {
                        Logger.Debug("Unlock Camera");
                        ((OverworldCamera)Screen.Camera).YawLocked = false;
                        GameJolt.PokegearScreen.Call_Flag = "";
                    }
                    if (reDelay > 0.0f)
                    {
                        reDelay -= 0.1f;
                        if (reDelay <= 0.0f)
                        {
                            reDelay = 0.0f;
                        }
                    }
                }
            }
        }
    }

    public void StartScript(String input, int inputType, bool checkDelay = true,
                            bool resetInsight = true, String scriptTrigger = "")
    {
        Screen.Level.OwnPlayer.Opacity = 1;
        if (Core.Player.IsRunning() == true || Screen.Level.Riding == true)
        {
            Screen.Camera.Speed = 0.04f;
        }
        ScriptLevelIndex += 1;
        emAddToWhile = true;
        TempSpin = false;

        bool[] arr = new bool[100];
        ScriptLevels[ScriptLevelIndex] = new ScriptLevel
        {
            IfIndex = 0,
            WhenIndex = 0,
            ScriptVersion = 1,
            WaitingEndIf = [..arr],
            WaitingEndWhen = [..arr],
            CurrentLine = 0,
            ScriptName = "No script running",
            CanTriggerElse = [..arr],
            Switched = [..arr],
        };

        ScriptLevel l = ScriptLevels[ScriptLevelIndex]!;

        ActionScript.ScriptTrigger = scriptTrigger;

        if (resetInsight == true)
        {
            IsInSightScript = false;
        }

        if (reDelay == 0.0f || checkDelay == false)
        {
            switch (inputType)
            {
                case 0:
                    Logger.Debug("Start script (ID: " + input + ")");
                    l.ScriptName = "Type: Script; Input: " + input;

                    String path = GameModeManager.GetScriptPath(input + ".dat");
                    Security.FileValidation.CheckFileValid(path, false, "ActionScript.cs");

                    if (System.IO.File.Exists(path) == true)
                    {
                        String data = System.IO.File.ReadAllText(path);
                        data = data.Replace(Environment.NewLine, "^");
                        String[] scriptData = data.Split('^');
                        AddScriptLines(scriptData);
                    }
                    else
                    {
                        Logger.Log(Logger.LogTypes.ErrorMessage, "ActionScript.cs: The script file \"" + path + "\" doesn't exist!");
                    }
                    break;

                case 1:
                    Logger.Debug("Start Script (Text: " + input + ")");
                    l.ScriptName = "Type: Text; Input: " + input;

                    String textData = "version=2^@text.show(" + input + ")^" + ":end";
                    String[] textScriptData = textData.Split('^');
                    AddScriptLines(textScriptData);
                    break;

                case 2:
                    String activator = Environment.StackTrace.Split(Environment.NewLine)[3];
                    activator = activator.Remove(activator.IndexOf('('));

                    Logger.Debug("Start Script (DirectInput; " + activator + ")");
                    l.ScriptName = "Type: Direct; Input: " + input;

                    String directData = input.Replace(Environment.NewLine, "^");
                    String[] directScriptData = directData.Split('^');
                    AddScriptLines(directScriptData);
                    break;
            }
        }
    }

    private void AddScriptLines(String[] scriptData)
    {
        int i = 0;
        ScriptLevel l = ScriptLevels[ScriptLevelIndex]!;
        foreach (String rawLine in scriptData)
        {
            String newScript = rawLine;
            if (i == 0 && newScript.ToLower().StartsWith("version=") == true)
            {
                l.ScriptVersion = int.Parse(newScript.ToLower().Remove(0, "version=".Length));
                l.CurrentLine += 1;
            }
            else
            {
                while (newScript.StartsWith(" ") == true || newScript.StartsWith(StringHelper.Tab) == true)
                {
                    newScript = newScript.Remove(0, 1);
                }
                while (newScript.EndsWith(" ") == true || newScript.EndsWith(StringHelper.Tab) == true)
                {
                    newScript = newScript.Remove(newScript.Length - 1, 1);
                }
                if (newScript.Equals("") == false)
                {
                    Scripts.Insert(i, new Script(newScript, ScriptLevelIndex));
                    i += 1;
                }
            }
        }
    }

    public void Switch(Object answer)
    {
        ScriptLevel l = ScriptLevels[ScriptLevelIndex]!;
        bool proceed = false;
        bool first = true;

        while (proceed == false)
        {
            if (Scripts.Count == 0)
            {
                Logger.Log(Logger.LogTypes.Warning, "ActionScript.cs: Illegal \":when\" construct. Terminating execution.");
                break;
            }

            Script s = Scripts[0];

            switch (s.ScriptType)
            {
                case Script.ScriptTypes.select:
                    if (first == false)
                    {
                        l.WhenIndex += 1;
                        l.WaitingEndWhen[l.WhenIndex] = true;
                        l.Switched[l.WhenIndex] = true;
                    }
                    break;
                case Script.ScriptTypes.Command:
                    if (s.ScriptV2.Value.ToLower().StartsWith("options.show(") == true && first == false)
                    {
                        l.WhenIndex += 1;
                        l.WaitingEndWhen[l.WhenIndex] = true;
                        l.Switched[l.WhenIndex] = true;
                    }
                    break;
                case Script.ScriptTypes.SwitchWhen:
                case Script.ScriptTypes.when:
                    if (l.Switched[l.WhenIndex] == false)
                    {
                        bool equal = false;
                        String[] args = Scripts[0].Value.Split(';');

                        foreach (String arg in args)
                        {
                            if (ScriptVersion2.ScriptComparer.EvaluateConstruct(arg).Equals(
                                    ScriptVersion2.ScriptComparer.EvaluateConstruct(answer.ToString() ?? "")) == true)
                            {
                                equal = true;
                                break;
                            }
                        }

                        if (equal == true)
                        {
                            l.WaitingEndWhen[l.WhenIndex] = false;
                            proceed = true;
                        }
                        else
                        {
                            l.WaitingEndWhen[l.WhenIndex] = true;
                        }
                    }
                    break;
                case Script.ScriptTypes.SwitchEndWhen:
                case Script.ScriptTypes.endwhen:
                    l.WaitingEndWhen[l.WhenIndex] = false;
                    l.Switched[l.WhenIndex] = false;
                    l.WhenIndex -= 1;
                    if (l.WaitingEndWhen[l.WhenIndex] == false)
                    {
                        proceed = true;
                    }
                    break;
            }

            AddToWhileQuery(Scripts[0]);
            Scripts.RemoveAt(0);
            l.CurrentLine += 1;
            first = false;
        }
    }

    public void ChooseIf(bool t)
    {
        ScriptLevel l = ScriptLevels[ScriptLevelIndex]!;
        bool proceed = false;

        while (proceed == false)
        {
            if (Scripts.Count == 0)
            {
                Logger.Log(Logger.LogTypes.Warning, "ActionScript.cs: Illegal \":if\" construct. Terminating execution.");
                break;
            }

            Script s = Scripts[0];

            switch (s.ScriptType)
            {
                case Script.ScriptTypes.@if:
                case Script.ScriptTypes.SwitchIf:
                    l.IfIndex += 1;
                    if (l.WaitingEndIf[l.IfIndex - 1] == true)
                    {
                        l.WaitingEndIf[l.IfIndex] = true;
                        l.CanTriggerElse[l.IfIndex] = false;
                    }
                    else
                    {
                        if (t == true)
                        {
                            proceed = true;
                            l.WaitingEndIf[l.IfIndex] = false;
                            l.CanTriggerElse[l.IfIndex] = false;
                        }
                        else
                        {
                            l.WaitingEndIf[l.IfIndex] = true;
                            l.CanTriggerElse[l.IfIndex] = true;
                        }
                    }
                    break;
                case Script.ScriptTypes.@else:
                case Script.ScriptTypes.SwitchElse:
                    if (l.CanTriggerElse[l.IfIndex] == true)
                    {
                        l.WaitingEndIf[l.IfIndex] = false;
                        proceed = true;
                    }
                    else
                    {
                        l.WaitingEndIf[l.IfIndex] = true;
                    }
                    break;
                case Script.ScriptTypes.endif:
                case Script.ScriptTypes.SwitchEndIf:
                    l.IfIndex -= 1;
                    if (l.WaitingEndIf[l.IfIndex] == false)
                    {
                        proceed = true;
                    }
                    break;
            }

            AddToWhileQuery(Scripts[0]);
            Scripts.RemoveAt(0);
            l.CurrentLine += 1;
        }

        if (Scripts.Count > 0)
        {
            Scripts[0].Update();
        }
    }

    public void AddToWhileQuery(Script removedScript)
    {
        if (emAddToWhile == true)
        {
            if (ScriptLevelIndex == 0)
            {
                emAddToWhile = false;
            }
            else
            {
                ScriptLevelIndex -= 1;
            }
        }

        ScriptLevel csl = CSL();
        if (csl.WhileQueryInitialized == true && csl.ScriptVersion == 2)
        {
            csl.WhileQuery.Add(removedScript);

            if (removedScript.ScriptV2.ScriptType == ScriptV2.ScriptTypes.endwhile)
            {
                int i = 0;
                foreach (Script s in csl.WhileQuery)
                {
                    Scripts.Insert(i, s.Clone());
                    i += 1;
                }

                csl.WhileQuery.Clear();
                csl.WhileQueryInitialized = false;
            }
        }

        if (emAddToWhile == true)
        {
            ScriptLevelIndex += 1;
            emAddToWhile = false;
        }
    }

    // ---- Registers ----

    public static bool IsRegistered(String i)
    {
        CheckTimeBasedRegisters();
        if (Core.Player.RegisterData.Contains(",") == true)
        {
            String[] data = Core.Player.RegisterData.Split(',');

            foreach (String d in data)
            {
                String entry = d;
                if (entry.StartsWith("[") == true && entry.EndsWith("]") == false && entry.Contains("]") == true)
                {
                    entry = entry.Remove(0, entry.IndexOf(']') + 1);
                    if (entry.Equals(i) == true)
                    {
                        return true;
                    }
                }
                else
                {
                    if (entry.Equals(i) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        else
        {
            if (Core.Player.RegisterData.StartsWith("[") == true &&
                Core.Player.RegisterData.EndsWith("]") == false &&
                Core.Player.RegisterData.Contains("]") == true)
            {
                String d = Core.Player.RegisterData.Remove(0, Core.Player.RegisterData.IndexOf(']') + 1);
                if (d.Equals(i) == true)
                {
                    return true;
                }
            }
            else
            {
                if (Core.Player.RegisterData.Equals(i) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static void CheckTimeBasedRegisters()
    {
        if (Core.Player.RegisterData.Equals("") == false)
        {
            List<String> data = [Core.Player.RegisterData];
            if (Core.Player.RegisterData.Contains(",") == true)
            {
                data = [..Core.Player.RegisterData.Split(',')];
            }

            bool removedRegisters = false;

            for (int i = 0; i <= data.Count - 1; i++)
            {
                if (i <= data.Count - 1)
                {
                    String d = data[i];

                    if (d.StartsWith("[TIME|") == true)
                    {
                        String timeString = d.Remove(0, "[TIME|".Length);
                        timeString = timeString.Remove(timeString.IndexOf(']'));

                        String[] timeData = timeString.Split('|');

                        DateTime regDate = UnixToTime(timeData[0]);
                        int value = int.Parse(timeData[1]);
                        String format = timeData[2];

                        bool remove = false;

                        DateTime date1 = DateTime.Now;
                        DateTime date2 = regDate;
                        TimeSpan diff = date1 - date2;
                        switch (format)
                        {
                            case "days":
                            case "day":
                                if ((int)diff.TotalDays >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "minutes":
                            case "minute":
                                if ((int)diff.TotalMinutes >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "seconds":
                            case "second":
                                if ((int)diff.TotalSeconds >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "years":
                            case "year":
                                if ((int)Math.Floor(diff.TotalDays / 365.0) >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "weeks":
                            case "week":
                                if ((int)Math.Floor(diff.TotalDays / 7.0) >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "months":
                            case "month":
                                if (((date1.Year - date2.Year) * 12) + date1.Month - date2.Month >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "hours":
                            case "hour":
                                if ((int)diff.TotalHours >= value)
                                {
                                    remove = true;
                                }
                                break;
                            case "dayofweek":
                                if ((int)Math.Floor(diff.TotalDays / 7.0) >= value)
                                {
                                    remove = true;
                                }
                                break;
                        }

                        if (remove == true)
                        {
                            data.RemoveAt(i);
                            i -= 1;
                            removedRegisters = true;
                        }
                    }
                }
            }

            if (removedRegisters == true)
            {
                String s = "";

                if (data.Count > 0)
                {
                    foreach (String d in data)
                    {
                        if (s.Equals("") == false)
                        {
                            s += ",";
                        }
                        s += d;
                    }
                }

                Core.Player.RegisterData = s;
            }
        }
    }

    public static DateTime UnixToTime(String strUnixTime)
    {
        DateTime unixDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        strUnixTime = strUnixTime.Split('.')[0];
        long a = Convert.ToInt64(strUnixTime);
        unixDate = unixDate.AddSeconds(a);
        unixDate = unixDate.ToLocalTime();
        return unixDate;
    }

    public static String TimeToUnix(DateTime dteDate)
    {
        return (dteDate.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
            .TotalSeconds.ToString(CultureInfo.InvariantCulture);
    }

    public static void RegisterID(String i)
    {
        String data = Core.Player.RegisterData;

        if (data.Equals("") == true)
        {
            data = i;
        }
        else
        {
            String[] checkData = data.Split(',');
            if (checkData.Contains(i) == false)
            {
                data += "," + i;
            }
        }

        Core.Player.RegisterData = data;
    }

    public static void RegisterID(String name, String type, String value)
    {
        String data = Core.Player.RegisterData;

        String reg = "[" + type.ToUpper() + "|" + value + "]" + name;

        if (data.Equals("") == true)
        {
            data = reg;
        }
        else
        {
            String[] checkData = data.Split(',');
            if (checkData.Contains(reg) == false)
            {
                data += "," + reg;
            }
        }

        Core.Player.RegisterData = data;
    }

    public static void UnregisterID(String i)
    {
        String[] checkData = Core.Player.RegisterData.Split(',');
        String data = "";

        List<String> checkList = [..checkData];
        checkList.Remove(i);

        checkData = [..checkList];
        for (int a = 0; a <= checkData.Length - 1; a++)
        {
            if (a != 0)
            {
                data += ",";
            }
            data += checkData[a];
        }

        Core.Player.RegisterData = data;
    }

    public static void UnregisterID(String name, String type)
    {
        String[] data = Core.Player.RegisterData.Split(',');
        String newData = "";

        foreach (String line in data)
        {
            if (line.StartsWith("[") == true && line.Contains("]") == true && line.EndsWith("]") == false)
            {
                String lName = line.Remove(0, line.IndexOf(']') + 1);
                String lType = line.Remove(0, 1);
                lType = lType.Remove(lType.IndexOf('|'));

                if (lName.Equals(name) == false || lType.ToLower().Equals(type.ToLower()) == false)
                {
                    if (newData.Equals("") == false)
                    {
                        newData += ",";
                    }
                    newData += line;
                }
            }
            else
            {
                if (newData.Equals("") == false)
                {
                    newData += ",";
                }
                newData += line;
            }
        }

        Core.Player.RegisterData = newData;
    }

    public static void ChangeRegister(String name, String newValue)
    {
        String[] data = Core.Player.RegisterData.Split(',');
        String newData = "";

        foreach (String line in data)
        {
            if (newData.Equals("") == false)
            {
                newData += ",";
            }
            if (line.StartsWith("[") == true && line.Contains("]") == true && line.EndsWith("]") == false)
            {
                String lName = line.Remove(0, line.IndexOf(']') + 1);
                String lType = line.Remove(0, 1);
                lType = lType.Remove(lType.IndexOf('|'));

                if (lName.ToLower().Equals(name.ToLower()) == true)
                {
                    newData += "[" + lType + "|" + newValue + "]" + name;
                }
                else
                {
                    newData += line;
                }
            }
            else
            {
                newData += line;
            }
        }

        Core.Player.RegisterData = newData;
    }

    public static Object[] GetRegisterValue(String name)
    {
        String[] registers = Core.Player.RegisterData.Split(',');
        foreach (String line in registers)
        {
            if (line.StartsWith("[") == true && line.Contains("]") == true && line.EndsWith("]") == false)
            {
                String lName = line.Remove(0, line.IndexOf(']') + 1);

                if (lName.ToLower().Equals(name.ToLower()) == true)
                {
                    String lType = line.Remove(0, 1);
                    lType = lType.Remove(lType.IndexOf('|'));
                    String lValue = line.Remove(0, line.IndexOf('|') + 1);
                    lValue = lValue.Remove(lValue.IndexOf(']'));

                    return [lValue, lType];
                }
            }
        }

        return [null!, null!];
    }
}
