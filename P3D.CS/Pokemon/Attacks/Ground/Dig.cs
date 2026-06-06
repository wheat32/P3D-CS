using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class Dig : Attack
{
    public Dig()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 91;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Dig");
        Description = "The user burrows, then attacks on the second turn. It can also be used to exit dungeons.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
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
        canHitUnderground = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int dig = battleScreen.FieldEffects.DigCounter.Self;
        if (own == false)
        {
            dig = battleScreen.FieldEffects.DigCounter.Opponent;
        }

        if (dig == 0)
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
        int dig = battleScreen.FieldEffects.DigCounter.Self;
        if (own == false)
        {
            dig = battleScreen.FieldEffects.DigCounter.Opponent;
        }

        if (dig == 0)
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
        int digCounter = battleScreen.FieldEffects.DigCounter.Self;

        if (own == false)
        {
            digCounter = battleScreen.FieldEffects.DigCounter.Opponent;
        }

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Dig!", "move:dig") == true)
                {
                    digCounter = 1;
                }
            }
        }

        if (digCounter == 0)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.DigCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.DigCounter.Opponent = 1;
            }

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " burrowed its way underground!"));

            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.DigCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.DigCounter.Opponent = 0;
            }

            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.DigCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.DigCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int dig = battleScreen.FieldEffects.DigCounter.Self;
        if (own == false)
        {
            dig = battleScreen.FieldEffects.DigCounter.Opponent;
        }

        if (dig == 0)
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
            battleScreen.FieldEffects.DigCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.DigCounter.Opponent = 0;
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
