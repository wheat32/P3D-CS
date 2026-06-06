using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class Hail : Attack
{
    public Hail()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 258;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Hail");
        Description = "The user summons a hailstorm lasting five turns. It damages all Pokémon except the Ice type.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.All;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int turns = BattleCalculation.FieldEffectTurns(battleScreen, own, Name.ToLower());
        battleScreen.Battle.ChangeWeather(own, own, BattleWeather.WeatherTypes.Hailstorm, turns, battleScreen, "It started to hail!", "move:hail");
    }

}
