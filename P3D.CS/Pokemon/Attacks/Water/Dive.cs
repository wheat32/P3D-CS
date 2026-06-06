using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class Dive : Attack
{
    public Dive()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 291;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Dive");
        Description = "Diving on the first turn, the user floats up and attacks on the next turn.";
        criticalChance = 1;
        isHMMove = true;
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
        canHitUnderwater = true;
        canHitUnderground = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int dive = battleScreen.FieldEffects.DiveCounter.Self;
        if (own == false)
        {
            dive = battleScreen.FieldEffects.DiveCounter.Opponent;
        }

        if (dive == 0)
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
        int dive = battleScreen.FieldEffects.DiveCounter.Self;
        if (own == false)
        {
            dive = battleScreen.FieldEffects.DiveCounter.Opponent;
        }

        if (dive == 0)
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
        int diveCounter = battleScreen.FieldEffects.DiveCounter.Self;

        if (own == false)
        {
            diveCounter = battleScreen.FieldEffects.DiveCounter.Opponent;
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
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Dive!", "move:dive") == true)
                {
                    hasToCharge = false;
                }
            }
        }

        if (diveCounter == 0 && hasToCharge == true)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.DiveCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.DiveCounter.Opponent = 1;
            }

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " dived into the water!"));

            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.DiveCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.DiveCounter.Opponent = 0;
            }

            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.DiveCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.DiveCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int Dive = battleScreen.FieldEffects.DiveCounter.Self;
        if (own == false)
        {
            Dive = battleScreen.FieldEffects.DiveCounter.Opponent;
        }

        if (Dive == 0)
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
            battleScreen.FieldEffects.DiveCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.DiveCounter.Opponent = 0;
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
