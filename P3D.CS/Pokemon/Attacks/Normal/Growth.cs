using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Growth : Attack
{
    public Growth()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 74;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Growth");
        Description = "The user's body grows all at once, raising the Attack and Sp. Atk. stats.";
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

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.RaiseSpAttack;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int statRaise = 1;
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny)
        {
            statRaise = 2;
        }

        bool b = false;

        if (battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", statRaise, "", "move:growth") == true)
        {
            b = true;
        }
        if (battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", statRaise, "", "move:growth") == true)
        {
            b = true;
        }

        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
