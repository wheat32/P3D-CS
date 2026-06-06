using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class CloseCombat : Attack
{
    public CloseCombat()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 370;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 120;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Close Combat");
        Description = "The user fights the target up close without guarding itself. It also cuts the user's Defense and Sp. Def.";
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
        kingsrockAffected = true;
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
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", 1, "", "move:closecombat");
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Special Defense", 1, "", "move:closecombat");
    }

}
