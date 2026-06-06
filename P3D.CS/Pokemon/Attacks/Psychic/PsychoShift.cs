using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class PsychoShift : Attack
{
    public PsychoShift()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 375;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Psycho Shift");
        Description = "Using its psychic power of suggestion, the user transfers its status conditions to the target.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = true;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool fails = false;
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon o = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            o = battleScreen.SelfPokemon;
        }
        Pokemon.StatusProblems status = p.Status;

        if (o.Status != Pokemon.StatusProblems.None)
        {
            fails = true;
        }

        switch (p.Status)
        {
            case Pokemon.StatusProblems.Poison:
                fails = battleScreen.Battle.InflictPoison(own == false, own, battleScreen, false, "", "move:psychoshift") == false;
                break;
            case Pokemon.StatusProblems.BadPoison:
                fails = battleScreen.Battle.InflictPoison(own == false, own, battleScreen, true, "", "move:psychoshift") == false;
                break;
            case Pokemon.StatusProblems.Sleep:
                fails = battleScreen.Battle.InflictSleep(own == false, own, battleScreen, -1, "", "move:psychoshift") == false;
                break;
            case Pokemon.StatusProblems.Paralyzed:
                fails = battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:psychoshift") == false;
                break;
            case Pokemon.StatusProblems.Freeze:
                fails = battleScreen.Battle.InflictFreeze(own == false, own, battleScreen, "", "move:psychoshift") == false;
                break;
            case Pokemon.StatusProblems.Burn:
                fails = battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:psychoshift") == false;
                break;
            default:
                fails = true;
                break;
        }

        if (fails == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            battleScreen.Battle.CureStatusProblem(own, own, battleScreen, "", "move:psychoshift");
        }


    }

}
