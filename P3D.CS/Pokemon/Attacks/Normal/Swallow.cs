using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Swallow : Attack
{
    public Swallow()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 256;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Swallow");
        Description = "The power stored using the move Stockpile is absorbed by the user to heal its HP. Storing more power heals more HP.";
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

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        int stockpiled = battleScreen.FieldEffects.StockpileCount.Self;
        if (own == false)
        {
            stockpiled = battleScreen.FieldEffects.StockpileCount.Opponent;
        }

        if (stockpiled == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int stockpiled = battleScreen.FieldEffects.StockpileCount.Self;
        if (own == false)
        {
            stockpiled = battleScreen.FieldEffects.StockpileCount.Opponent;
        }

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (own == true)
        {
            battleScreen.FieldEffects.StockpileCount.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.StockpileCount.Opponent = 0;
        }

        int hpPercentage = 25;
        switch (stockpiled)
        {
            case 1:
                hpPercentage = 25;
                break;
            case 2:
                hpPercentage = 50;
                break;
            case 3:
                hpPercentage = 100;
                break;
        }
        int hpGain = (int)(Math.Ceiling((double)((p.MaxHP / 100) * hpPercentage)));

        battleScreen.Battle.GainHP(hpGain, own, own, battleScreen, p.GetDisplayName() + "'s HP was restored!", "move:swallow");

        battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", stockpiled, p.GetDisplayName() + "'s Defense fell!", "move:spitup");
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Special Defense", stockpiled, p.GetDisplayName() + "'s Special Defense fell!", "move:spitup");

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s stockpiled effect wore off!"));
    }

}
