using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Bide : Attack
{
    public Bide()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 117;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Bide");
        Description = "The user endures attacks for two turns, then strikes back to cause double the damage taken.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 1;
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
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;

        useAccEvasion = false;
        canHitInMidAir = true;
        canHitSleeping = true;
        canHitUnderground = true;
        canHitUnderwater = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.BideCounter.Self < 3)
            {
                battleScreen.FieldEffects.BideCounter.Self += 1;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.BideCounter.Opponent < 3)
            {
                battleScreen.FieldEffects.BideCounter.Opponent += 1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        if (own == true)
        {
            int damage = battleScreen.FieldEffects.BideDamage.Self * 2;
            battleScreen.FieldEffects.BideDamage.Self = 0;
            battleScreen.FieldEffects.BideCounter.Self = 0;
            return damage;
        }
        else
        {
            int damage = battleScreen.FieldEffects.BideDamage.Opponent * 2;
            battleScreen.FieldEffects.BideDamage.Opponent = 0;
            battleScreen.FieldEffects.BideCounter.Opponent = 0;
            return damage;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.BideCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.BideCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int bide = battleScreen.FieldEffects.BideCounter.Self;
        if (own == false)
        {
            bide = battleScreen.FieldEffects.BideCounter.Opponent;
        }

        if (bide == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MoveFails(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.BideCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.BideCounter.Opponent = 0;
        }
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void AbsorbedBySubstitute(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

}
