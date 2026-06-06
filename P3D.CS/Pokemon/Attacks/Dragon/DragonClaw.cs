using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dragon;

public class DragonClaw : Attack
{
    public DragonClaw()
    {
        // #Definitions
        type = new Element(Element.Types.Dragon);
        ID = 337;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 80;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Dragon Claw");
        Description = "The user slashes the target with huge, sharp claws.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;
    }

}
