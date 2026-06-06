using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class BulletSeed : Attack
{
    public BulletSeed()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 331;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 25;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Bullet Seed");
        Description = "The user forcefully shoots seeds at the target. Two to five seeds are shot in rapid succession.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
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
        isBulletMove = true;
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
