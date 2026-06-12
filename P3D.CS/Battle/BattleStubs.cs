using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

// ---------------------------------------------------------------------------
// TODO Phase 5: replace all stubs below with fully ported implementations
// ---------------------------------------------------------------------------

// ---- Battle step stubs (used by moves like Encore that check what the opponent just did) ----
public class BattleStepTypes
{
    // Static access: BattleStepTypes.Move
    public static readonly String Move = "Move";
    public static readonly String Switch = "Switch";
    public static readonly String Other = "Other";

    // Instance access aliases (VB used instance.StepTypes.Move pattern)
    public String MoveStep => Move;
    public String SwitchStep => Switch;
    public String OtherStep => Other;
}

public class BattleRoundConst
{
    public static class StepTypes
    {
        public static readonly String Move = "Move";
        public static readonly String Switch = "Switch";
        public static readonly String Item = "Item";
        public static readonly String Other = "Other";
        public static readonly String Flee = "Flee";
        public static readonly String Text = "Text";
    }

    public String StepType = StepTypes.Move;
    public Object? Argument;
}

public class BattleStep
{
    public String StepType = BattleStepTypes.Move;
    public Object? Argument;
}

// BattleMenu is fully ported in Battle/BattleMenu.cs
// BattleScreen is fully ported in Battle/BattleScreen.cs

// ---- BattleCatchScreen ----
public class BattleCatchScreen : Screen
{
    public BattleCatchScreen(BattleScreen battleScreen, Items.Item ball)
    {
        PreScreen = battleScreen;
        Identification = Identifications.BattleCatchScreen;
    }
}

// BattleCalculation is fully ported in Battle/BattleCalculation.cs

// ---- AttackSpecialBasePower ----
public static class AttackSpecialBasePower
{
    public static int GetGameModeBasePower(Attack attack, bool own,
                                            BattleScreen battleScreen) => attack.Power;
}

// ---- AttackSpecialFunctions ----
public static class AttackSpecialFunctions
{
    public static void ExecuteMoveHitsFunction(Attack attack, bool own,
                                                BattleScreen battleScreen) { }
}

// AnimationQueryObject is fully ported in Battle/BattleAnimations/BattleAnimations.cs

// ---- Battle statistics tracker (mirrors VB NetworkPlayerStatistics) ----

public class BattleStatistics
{
    public int Critical;
    public int SuperEffective;
    public int NotVeryEffective;
    public int NoEffect;
    public int Turns;
    public int Switches;
    public int Moves;

    public override String ToString()
    {
        return "{" + Critical + "|" + SuperEffective + "|" + NotVeryEffective + "|"
               + NoEffect + "|" + Turns + "|" + Switches + "|" + Moves + "}";
    }

    public void FromString(String s)
    {
        s = s.Remove(s.Length - 1, 1).Remove(0, 1);
        String[] data = s.Split('|');
        Critical = int.Parse(data[0]);
        SuperEffective = int.Parse(data[1]);
        NotVeryEffective = int.Parse(data[2]);
        NoEffect = int.Parse(data[3]);
        Turns = int.Parse(data[4]);
        Switches = int.Parse(data[5]);
        Moves = int.Parse(data[6]);
    }
}

// ---- Player statistics (global tracker) ----

public static class PlayerStatistics
{
    public static void Track(String key, int amount) { }
    public static void Track(String key, double amount) { }
}

