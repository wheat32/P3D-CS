using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Refresh : Attack
{
    public Refresh()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 287;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Refresh");
        Description = "The user rests to cure itself of a poisoning, burn, or paralysis.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = true;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Healing;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Poison || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.BadPoison || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Burn || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Paralyzed)
            {
                battleScreen.Battle.CureStatusProblem(own, own, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " was cured.", "move:refresh");
            }
        }
        else
        {
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Poison || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.BadPoison || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Burn || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Paralyzed)
            {
                battleScreen.Battle.CureStatusProblem(own, own, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " was cured.", "move:refresh");
            }
        }

    }

}
