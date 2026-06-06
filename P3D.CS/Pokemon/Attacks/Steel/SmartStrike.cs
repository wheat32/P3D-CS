using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class SmartStrike : Attack
{
    public SmartStrike()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 684;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 70;
        Accuracy = 0;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Smart Strike");
        Description = "The user stabs the target with a sharp horn. This attack never misses.";
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
        useAccEvasion = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CannotMiss;
    }

}
