using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class SparklingAria : Attack
{
    public SparklingAria()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 664;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 90;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Sparkling Aria");
        Description = "The user bursts into song, emitting many bubbles. Any Pokémon suffering from a burn will be healed by the touch of these bubbles.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
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
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Burn)
            {
                battleScreen.Battle.CureStatusProblem(own == false, own, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " was cured of burn.", "move:sparklingaria");
            }
        }
        else
        {
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Burn)
            {
                battleScreen.Battle.CureStatusProblem(own == false, own, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " was cured of burn.", "move:sparklingaria");
            }
        }
    }

}
