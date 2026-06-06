using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class ShellSmash : Attack
{
    public ShellSmash()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 504;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Shell Smash");
        Description = "The user breaks its shell, lowering its Defense and Sp. Def. stats but sharply raising Attack, Sp. Atk, and Speed stats.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = true;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.RaiseAttack;
        aiField2 = AIField.RaiseSpAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", 1, "", "move:shellsmash");
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Special Defense", 1, "", "move:shellsmash");
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", 2, "", "move:shellsmash");
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", 2, "", "move:shellsmash");
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Speed", 2, "", "move:shellsmash");
    }

}
