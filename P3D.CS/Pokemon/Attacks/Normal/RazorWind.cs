using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class RazorWind : Attack
{
    public RazorWind()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 13;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Razor Wind");
        Description = "A two-turn attack. Blades of wind hit opposing Pokémon on the second turn. Critical hits land more easily.";
        criticalChance = 2;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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
        isWindMove = true;
        isSlicingMove = true;
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
        int RazorWind = battleScreen.FieldEffects.RazorWindCounter.Self;
        if (own == false)
        {
            RazorWind = battleScreen.FieldEffects.RazorWindCounter.Opponent;
        }

        if (RazorWind == 0)
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
        int RazorWind = battleScreen.FieldEffects.RazorWindCounter.Self;
        if (own == false)
        {
            RazorWind = battleScreen.FieldEffects.RazorWindCounter.Opponent;
        }

        if (RazorWind == 0)
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

        int razorWind = battleScreen.FieldEffects.RazorWindCounter.Self;
        if (own == false)
        {
            razorWind = battleScreen.FieldEffects.RazorWindCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Razor Wind!", "move:razorwind") == true)
                {
                    razorWind = 1;
                }
            }
        }

        if (razorWind == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " whipped up a whirlwind!"));
            if (own == true)
            {
                battleScreen.FieldEffects.RazorWindCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.RazorWindCounter.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.RazorWindCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.RazorWindCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.RazorWindCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.RazorWindCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int razorWind = battleScreen.FieldEffects.RazorWindCounter.Self;
        if (own == false)
        {
            razorWind = battleScreen.FieldEffects.RazorWindCounter.Opponent;
        }

        if (razorWind == 0)
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
            battleScreen.FieldEffects.RazorWindCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.RazorWindCounter.Opponent = 0;
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
