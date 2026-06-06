using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class SkullBash : Attack
{
    public SkullBash()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 130;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 130;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Skull Bash");
        Description = "The user tucks in its head to raise its Defense in the first turn, then rams the target on the next turn.";
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
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.RaiseDefense;
        aiField3 = AIField.MultiTurn;
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int SkullBash = battleScreen.FieldEffects.SkullBashCounter.Self;
        if (own == false)
        {
            SkullBash = battleScreen.FieldEffects.SkullBashCounter.Opponent;
        }

        if (SkullBash == 0)
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
        int SkullBash = battleScreen.FieldEffects.SkullBashCounter.Self;
        if (own == false)
        {
            SkullBash = battleScreen.FieldEffects.SkullBashCounter.Opponent;
        }

        if (SkullBash == 0)
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

        int skullBash = battleScreen.FieldEffects.SkullBashCounter.Self;
        if (own == false)
        {
            skullBash = battleScreen.FieldEffects.SkullBashCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Skull Bash!", "move:skullbash") == true)
                {
                    skullBash = 1;
                }
            }
        }

        if (skullBash == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " tucked in its head!"));
            if (own == true)
            {
                battleScreen.FieldEffects.SkullBashCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.SkullBashCounter.Opponent = 1;
            }
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:skullbash");
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.SkullBashCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.SkullBashCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.SkullBashCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SkullBashCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int skullBash = battleScreen.FieldEffects.SkullBashCounter.Self;
        if (own == false)
        {
            skullBash = battleScreen.FieldEffects.SkullBashCounter.Opponent;
        }

        if (skullBash == 0)
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
            battleScreen.FieldEffects.SkullBashCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SkullBashCounter.Opponent = 0;
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
