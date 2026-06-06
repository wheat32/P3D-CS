namespace P3D.ScriptVersion2
{
    // TODO Phase 4: full ScriptLibrary port
    public static class ScriptLibrary
    {
        public static void InitializeLibrary() { }
        public static String GetHelpContent(String subClass, int maxLines) => "";
    }

    // ScriptComparer base — partial class; construct/comparison routing stubs for
    // ScriptConstructs/** (DoXxx methods) are in ScriptComparer.cs when ported.
    public static partial class ScriptComparer
    {
        public static readonly String DefaultNull = "\0";

        public static Object EvaluateConstruct(Object construct)
        {
            if (construct is String s)
            {
                if (s.Equals("") == true)
                {
                    return "";
                }

                String output = "";
                String input = s;

                bool foundNOT = false;

                while (input.Length > 0)
                {
                    char c = input[0];
                    int endIndex = 0;

                    if (c == '<')
                    {
                        int level = 0;
                        input = input.Remove(0, 1);

                        for (int i = 0; i <= input.Length - 1; i++)
                        {
                            if (input[i] == '<')
                            {
                                level += 1;
                            }
                            if (input[i] == '>')
                            {
                                if (level > 0)
                                {
                                    level -= 1;
                                }
                                else
                                {
                                    endIndex = i;
                                    break;
                                }
                            }
                        }

                        String arg = input.Substring(0, endIndex);
                        input = input.Remove(0, endIndex + 1);

                        String classValue = arg;

                        if (classValue.StartsWith("not ") == true)
                        {
                            classValue = classValue.Remove(0, 4);
                            foundNOT = true;
                        }

                        String mainClass = classValue.Remove(classValue.IndexOf('.'));
                        String subClass = classValue.Remove(0, classValue.IndexOf('.') + 1);

                        Object resultValue = GetConstructReturnValue(mainClass, subClass);

                        if (resultValue.Equals(DefaultNull) == true)
                        {
                            Logger.Log(Logger.LogTypes.Warning, $"No value was returned from a construct. mainclass: {mainClass}; subclass: {subClass}");
                            resultValue = arg;
                        }

                        if (foundNOT == true)
                        {
                            String[] bools = ["false", "true"];
                            if (bools.Contains(resultValue.ToString()?.ToLower()) == true)
                            {
                                switch (resultValue.ToString()?.ToLower())
                                {
                                    case "false":
                                        resultValue = "true";
                                        break;
                                    case "true":
                                        resultValue = "false";
                                        break;
                                }
                            }
                        }
                        foundNOT = false;

                        input = resultValue.ToString() + input;
                    }
                    else
                    {
                        output += input[0];
                        input = input.Remove(0, 1);
                    }
                }

                return output;
            }
            return construct;
        }

        public static bool EvaluateScriptComparison(String inputString)
        {
            return EvaluateScriptComparison(inputString, false);
        }

        public static bool EvaluateScriptComparison(String inputString, bool caseSensitive)
        {
            String comparer = "=";

            int countStarts = 0;
            foreach (char c in inputString)
            {
                if (c == '<')
                {
                    countStarts += 1;
                }
                else if (c == '>')
                {
                    countStarts -= 1;
                }
            }

            String setComparer;
            if (countStarts < 0)
            {
                setComparer = ">";
            }
            else if (countStarts > 0)
            {
                setComparer = "<";
            }
            else
            {
                setComparer = "=";
            }

            int comparerIndex = inputString.IndexOf('=');
            int i2 = 0;

            if (setComparer.Equals(">") == true)
            {
                int level = 0;
                foreach (char c in inputString)
                {
                    if (c == '<')
                    {
                        level += 1;
                    }
                    else if (c == '>')
                    {
                        level -= 1;
                        if (level == -1)
                        {
                            comparerIndex = i2;
                            break;
                        }
                    }
                    i2 += 1;
                }
            }
            else if (setComparer.Equals("<") == true)
            {
                List<int> started2 = [];
                foreach (char c in inputString)
                {
                    if (c == '<')
                    {
                        started2.Add(i2);
                    }
                    else if (c == '>')
                    {
                        started2.RemoveAt(started2.Count - 1);
                    }
                    i2 += 1;
                }
                comparerIndex = started2[0];
            }

            if (setComparer.Equals("") == false)
            {
                comparer = setComparer;
            }
            else
            {
                comparerIndex = inputString.IndexOf('=');
            }

            Object compareValue = inputString.Substring(comparerIndex + 1);
            String classValue = inputString.Substring(0, comparerIndex);

            Object resultValue = EvaluateConstruct(classValue);
            compareValue = EvaluateConstruct(compareValue.ToString() ?? "");

            bool comparisonResult = false;

            switch (comparer)
            {
                case "=":
                    if (caseSensitive == true || resultValue is not String || compareValue is not String)
                    {
                        if (resultValue.Equals(compareValue) == true)
                        {
                            comparisonResult = true;
                        }
                    }
                    else
                    {
                        if (ScriptConversion.IsBoolean(resultValue.ToString() ?? "") == true &&
                            ScriptConversion.IsBoolean(compareValue.ToString() ?? "") == true)
                        {
                            if (ScriptConversion.ToBoolean(resultValue) == ScriptConversion.ToBoolean(compareValue))
                            {
                                comparisonResult = true;
                            }
                        }
                        else
                        {
                            if ((resultValue.ToString() ?? "").ToLower().Equals((compareValue.ToString() ?? "").ToLower()) == true)
                            {
                                comparisonResult = true;
                            }
                        }
                    }
                    break;
                case ">":
                    if (StringHelper.IsNumeric(resultValue) == true && StringHelper.IsNumeric(compareValue) == true)
                    {
                        if (ScriptConversion.ToDouble(resultValue) > ScriptConversion.ToDouble(compareValue))
                        {
                            comparisonResult = true;
                        }
                    }
                    break;
                case "<":
                    if (StringHelper.IsNumeric(resultValue) == true && StringHelper.IsNumeric(compareValue) == true)
                    {
                        if (ScriptConversion.ToDouble(resultValue) < ScriptConversion.ToDouble(compareValue))
                        {
                            comparisonResult = true;
                        }
                    }
                    break;
            }

            return comparisonResult;
        }

        public static PairValue GetSubClassArgumentPair(String inputString)
        {
            PairValue p = new PairValue();

            String command = inputString;
            String argument = "";

            if (command.Contains("(") == true && command.EndsWith(")") == true)
            {
                argument = command.Remove(0, command.IndexOf('(') + 1);
                argument = argument.Remove(argument.Length - 1, 1);
                command = command.Remove(command.IndexOf('('));
            }

            argument = EvaluateConstruct(argument).ToString() ?? "";

            p.Command = command;
            p.Argument = argument;

            return p;
        }

        public static String ReturnBoolean(bool value)
        {
            if (value == true)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        private static Object GetConstructReturnValue(String mainClass, String subClass)
        {
            switch (mainClass.ToLower())
            {
                case "pokemon": return DoPokemon(subClass);
                case "overworldpokemon": return DoOverworldPokemon(subClass);
                case "player": return DoPlayer(subClass);
                case "environment": return DoEnvironment(subClass);
                case "register": return DoRegister(subClass);
                case "system": return DoSystem(subClass);
                case "npc": return DoNPC(subClass);
                case "inventory": return DoInventory(subClass);
                case "storage": return DoStorage(subClass);
                case "phone": return DoPhone(subClass);
                case "entity": return DoEntity(subClass);
                case "level": return DoLevel(subClass);
                case "battle": return DoBattle(subClass);
                case "daycare": return DoDaycare(subClass);
                case "rival": return DoRival(subClass);
                case "math": return DoMath(subClass);
                case "pokedex": return DoPokedex(subClass);
                case "radio": return DoRadio(subClass);
                case "camera": return DoCamera(subClass);
                case "filesystem": return DoFileSystem(subClass);
                case "screen": return DoScreen(subClass);
                case "script": return DoScript(subClass);
            }
            return DefaultNull;
        }

        public struct PairValue
        {
            public String Command;
            public String Argument;
        }
    }

    // ScriptCommander base — routes @command.sub(arg) to the right DoXxx handler.
    // DoXxx methods live in ScriptCommands/** (ported in a later phase).
    public static partial class ScriptCommander
    {
        private static ScriptV2? _scriptV2;
        public static String Value = "";

        private static bool IsReady
        {
            get => _scriptV2 != null ? _scriptV2.IsReady : true;
            set { if (_scriptV2 != null) { _scriptV2.IsReady = value; } }
        }

        private static bool Started
        {
            get => _scriptV2 != null ? _scriptV2.started : false;
            set { if (_scriptV2 != null) { _scriptV2.started = value; } }
        }

        private static bool CanContinue
        {
            get => _scriptV2 != null ? _scriptV2.CanContinue : true;
            set { if (_scriptV2 != null) { _scriptV2.CanContinue = value; } }
        }

        public static Object Parse(String input)
        {
            return ScriptComparer.EvaluateConstruct(input);
        }

        public static void ExecuteCommand(ScriptV2 scriptV2, String inputString)
        {
            _scriptV2 = scriptV2;

            String classValue = inputString;

            String mainClass = classValue;
            String subClass = "";

            int bIndex = classValue.Contains("(") == true ? classValue.IndexOf('(') : -1;
            int pIndex = classValue.Contains(".") == true ? classValue.IndexOf('.') : -1;

            if (pIndex > -1 && (pIndex < bIndex || bIndex == -1) == true)
            {
                mainClass = classValue.Remove(classValue.IndexOf('.'));
                subClass = classValue.Remove(0, classValue.IndexOf('.') + 1);
            }
            else
            {
                if (classValue.Contains("(") == true)
                {
                    mainClass = classValue.Remove(classValue.IndexOf('('));
                    subClass = classValue.Remove(0, classValue.IndexOf('(') + 1);
                }
            }

            switch (mainClass.ToLower())
            {
                case "register": DoRegister(subClass); break;
                case "script": DoScript(subClass); break;
                case "screen": DoScreen(subClass); break;
                case "player":
                    if (InsertSpin(inputString) == false)
                    {
                        DoPlayer(subClass);
                    }
                    break;
                case "music": DoMusic(subClass); break;
                case "sound": DoSound(subClass); break;
                case "entity":
                    if (InsertSpin(inputString) == false)
                    {
                        DoEntity(subClass);
                    }
                    break;
                case "battle": DoBattle(subClass); break;
                case "pokemon": DoPokemon(subClass); break;
                case "overworldpokemon": DoOverworldPokemon(subClass); break;
                case "environment": DoEnvironment(subClass); break;
                case "text":
                    if (InsertSpin(inputString) == false)
                    {
                        DoText(subClass);
                    }
                    break;
                case "options":
                    if (InsertSpin(inputString) == false)
                    {
                        DoOptions(subClass);
                    }
                    break;
                case "level": DoLevel(subClass); break;
                case "camera":
                    if (InsertSpin(inputString) == false)
                    {
                        DoCamera(subClass);
                    }
                    break;
                case "item": DoItem(subClass); break;
                case "storage": DoStorage(subClass); break;
                case "npc":
                    if (InsertSpin(inputString) == false)
                    {
                        DoNPC(subClass);
                    }
                    break;
                case "chat": DoChat(subClass); break;
                case "daycare": DoDayCare(subClass); break;
                case "pokedex": DoPokedex(subClass); break;
                case "radio": DoRadio(subClass); break;
                case "help": DoHelp(subClass); break;
                case "system": DoSystem(subClass); break;
                case "title": DoTitle(subClass); break;
                default:
                    Logger.Log(Logger.LogTypes.Message, "ScriptCommander.cs: This class (" + mainClass + ") doesn't exist.");
                    IsReady = true;
                    break;
            }
        }

        private static bool InsertSpin(String inputString)
        {
            if (ActionScript.TempSpin == true)
            {
                if (ActionScript.TempInputDirection > -1)
                {
                    if (inputString.ToLower().StartsWith("player.turnto(") == false)
                    {
                        if (P3D.Screen.Camera.GetPlayerFacingDirection() != ActionScript.TempInputDirection)
                        {
                            if (((OverworldCamera)P3D.Screen.Camera).ThirdPerson == false)
                            {
                                ((OverworldScreen)Core.CurrentScreen).ActionScript.Scripts.Insert(
                                    0, new Script("@player.turnto(" + ActionScript.TempInputDirection + ")", ActionScript.ScriptLevelIndex));
                                return true;
                            }
                        }
                    }
                    ActionScript.TempInputDirection = -1;
                    ActionScript.TempSpin = false;
                }
            }
            return false;
        }

        private static void DoHelp(String subClass)
        {
            if (subClass.EndsWith(")") == true)
            {
                subClass = subClass.Remove(subClass.Length - 1, 1);
            }
            Chat.AddLine(new Chat.ChatMessage("[HELP]", ScriptLibrary.GetHelpContent(subClass, 20), "0", Chat.ChatMessage.MessageTypes.CommandMessage));
            IsReady = true;
        }

        private static int Int(Object expression) => ScriptConversion.ToInteger(expression);
        private static float Sng(Object expression) => ScriptConversion.ToSingle(expression);
        private static double Dbl(Object expression) => ScriptConversion.ToDouble(expression);
    }
}
