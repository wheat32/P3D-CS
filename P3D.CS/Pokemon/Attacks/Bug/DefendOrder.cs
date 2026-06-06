using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class DefendOrder : Attack
{
    public DefendOrder()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 455;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Defend Order");
        Description = "The user calls out its underlings to shield its body, raising its Defense and Sp. Def. stats.";
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
        aiField2 = AIField.RaiseAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool failed = true;

        if (battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:defendorder") == true)
        {
            failed = false;
        }
        if (battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 1, "", "move:defendorder") == true)
        {
            failed = false;
        }

        if (failed == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
