using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class HeatCrash : Attack
{
    public HeatCrash()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 535;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Heat Crash");
        Description = "The user slams its target with its flame-covered body. The more the user outweighs the target, the greater the move's power.";
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
        int minimize = battleScreen.FieldEffects.Minimize.Opponent;
        if (own == false)
        {
            minimize = battleScreen.FieldEffects.Minimize.Self;
        }

        int MinimizeMultiplier = 1;

        if (minimize > 0)
        {
            MinimizeMultiplier = 2;
        }

        float userWeight = battleScreen.FieldEffects.GetPokemonWeight(own, battleScreen);
        float targetWeight = battleScreen.FieldEffects.GetPokemonWeight(own == false, battleScreen);

        if (targetWeight <= (1 / 5) * userWeight)
        {
            return 120 * MinimizeMultiplier;
        }

        if (targetWeight > (1 / 5) * userWeight && targetWeight <= (1 / 4) * userWeight)
        {
            return 100 * MinimizeMultiplier;
        }

        if (targetWeight > (1 / 4) * userWeight && targetWeight <= (1 / 3) * userWeight)
        {
            return 80 * MinimizeMultiplier;
        }

        if (targetWeight > (1 / 3) * userWeight && targetWeight <= (1 / 2) * userWeight)
        {
            return 60 * MinimizeMultiplier;
        }

        if (targetWeight > (1 / 2) * userWeight)
        {
            return 40 * MinimizeMultiplier;
        }

        return 40 * MinimizeMultiplier;
    }

}
