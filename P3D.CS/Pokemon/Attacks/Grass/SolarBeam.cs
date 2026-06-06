using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class SolarBeam : Attack
{
    public SolarBeam()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 76;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 120;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Solar Beam");
        Description = "A two-turn attack. The user gathers light, then blasts a bundled beam on the second turn.";
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int SolarBeam = battleScreen.FieldEffects.SolarBeam.Self;
        if (own == false)
        {
            SolarBeam = battleScreen.FieldEffects.SolarBeam.Opponent;
        }

        if (SolarBeam == 0)
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
        int SolarBeam = battleScreen.FieldEffects.SolarBeam.Self;
        if (own == false)
        {
            SolarBeam = battleScreen.FieldEffects.SolarBeam.Opponent;
        }

        if (SolarBeam == 0)
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

        int beam = battleScreen.FieldEffects.SolarBeam.Self;
        if (own == false)
        {
            beam = battleScreen.FieldEffects.SolarBeam.Opponent;
        }

        if (beam == 0)
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
                        if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Solar Beam!", "move:solarbeam") == true)
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
                battleScreen.FieldEffects.SolarBeam.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.SolarBeam.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.SolarBeam.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.SolarBeam.Opponent = 0;
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
            battleScreen.FieldEffects.SolarBeam.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SolarBeam.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int solarBeam = battleScreen.FieldEffects.SolarBeam.Self;
        if (own == false)
        {
            solarBeam = battleScreen.FieldEffects.SolarBeam.Opponent;
        }

        if (solarBeam == 0)
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
            battleScreen.FieldEffects.SolarBeam.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SolarBeam.Opponent = 0;
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
