using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class DoubleSlap : Attack
{
    public DoubleSlap()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 3;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 15;
        Accuracy = 85;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Double Slap");
        Description = "The target is slapped repeatedly, back and forth, two to five times in a row.";
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
        // #End
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
