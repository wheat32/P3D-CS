using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class WakeUpSlap : Attack
{
    public WakeUpSlap()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 358;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 70;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Wake-Up Slap");
        Description = "This attack inflicts big damage on a sleeping target. It also wakes the target up, however.";
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

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        if (op.Status == Pokemon.StatusProblems.Sleep)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Sleep)
            {
                battleScreen.Battle.CureStatusProblem(own == false, own, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " was cured of sleep.", "move:wakeupslap");
            }
        }
        else
        {
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Sleep)
            {
                battleScreen.Battle.CureStatusProblem(own == false, own, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " was cured of sleep.", "move:wakeupslap");
            }
        }
    }

}
