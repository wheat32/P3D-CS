using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Cut : Attack
{
    public Cut()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 15;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 50;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Cut");
        Description = "The target is cut with a scythe or a claw. It can also be used to cut down thin trees.";
        criticalChance = 1;
        isHMMove = true;
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isSlicingMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

}
