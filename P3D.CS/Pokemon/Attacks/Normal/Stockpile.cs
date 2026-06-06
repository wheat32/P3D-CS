using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Stockpile : Attack
{
    public Stockpile()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 254;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Stockpile");
        Description = "The user charges up power and raises both its Defense and Sp. Def. The move can be used three times.";
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

        aiField1 = AIField.RaiseDefense;
        aiField2 = AIField.RaiseSpDefense;
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

        if (stockpiled < 3)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.StockpileCount.Self += 1;
            }
            else
            {
                battleScreen.FieldEffects.StockpileCount.Opponent += 1;
            }
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " stockpiled " + (stockpiled + 1).ToString() + "!"));

            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:stockpile");
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 1, "", "move:stockpile");
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
