using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class CometPunch : Attack
{
    public CometPunch()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 4;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 18;
        Accuracy = 85;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Comet Punch");
        Description = "The target is hit with a flurry of punches that strike two to five times in a row.";
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
        isPunchingMove = true;
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
