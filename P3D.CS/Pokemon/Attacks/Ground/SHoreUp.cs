using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class ShoreUp : Attack
{
    public ShoreUp()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 659;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Shore Up");
        Description = "The user regains up to half of its max HP. It restores more HP in a sandstorm.";
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
        mirrorMoveAffected = false;
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
            case BattleWeather.WeatherTypes.Sandstorm:
                restoreHP = (int)(Math.Ceiling((double)((2 / 3) * p.MaxHP)));
                break;
            default:
                restoreHP = (int)(Math.Ceiling((double)((1 / 2) * p.MaxHP)));
                break;
        }

        if (p.HP < p.MaxHP && p.HP > 0)
        {
            battleScreen.Battle.GainHP(restoreHP, own, own, battleScreen, p.GetDisplayName() + "'s HP was restored!", "move:shoreup");
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
