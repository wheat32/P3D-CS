using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class FurySwipes : Attack
{
    public FurySwipes()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 154;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 18;
        Accuracy = 80;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Fury Swipes");
        Description = "The target is raked with sharp claws or scythes for two to five times in quick succession.";
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
