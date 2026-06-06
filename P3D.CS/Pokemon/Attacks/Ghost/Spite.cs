using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class Spite : Attack
{
    public Spite()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 180;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Spite");
        Description = "The user unleashes its grudge on the move last used by the target by cutting 4 PP from it.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = false;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Attack lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Self;
        }

        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        if (lastMove != null)
        {
            if (lastMove.currentPP > 0)
            {
                int reduce = 4;
                if (lastMove.currentPP - reduce < 0)
                {
                    reduce = lastMove.currentPP;
                }

                lastMove.currentPP -= reduce;

                battleScreen.BattleQuery.Add(new TextQueryObject("It reduced the PP of the " + op.GetDisplayName() + "'s " + lastMove.Name + " by " + reduce + "!"));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
