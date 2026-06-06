using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class WorkUp : Attack
{
    public WorkUp()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 526;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Work Up");
        Description = "The user is roused, and its Attack and Sp. Atk. stats increase.";
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
        kingsrockAffected = true;
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.RaiseAttack;
        aiField2 = AIField.RaiseSpAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", 1, "", "move:workup");
        bool d = battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", 1, "", "move:workup");
        if (b == false || d == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
