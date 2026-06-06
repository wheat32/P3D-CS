using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Swagger : Attack
{
    public Swagger()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 207;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 85;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Swagger");
        Description = "The user enrages and confuses the target. However, it also sharply raises the target's Attack stat.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Confusion;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool fail = true;

        if (battleScreen.Battle.RaiseStat(own == false, own, battleScreen, "Attack", 2, "", "move:swagger") == true)
        {
            fail = false;
        }

        if (battleScreen.Battle.InflictConfusion(own == false, own, battleScreen, "", "move:swagger") == true)
        {
            fail = false;
        }

        if (fail == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
