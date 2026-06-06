using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class IceBall : Attack
{
    public IceBall()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 301;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 30;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Ice Ball");
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

        isBulletMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int iceball = battleScreen.FieldEffects.IceBallCounter.Self;
        if (own == false)
        {
            iceball = battleScreen.FieldEffects.IceBallCounter.Opponent;
        }

        int p = Power;

        if (iceball > 0)
        {
            for (int i = 1; i <= iceball; i++)
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
        int iceball = battleScreen.FieldEffects.IceBallCounter.Self;
        if (own == false)
        {
            iceball = battleScreen.FieldEffects.IceBallCounter.Opponent;
        }

        if (iceball == 5)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.IceBallCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.IceBallCounter.Opponent = 0;
            }
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.IceBallCounter.Self += 1;
            }
            else
            {
                battleScreen.FieldEffects.IceBallCounter.Opponent += 1;
            }
        }
    }

    private void Interruption(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.IceBallCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.IceBallCounter.Opponent = 0;
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
