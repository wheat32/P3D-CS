using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class EchoedVoice : Attack
{
    public EchoedVoice()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 497;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 40;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Echoed Voice");
        Description = "The user attacks the target with an echoing voice. If this move is used every turn, its power is increased.";
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

        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int echoed = battleScreen.FieldEffects.EchoedVoice.Self;
        if (own == false)
        {
            echoed = battleScreen.FieldEffects.EchoedVoice.Opponent;
        }

        int p = Power;

        if (echoed > 0)
        {
            for (int i = 1; i <= echoed; i++)
            {
                p += 40;
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

        if (lastMove.ID != 497)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.EchoedVoice.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.EchoedVoice.Opponent = 0;
            }
        }
        else
        {
            int echoed = battleScreen.FieldEffects.EchoedVoice.Self;
            if (own == false)
            {
                echoed = battleScreen.FieldEffects.EchoedVoice.Opponent;
            }

            if (echoed < 4)
            {
                if (own == true)
                {
                    battleScreen.FieldEffects.EchoedVoice.Self += 1;
                }
                else
                {
                    battleScreen.FieldEffects.EchoedVoice.Opponent += 1;
                }
            }
        }
    }

    private void ResetCounter(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.EchoedVoice.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.EchoedVoice.Opponent = 0;
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
