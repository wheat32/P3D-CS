using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class ElectroBall : Attack
{
    public ElectroBall()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 486;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Electro Ball");
        Description = "The user hurls an electric orb at the target. The faster the user is than the target, the greater the move's power.";
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

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int p_Speed = BattleCalculation.DetermineBattleSpeed(own, battleScreen);
        int op_Speed = BattleCalculation.DetermineBattleSpeed(own == false, battleScreen);

        int ratio = (int)(Math.Ceiling((double)(100 * (op_Speed / p_Speed))));

        if (ratio > 50)
        {
            return 60;
        }
        else
        {
            if (ratio > 34)
            {
                return 80;
            }
            else
            {
                if (ratio > 25)
                {
                    return 120;
                }
                else
                {
                    return 150;
                }
            }
        }

    }

}
