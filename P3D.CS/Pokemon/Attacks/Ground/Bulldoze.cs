using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class Bulldoze : Attack
{
    public Bulldoze()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 523;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 60;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Bulldoze");
        Description = "The user strikes everything around it by stomping down on the ground. This lowers the Speed stat of those hit.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        canHitUnderground = true;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Speed", 1, "", "move:bulldoze");
    }

}
