using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class PhantomForce : Attack
{
    public PhantomForce()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 566;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 90;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Phantom Force");
        Description = "The user vanishes somewhere, then strikes the target on the next turn. This move hits even if the target protects itself.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = false;
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
        useAccEvasion = true;
        canHitUnderwater = false;
        canHitUnderground = false;
        canHitInMidAir = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int minimize = battleScreen.FieldEffects.Minimize.Opponent;
        if (own == false)
        {
            minimize = battleScreen.FieldEffects.Minimize.Self;
        }
        if (minimize > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int phantomforce = battleScreen.FieldEffects.PhantomForceCounter.Self;
        if (own == false)
        {
            phantomforce = battleScreen.FieldEffects.PhantomForceCounter.Opponent;
        }

        if (phantomforce == 0)
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
        int phantomforce = battleScreen.FieldEffects.PhantomForceCounter.Self;
        if (own == false)
        {
            phantomforce = battleScreen.FieldEffects.PhantomForceCounter.Opponent;
        }

        if (phantomforce == 0)
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
        int phantomforceCounter = battleScreen.FieldEffects.PhantomForceCounter.Self;

        if (own == false)
        {
            phantomforceCounter = battleScreen.FieldEffects.PhantomForceCounter.Opponent;
        }

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        bool hasToCharge = true;
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Phantom Force!", "move:phantomforce") == true)
                {
                    hasToCharge = false;
                }
            }
        }

        if (phantomforceCounter == 0 && hasToCharge == true)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.PhantomForceCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.PhantomForceCounter.Opponent = 1;
            }

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " vanished instantly!"));

            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.PhantomForceCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.PhantomForceCounter.Opponent = 0;
            }

            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.PhantomForceCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.PhantomForceCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int PhantomForce = battleScreen.FieldEffects.PhantomForceCounter.Self;
        if (own == false)
        {
            PhantomForce = battleScreen.FieldEffects.PhantomForceCounter.Opponent;
        }

        if (PhantomForce == 0)
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
            battleScreen.FieldEffects.PhantomForceCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.PhantomForceCounter.Opponent = 0;
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

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon op = battleScreen.OpponentPokemon;

        if (own == true)
        {
            if (battleScreen.FieldEffects.DetectCounter.Opponent > 0 || battleScreen.FieldEffects.ProtectCounter.Opponent > 0 || battleScreen.FieldEffects.KingsShieldCounter.Opponent > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Opponent > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Opponent > 0 || battleScreen.FieldEffects.MatBlockCounter.Opponent > 0 || battleScreen.FieldEffects.WideGuardCounter.Opponent > 0 || battleScreen.FieldEffects.QuickGuardCounter.Opponent > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Phantom Force lifted " + op.GetDisplayName() + "'s protection!"));
            }
            battleScreen.FieldEffects.DetectCounter.Opponent = 0;
            battleScreen.FieldEffects.ProtectCounter.Opponent = 0;
            battleScreen.FieldEffects.KingsShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.SpikyShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.BanefulBunkerCounter.Opponent = 0;
            battleScreen.FieldEffects.CraftyShieldCounter.Opponent = 0;
            battleScreen.FieldEffects.MatBlockCounter.Opponent = 0;
            battleScreen.FieldEffects.WideGuardCounter.Opponent = 0;
            battleScreen.FieldEffects.QuickGuardCounter.Opponent = 0;
        }
        else
        {
            op = battleScreen.SelfPokemon;
            if (battleScreen.FieldEffects.DetectCounter.Self > 0 || battleScreen.FieldEffects.ProtectCounter.Self > 0 || battleScreen.FieldEffects.KingsShieldCounter.Self > 0 || battleScreen.FieldEffects.SpikyShieldCounter.Self > 0 || battleScreen.FieldEffects.BanefulBunkerCounter.Self > 0 || battleScreen.FieldEffects.CraftyShieldCounter.Self > 0 || battleScreen.FieldEffects.MatBlockCounter.Self > 0 || battleScreen.FieldEffects.WideGuardCounter.Self > 0 || battleScreen.FieldEffects.QuickGuardCounter.Self > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Phantom Force lifted " + op.GetDisplayName() + "'s protection!"));
            }
            battleScreen.FieldEffects.DetectCounter.Self = 0;
            battleScreen.FieldEffects.ProtectCounter.Self = 0;
            battleScreen.FieldEffects.KingsShieldCounter.Self = 0;
            battleScreen.FieldEffects.SpikyShieldCounter.Self = 0;
            battleScreen.FieldEffects.BanefulBunkerCounter.Self = 0;
            battleScreen.FieldEffects.CraftyShieldCounter.Self = 0;
            battleScreen.FieldEffects.MatBlockCounter.Self = 0;
            battleScreen.FieldEffects.WideGuardCounter.Self = 0;
            battleScreen.FieldEffects.QuickGuardCounter.Self = 0;
        }

    }

}
