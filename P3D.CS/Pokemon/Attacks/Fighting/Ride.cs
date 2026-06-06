using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class Ride : Attack
{
    public Ride()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 560;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 60;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Ride");
        Description = "The user runs over the target rapidly. The power rises when the user's accuracy stat is low.";
        criticalChance = 1;
        isHMMove = true;
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

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int accLevel = p.Accuracy;
        if (accLevel >= 0)
        {
            return Power;
        }
        else
        {
            switch (accLevel)
            {
                case -1:
                    return 75;
                    break;
                case -2:
                    return 90;
                    break;
                case -3:
                    return 105;
                    break;
                case -4:
                    return 120;
                    break;
                case -5:
                    return 135;
                    break;
                case -6:
                    return 150;
                    break;
            }
        }

        return Power;
    }

}
