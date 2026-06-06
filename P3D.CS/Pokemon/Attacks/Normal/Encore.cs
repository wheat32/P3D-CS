using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Encore : Attack
{
    public Encore()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 227;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Encore");
        Description = "The user compels the target to keep using only the move it last used for three turns.";
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
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        Attack lastMove = null;

        if (own == true)
        {
            if ((battleScreen.FieldEffects.LastMove.Opponent == null && Battle.OpponentStep.StepType == Battle.RoundConst.StepTypes.Move) || Battle.OpponentStep.StepType == Battle.RoundConst.StepTypes.Move && (Attack)Battle.OpponentStep.Argument.ID != battleScreen.FieldEffects.LastMove.Opponent.ID)
            {
                lastMove = (Attack)Battle.OpponentStep.Argument;
            }
            else
            {
                lastMove = battleScreen.FieldEffects.LastMove.Opponent;
            }
        }
        else
        {
            if ((battleScreen.FieldEffects.LastMove.Self == null && Battle.SelfStep.StepType == Battle.RoundConst.StepTypes.Move) || Battle.SelfStep.StepType == Battle.RoundConst.StepTypes.Move && (Attack)Battle.SelfStep.Argument.ID != battleScreen.FieldEffects.LastMove.Self.ID)
            {
                lastMove = (Attack)Battle.SelfStep.Argument;
            }
            else
            {
                lastMove = battleScreen.FieldEffects.LastMove.Self;
            }
        }

        if (lastMove != null)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.EncoreMove.Opponent = lastMove;
            }
            else
            {
                battleScreen.FieldEffects.EncoreMove.Self = lastMove;
            }
            if (own == true)
            {
                battleScreen.FieldEffects.Encore.Opponent = 3;
            }
            else
            {
                battleScreen.FieldEffects.Encore.Self = 3;
            }
            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName + " received an encore!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
