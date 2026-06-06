using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class WaterShuriken : Attack
{
    public WaterShuriken()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 594;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 15;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Water Shuriken");
        Description = "The user hits the target with throwing stars two to five times in a row. This move always goes first.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 1;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
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
        aiField2 = AIField.HighPriority;
    }

    public override int GetTimesToAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (p.Ability.Name.ToLower() == "skill link")
        {
            return 5;
        }

        int r = Core.Random.Next(0, 100);
        if (r < 37)
        {
            return 2;
        }
        else if (r >= 37 && r < 75)
        {
            return 3;
        }
        else if (r >= 75 && r < 88)
        {
            return 4;
        }
        else if (r >= 88)
        {
            return 5;
        }

        return 2;
    }

}
