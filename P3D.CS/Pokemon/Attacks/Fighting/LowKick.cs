using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class LowKick : Attack
{
    public LowKick()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 67;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Low Kick");
        Description = "A powerful low kick that makes the target fall over. It inflicts greater damage on heavier targets.";
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
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        float weight = battleScreen.FieldEffects.GetPokemonWeight(own == false, battleScreen);

        if (weight <= 9.9F)
        {
            return 20;
        }
        else if (weight > 9.9F && weight <= 24.9F)
        {
            return 40;
        }
        else if (weight > 24.9F && weight <= 49.9F)
        {
            return 60;
        }
        else if (weight > 49.9F && weight <= 99.9F)
        {
            return 80;
        }
        else if (weight > 99.9F && weight <= 199.9F)
        {
            return 100;
        }
        else
        {
            return 120;
        }
    }

}
