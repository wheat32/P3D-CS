using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class SuperPower : Attack
{
    public SuperPower()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 276;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 120;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Superpower");
        Description = "The user attacks the target with great power. However, it also lowers the user's Attack and Defense.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Attack", 1, "", "move:superpower");
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", 1, "", "move:superpower");
    }

}
