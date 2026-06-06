using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class HeavySlam : Attack
{
    public HeavySlam()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 484;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Heavy Slam");
        Description = "The user slams into the target with its heavy body. The more the user outweighs the target, the greater its damage.";
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
        float userWeight = battleScreen.FieldEffects.GetPokemonWeight(own, battleScreen);
        float targetWeight = battleScreen.FieldEffects.GetPokemonWeight(own == false, battleScreen);

        if (targetWeight <= (1 / 5) * userWeight)
        {
            return 120;
        }

        if (targetWeight > (1 / 5) * userWeight && targetWeight <= (1 / 4) * userWeight)
        {
            return 100;
        }

        if (targetWeight > (1 / 4) * userWeight && targetWeight <= (1 / 3) * userWeight)
        {
            return 80;
        }

        if (targetWeight > (1 / 3) * userWeight && targetWeight <= (1 / 2) * userWeight)
        {
            return 60;
        }

        if (targetWeight > (1 / 2) * userWeight)
        {
            return 40;
        }

        return 40;
    }

}
