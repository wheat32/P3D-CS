using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class SpitUp : Attack
{
    public SpitUp()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 255;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Spit Up");
        Description = "The power stored using the move Stockpile is released at once in an attack. The more power is stored, the greater the damage.";
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

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int stockpiled = battleScreen.FieldEffects.StockpileCount.Self;
        if (own == false)
        {
            stockpiled = battleScreen.FieldEffects.StockpileCount.Opponent;
        }

        return stockpiled * 100;
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

        battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", stockpiled, p.GetDisplayName() + "'s Defense fell!", "move:spitup");
        battleScreen.Battle.LowerStat(own, own, battleScreen, "Special Defense", stockpiled, p.GetDisplayName() + "'s Special Defense fell!", "move:spitup");

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s stockpiled effect wore off!"));
    }

}
