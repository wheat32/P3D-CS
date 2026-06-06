using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class ThousandArrows : Attack
{
    public ThousandArrows()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 614;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 90;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Thousand Arrows");
        Description = "This move also hits opposing Pokémon that are in the air. Those Pokémon are knocked down to the ground.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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
        canHitInMidAir = true;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        if (own == true)
        {
            if (battleScreen.FieldEffects.Smacked.Opponent == 0)
            {
                battleScreen.FieldEffects.Smacked.Opponent = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " fell straight down."));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Smacked.Self == 0)
            {
                battleScreen.FieldEffects.Smacked.Self = 1;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " fell straight down."));
            }
        }
    }

}
