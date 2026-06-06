using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class Bonemerang : Attack
{
    public Bonemerang()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 155;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 50;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Bonemerang");
        Description = "The user throws the bone it holds. The bone loops to hit the target twice, coming and going.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 2;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;
        disabledWhileGravity = false;
        useEffectiveness = true;
        isHealingMove = false;
        removesSelfFrozen = false;
        isRecoilMove = false;

        immunityAffected = true;
        isDamagingMove = true;
        isProtectMove = false;

        hasSecondaryEffect = false;
        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

}
