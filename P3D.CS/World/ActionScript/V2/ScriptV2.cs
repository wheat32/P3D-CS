namespace P3D
{
    public class ScriptV2
    {
        public enum ScriptTypes : int
        {
            Command = 100,
            @if = 101,
            when = 102,
            then = 103,
            @else = 104,
            endif = 105,
            end = 106,
            select = 107,
            endwhen = 108,
            @return = 109,
            endscript = 110,
            @while = 111,
            endwhile = 112,
            exitwhile = 113,
            Comment = 128,
        }

        public ScriptTypes ScriptType;
        public String Value = "";
        public bool started;
        public bool IsReady;
        public bool CanContinue = true;
        public String RawScriptLine = "";

        public static String TempReturn = "NULL";

        public void Initialize(String scriptLine)
        {
            RawScriptLine = scriptLine;

            String firstChar = scriptLine[0].ToString();
            switch (firstChar)
            {
                case "@":
                    ScriptType = ScriptTypes.Command;
                    Value = scriptLine.Remove(0, 1);
                    break;
                case ":":
                {
                    String structureType = scriptLine.Remove(0, 1);

                    if (structureType.Contains(":") == true)
                    {
                        Value = structureType.Remove(0, structureType.IndexOf(':') + 1);
                        structureType = structureType.Remove(structureType.IndexOf(':'));
                    }

                    switch (structureType.ToLower())
                    {
                        case "if":
                            ScriptType = ScriptTypes.@if;
                            break;
                        case "when":
                            ScriptType = ScriptTypes.when;
                            break;
                        case "then":
                            ScriptType = ScriptTypes.then;
                            break;
                        case "else":
                            ScriptType = ScriptTypes.@else;
                            break;
                        case "endif":
                            ScriptType = ScriptTypes.endif;
                            break;
                        case "end":
                            ScriptType = ScriptTypes.end;
                            break;
                        case "select":
                            ScriptType = ScriptTypes.select;
                            break;
                        case "endwhen":
                            ScriptType = ScriptTypes.endwhen;
                            break;
                        case "return":
                            ScriptType = ScriptTypes.@return;
                            break;
                        case "endscript":
                            ScriptType = ScriptTypes.endscript;
                            break;
                        case "while":
                            ScriptType = ScriptTypes.@while;
                            break;
                        case "endwhile":
                            ScriptType = ScriptTypes.endwhile;
                            break;
                        case "exitwhile":
                            ScriptType = ScriptTypes.exitwhile;
                            break;
                        default:
                            Logger.Log(Logger.LogTypes.Message, "Illegal script line detected (" + scriptLine + ")");
                            IsReady = true;
                            break;
                    }
                    CanContinue = true;
                    break;
                }
                case "#":
                    ScriptType = ScriptTypes.Comment;
                    Value = scriptLine.Remove(0, 1);
                    CanContinue = true;
                    break;
                default:
                    Logger.Log(Logger.LogTypes.Message, "Illegal script line detected (" + scriptLine + ")");
                    IsReady = true;
                    CanContinue = true;
                    break;
            }
        }

        public void EndScript(bool forceEnd)
        {
            ActionScript.ScriptLevelIndex -= 1;
            ActionScript.ScriptTrigger = "";
            if (ActionScript.ScriptLevelIndex == -1 || forceEnd == true)
            {
                ActionScript.ScriptLevelIndex = -1;
                OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                oS.ActionScript.Scripts.Clear();
                oS.ActionScript.reDelay = 1.0f;
                IsReady = true;
                Screen.TextBox.reDelay = 1.0f;
                ActionScript.TempInputDirection = -1;
                ActionScript.TempSpin = false;
            }
        }

        public void Update()
        {
            switch (ScriptType)
            {
                case ScriptTypes.Command:
                    DoCommand();
                    break;
                case ScriptTypes.@if:
                    DoIf();
                    break;
                case ScriptTypes.then:
                    IsReady = true;
                    break;
                case ScriptTypes.@else:
                {
                    OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                    oS.ActionScript.ChooseIf(true);
                    IsReady = true;
                    break;
                }
                case ScriptTypes.endif:
                {
                    OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                    oS.ActionScript.ChooseIf(true);
                    IsReady = true;
                    break;
                }
                case ScriptTypes.@while:
                    DoWhile();
                    break;
                case ScriptTypes.endwhile:
                    IsReady = true;
                    break;
                case ScriptTypes.exitwhile:
                    DoExitWhile();
                    break;
                case ScriptTypes.select:
                    DoSelect();
                    break;
                case ScriptTypes.when:
                {
                    OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                    oS.ActionScript.Switch("");
                    IsReady = true;
                    break;
                }
                case ScriptTypes.endwhen:
                {
                    OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                    oS.ActionScript.Switch("");
                    IsReady = true;
                    break;
                }
                case ScriptTypes.end:
                    EndScript(false);
                    break;
                case ScriptTypes.endscript:
                    EndScript(true);
                    break;
                case ScriptTypes.@return:
                    DoReturn();
                    EndScript(false);
                    break;
                case ScriptTypes.Comment:
                    Logger.Debug("ScriptV2.cs: #Comment: \"" + Value + "\"");
                    IsReady = true;
                    break;
            }
        }

        private void DoWhile()
        {
            bool t = CheckCondition();

            if (t == true)
            {
                ActionScript.CSL().WhileQuery.Clear();
                ActionScript.CSL().WhileQueryInitialized = true;
            }
            else
            {
                ActionScript.CSL().WhileQuery.Clear();
                ActionScript.CSL().WhileQueryInitialized = false;

                OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
                while (oS.ActionScript.Scripts.Count > 0 &&
                       oS.ActionScript.Scripts[0].ScriptV2.ScriptType != ScriptTypes.endwhile)
                {
                    oS.ActionScript.Scripts.RemoveAt(0);
                }
            }

            IsReady = true;
        }

        private void DoExitWhile()
        {
            ActionScript.CSL().WhileQuery.Clear();
            ActionScript.CSL().WhileQueryInitialized = true;

            OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
            while (oS.ActionScript.Scripts.Count > 0 &&
                   oS.ActionScript.Scripts[0].ScriptV2.ScriptType != ScriptTypes.endwhile)
            {
                oS.ActionScript.Scripts.RemoveAt(0);
            }

            IsReady = true;
        }

        private void DoIf()
        {
            bool t = CheckCondition();

            ActionScript.CSL().WaitingEndIf[ActionScript.CSL().IfIndex + 1] = false;
            ActionScript.CSL().CanTriggerElse[ActionScript.CSL().IfIndex + 1] = false;

            OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
            oS.ActionScript.ChooseIf(t);

            IsReady = true;
        }

        private bool CheckCondition()
        {
            String check = Value;
            bool t = false;
            bool convertNextValue = false;

            List<List<String>> ors = [];
            List<String> currentOr = [];

            while (check.Contains(" <and> ") == true || check.Contains(" <or> ") == true)
            {
                if (check.StartsWith(" <and> ") == true)
                {
                    check = check.Remove(0, " <and> ".Length);
                }
                else if (check.StartsWith(" <or> ") == true)
                {
                    List<String> newOr = [..currentOr];
                    ors.Add(newOr);
                    currentOr = [];
                    check = check.Remove(0, " <or> ".Length);
                }
                else
                {
                    if (check.StartsWith("<not><") == true)
                    {
                        convertNextValue = true;
                        check = check.Remove(0, "<not>".Length);
                    }

                    int andStop = check.Contains(" <and> ") == true ? check.IndexOf(" <and> ") : -1;
                    int orStop = check.Contains(" <or> ") == true ? check.IndexOf(" <or> ") : -1;

                    int nextStop;
                    if (andStop > -1 && orStop == -1)
                    {
                        nextStop = andStop;
                    }
                    else if (orStop > -1 && andStop == -1)
                    {
                        nextStop = orStop;
                    }
                    else
                    {
                        nextStop = andStop < orStop ? andStop : orStop;
                    }

                    String newCheck = check.Remove(nextStop);

                    if (convertNextValue == true)
                    {
                        convertNextValue = false;
                        newCheck = "<not>" + newCheck;
                    }

                    currentOr.Add(newCheck);
                    check = check.Remove(0, nextStop);
                }
            }
            currentOr.Add(check);
            ors.Add(currentOr);

            List<bool> results = [];
            foreach (List<String> checkOR in ors)
            {
                bool vT = true;
                foreach (String c in checkOR)
                {
                    String ci = c;
                    bool invertResult = false;
                    if (ci.StartsWith("<not>") == true)
                    {
                        ci = ci.Remove(0, "<not>".Length);
                        invertResult = true;
                    }
                    bool v = ScriptVersion2.ScriptComparer.EvaluateScriptComparison(ci);
                    if (invertResult == true)
                    {
                        v = v == false;
                    }
                    if (v == false)
                    {
                        vT = false;
                        break;
                    }
                }
                results.Add(vT);
            }

            foreach (bool result in results)
            {
                if (result == true)
                {
                    t = true;
                    break;
                }
            }

            return t;
        }

        private void DoSelect()
        {
            ActionScript.CSL().WhenIndex += 1;

            OverworldScreen oS = (OverworldScreen)Core.CurrentScreen;
            oS.ActionScript.Switch(ScriptVersion2.ScriptComparer.EvaluateConstruct(Value));

            IsReady = true;
        }

        private void DoCommand()
        {
            ScriptVersion2.ScriptCommander.ExecuteCommand(this, Value);
        }

        private void DoReturn()
        {
            TempReturn = ScriptVersion2.ScriptComparer.EvaluateConstruct(Value).ToString() ?? "NULL";
        }
    }
}
