using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class WeatherBall : Attack
{
    public WeatherBall()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 311;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 50;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Weather Ball");
        Description = "An attack move that varies in power and type depending on the weather.";
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
        isBulletMove = true;
        // #End
    }

    public override Element GetAttackType(bool own, BattleScreen battleScreen)
    {
        WeatherTypes a = battleScreen.FieldEffects.Weather;
        switch (a)
        {
            case BattleWeather.WeatherTypes.Sunny:
                return new Element(Element.Types.Fire);
                break;
            case BattleWeather.WeatherTypes.Rain:
                return new Element(Element.Types.Water);
                break;
            case BattleWeather.WeatherTypes.Sandstorm:
                return new Element(Element.Types.Rock);
                break;
            case BattleWeather.WeatherTypes.Hailstorm:
                return new Element(Element.Types.Ice);
                break;
            default:
                return new Element(Element.Types.Normal);
                break;
        }

        return type;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sandstorm || battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
        {
            return (int)(Power * 2);
        }
        else if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny || battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain)
        {
            return (int)(Power * 3);
        }
        else
        {
            return Power;
        }
    }

}
