using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class Sandstorm : Attack
{
    public Sandstorm()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 201;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Sandstorm");
        Description = "A five-turn sandstorm is summoned to hurt all combatants except the Rock, Ground, and Steel types.";
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
        isWindMove = true;
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
        battleScreen.Battle.ChangeWeather(own, own, BattleWeather.WeatherTypes.Sandstorm, turns, battleScreen, "A sandstorm brewed!", "move:sandstorm");
    }

}
