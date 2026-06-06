using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class SolarBlade : Attack
{
    public SolarBlade()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 669;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 125;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Solar Blade");
        Description = "In this two-turn attack, the user gathers light and fills a blade with the light's energy, attacking the target on the next turn.";
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
        int SolarBlade = battleScreen.FieldEffects.SolarBlade.Self;
        if (own == false)
        {
            SolarBlade = battleScreen.FieldEffects.SolarBlade.Opponent;
        }

        if (SolarBlade == 0)
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
        int SolarBlade = battleScreen.FieldEffects.SolarBlade.Self;
        if (own == false)
        {
            SolarBlade = battleScreen.FieldEffects.SolarBlade.Opponent;
        }

        if (SolarBlade == 0)
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
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        bool hasToCharge = true;

        int blade = battleScreen.FieldEffects.SolarBlade.Self;
        if (own == false)
        {
            blade = battleScreen.FieldEffects.SolarBlade.Opponent;
        }

        if (blade == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " absorbed sunlight!"));
        }
        else
        {
            hasToCharge = false;
        }

        if (hasToCharge == true)
        {
            if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny)
            {
                hasToCharge = false;
            }
            else
            {
                if (p.Item != null)
                {
                    if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
                    {
                        if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Solar Blade!", "move:solarblade") == true)
                        {
                            hasToCharge = false;
                        }
                    }
                }
            }
        }

        if (hasToCharge == true)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.SolarBlade.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.SolarBlade.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.SolarBlade.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.SolarBlade.Opponent = 0;
            }
            return false;
        }
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain || battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sandstorm || battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
        {
            return (int)(Power / 2);
        }
        else
        {
            return Power;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.SolarBlade.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SolarBlade.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int solarBlade = battleScreen.FieldEffects.SolarBlade.Self;
        if (own == false)
        {
            solarBlade = battleScreen.FieldEffects.SolarBlade.Opponent;
        }

        if (solarBlade == 0)
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
            battleScreen.FieldEffects.SolarBlade.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SolarBlade.Opponent = 0;
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
