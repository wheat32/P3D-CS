using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Acupressure : Attack
{
    public Acupressure()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 367;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Acupressure");
        Description = "The user applies pressure to stress points, sharply boosting one of its or its allies' stats.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = true;
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
        useAccEvasion = false;
        // #End

        aiField1 = AIField.RaiseAttack;
        aiField2 = AIField.RaiseSpAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        List<String> statstoboost = [];
        if (p.StatAttack < 6)
        {
            statstoboost.Add("Attack");
        }

        if (p.StatDefense < 6)
        {
            statstoboost.Add("Defense");
        }

        if (p.StatSpAttack < 6)
        {
            statstoboost.Add("Special Attack");
        }

        if (p.StatSpDefense < 6)
        {
            statstoboost.Add("Special Defense");
        }

        if (p.StatSpeed < 6)
        {
            statstoboost.Add("Speed");
        }

        if (p.Accuracy < 6)
        {
            statstoboost.Add("Accuracy");
        }

        if (p.Evasion < 6)
        {
            statstoboost.Add("Evasion");
        }

        if (statstoboost.Count == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
        else
        {
            battleScreen.Battle.RaiseStat(own, own, battleScreen, statstoboost(Core.Random.Next(0, statstoboost.Count)), 2, "", "move:acupressure");
        }
    }

}
