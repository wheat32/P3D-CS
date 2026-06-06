using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class PayDay : Attack
{
    public PayDay()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 6;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Pay Day");
        Description = "Numerous coins are hurled at the target to inflict damage. Money is earned after the battle.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
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

        int coinAmount = p.Level * 5;

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "amulet coin" || p.Item.OriginalName.ToLower() == "luck incense")
            {
                coinAmount *= 2;
            }
        }

        foreach (MysteryEvent mysteryEvent in MysteryEventScreen.ActivatedMysteryEvents)
        {
            if (mysteryEvent.EventType == MysteryEventScreen.EventTypes.MoneyMultiplier)
            {
                coinAmount = (int)(coinAmount * (double)(mysteryEvent.Value.Replace(".", GameController.DecSeparator)));
            }
        }

        if (own == true)
        {
            battleScreen.FieldEffects.PayDayCounter.Self += coinAmount;
        }
        else
        {
            battleScreen.FieldEffects.PayDayCounter.Opponent += coinAmount;
        }

        battleScreen.BattleQuery.Add(new TextQueryObject("Coins were scattered everywhere!"));
    }

}
