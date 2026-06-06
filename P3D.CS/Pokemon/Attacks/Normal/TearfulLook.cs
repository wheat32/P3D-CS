using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class TearfulLook : Attack
{
    public TearfulLook()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 715;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Tearful Look");
        Description = "The user gets teary eyed to make the target lose its combative spirit. This lowers the target's Attack and Sp. Atk. stats.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = true;
        snatchAffected = false;
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.LowerAttack;
        aiField2 = AIField.LowerSpAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Attack", 1, "", "move:tearfullook");
        battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Attack", 1, "", "move:tearfullook");
    }

}
