using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class Hurricane : Attack
{
    public Hurricane()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 542;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 110;
        Accuracy = 70;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Hurricane");
        Description = "The user attacks by wrapping its opponent in a fierce wind that flies up into the sky. It may also confuse the target.";
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
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        canHitInMidAir = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanConfuse;

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictConfusion(own == false, own, battleScreen, "", "move:hurricane");
        }
    }

    public override int GetAccuracy(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain)
        {
            return 100;
        }
        else if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny)
        {
            return 50;
        }
        else
        {
            return Accuracy;
        }
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
