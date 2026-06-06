using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class Rollout : Attack
{
    public Rollout()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 205;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 30;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Rollout");
        Description = "The user continually rolls into the target over five turns. It becomes stronger each time it hits.";
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

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int rollout = battleScreen.FieldEffects.RolloutCounter.Self;
        if (own == false)
        {
            rollout = battleScreen.FieldEffects.RolloutCounter.Opponent;
        }

        int p = Power;

        if (rollout > 0)
        {
            for (int i = 1; i <= rollout; i++)
            {
                p *= 2;
            }
        }

        int defensecurl = battleScreen.FieldEffects.DefenseCurl.Self;
        if (own == false)
        {
            defensecurl = battleScreen.FieldEffects.DefenseCurl.Opponent;
        }

        if (defensecurl > 0)
        {
            p *= 2;
        }

        return p;
    }

    public override void MoveMultiTurn(bool own, BattleScreen battleScreen)
    {
        int rollout = battleScreen.FieldEffects.RolloutCounter.Self;
        if (own == false)
        {
            rollout = battleScreen.FieldEffects.RolloutCounter.Opponent;
        }

        if (rollout == 4)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.RolloutCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.RolloutCounter.Opponent = 0;
            }
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.RolloutCounter.Self += 1;
            }
            else
            {
                battleScreen.FieldEffects.RolloutCounter.Opponent += 1;
            }
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int rollout = battleScreen.FieldEffects.RolloutCounter.Self;
        if (own == false)
        {
            rollout = battleScreen.FieldEffects.RolloutCounter.Opponent;
        }

        if (rollout > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Interruption(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.RolloutCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.RolloutCounter.Opponent = 0;
        }
    }

    public override void MoveHasNoEffect(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

}
