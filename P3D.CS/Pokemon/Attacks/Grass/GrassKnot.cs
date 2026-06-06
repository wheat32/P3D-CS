using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class GrassKnot : Attack
{
    public GrassKnot()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 447;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Grass Knot");
        Description = "The user snares the target with grass and trips it. The heavier the target, the greater the move's power.";
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
