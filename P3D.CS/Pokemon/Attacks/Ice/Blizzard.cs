using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class Blizzard : Attack
{
    public Blizzard()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 59;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 110;
        Accuracy = 70;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Blizzard");
        Description = "A howling blizzard is summoned to strike the opposing team. It may also freeze them solid.";
        criticalChance = 1;
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
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = true;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanFreeze;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictFreeze(own == false, own, battleScreen, "", "move:blizzard");
        }
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
        {
            return false;
        }
        return useAccEvasion;
    }

    public override int GetAccuracy(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
        {
            return 100;
        }
        return Accuracy;
    }

}
