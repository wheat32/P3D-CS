using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class DarkestLariat : Attack
{
    public DarkestLariat()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 663;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 85;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Darkest Lariat");
        Description = "The user swings both arms and hits the target. The target's stat changes don't affect this attack's damage.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        useOpponentEvasion = false;
        useOpponentDefense = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;

    }

}
