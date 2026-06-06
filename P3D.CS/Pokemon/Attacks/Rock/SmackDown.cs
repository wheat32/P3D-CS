using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class SmackDown : Attack
{
    public SmackDown()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 479;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 50;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Smack Down");
        Description = "The user throws a stone or similar projectile to attack an opponent. A flying Pokémon will fall to the ground when it's hit.";
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
