using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class FuryCutter : Attack
{
    public FuryCutter()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 210;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 40;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Fury Cutter");
        Description = "The target is slashed with scythes or claws. Its power increases if it hits in succession.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isSlicingMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int fury = battleScreen.FieldEffects.FuryCutter.Self;
        if (own == false)
        {
            fury = battleScreen.FieldEffects.FuryCutter.Opponent;
        }

        int p = Power;

        if (fury > 0)
        {
            for (int i = 1; i <= fury; i++)
            {
                p *= 2;
            }
        }

        return p;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        }

        if (lastMove.ID != 210)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.FuryCutter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.FuryCutter.Opponent = 0;
            }
        }
        else
        {
            int fury = battleScreen.FieldEffects.FuryCutter.Self;
            if (own == false)
            {
                fury = battleScreen.FieldEffects.FuryCutter.Opponent;
            }

            if (fury < 4)
            {
                if (own == true)
                {
                    battleScreen.FieldEffects.FuryCutter.Self += 1;
                }
                else
                {
                    battleScreen.FieldEffects.FuryCutter.Opponent += 1;
                }
            }
        }
    }

    private void ResetCounter(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.FuryCutter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.FuryCutter.Opponent = 0;
        }
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void MoveHasNoEffect(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        ResetCounter(own, battleScreen);
    }

}
