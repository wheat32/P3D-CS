using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class MagicalLeaf : Attack
{
    public MagicalLeaf()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 345;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 60;
        Accuracy = 0;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Magical Leaf");
        Description = "The user scatters curious leaves that chase the target. This attack never misses.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
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
        counterAffected = false;

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
