using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class MorningSun : Attack
{
    public MorningSun()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 234;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Morning Sun");
        Description = "The user restores its own HP. The amount of HP regained varies with the weather.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = true;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Healing;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int restoreHP = 1;
        switch (battleScreen.FieldEffects.Weather)
        {
            case BattleWeather.WeatherTypes.Sunny:
                restoreHP = (int)(Math.Ceiling((double)((2 / 3) * p.MaxHP)));
                break;
            case BattleWeather.WeatherTypes.Clear:
                restoreHP = (int)(Math.Ceiling((double)((1 / 2) * p.MaxHP)));
                break;
            default:
                restoreHP = (int)(Math.Ceiling((double)((1 / 4) * p.MaxHP)));
                break;
        }

        if (p.HP < p.MaxHP && p.HP > 0)
        {
            battleScreen.Battle.GainHP(restoreHP, own, own, battleScreen, p.GetDisplayName() + "'s HP was restored!", "move:morningsun");
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
