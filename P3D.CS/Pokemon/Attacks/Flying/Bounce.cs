using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Bounce : Attack
{
    public Bounce()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 340;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 85;
        Accuracy = 85;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Bounce");
        Description = "The user bounces up high, then drops on the target on the second turn. This may also leave the target with paralysis.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneTarget;
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

        disabledWhileGravity = true;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
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

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int bounce = battleScreen.FieldEffects.BounceCounter.Self;
        if (own == false)
        {
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
        }

        if (bounce == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void PreAttack(bool own, BattleScreen battleScreen)
    {
        int bounce = battleScreen.FieldEffects.BounceCounter.Self;
        if (own == false)
        {
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
        }

        if (bounce == 0)
        {
            focusOpponentPokemon = false;
        }
        else
        {
            focusOpponentPokemon = true;
        }
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int bounce = battleScreen.FieldEffects.BounceCounter.Self;
        if (own == false)
        {
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Bounce!", "move:bounce") == true)
                {
                    bounce = 1;
                }
            }
        }

        if (bounce == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " sprang up!"));
            if (own == true)
            {
                battleScreen.FieldEffects.BounceCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.BounceCounter.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.BounceCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.BounceCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.BounceCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.BounceCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int bounce = battleScreen.FieldEffects.BounceCounter.Self;
        if (own == false)
        {
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
        }

        if (bounce == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void MoveFails(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.BounceCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.BounceCounter.Opponent = 0;
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

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

}
