using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class Magnitude : Attack
{
    public Magnitude()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 222;
        originalPP = 30;
        currentPP = 30;
        maxPP = 30;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Magnitude");
        Description = "The user looses a ground- shaking quake affecting everyone around the user. Its power varies.";
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
        counterAffected = true;

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
        canHitUnderground = true;
        // #End
    }

    private static int UsedLevel = 0;
    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int dig = battleScreen.FieldEffects.DigCounter.Opponent;
        if (own == false)
        {
            dig = battleScreen.FieldEffects.DigCounter.Self;
        }

        int basepower = 0;
        int magLevel = 4;

        int r = Core.Random.Next(0, 100);
        if (r < 5)
        {
            basepower = 10;
            magLevel = 4;
        }
        else if (r >= 5 && r < 15)
        {
            basepower = 30;
            magLevel = 5;
        }
        else if (r >= 15 && r < 35)
        {
            basepower = 50;
            magLevel = 6;
        }
        else if (r >= 35 && r < 65)
        {
            basepower = 70;
            magLevel = 7;
        }
        else if (r >= 65 && r < 85)
        {
            basepower = 90;
            magLevel = 8;
        }
        else if (r >= 85 && r < 95)
        {
            basepower = 110;
            magLevel = 9;
        }
        else if (r >= 95)
        {
            basepower = 150;
            magLevel = 10;
        }

        if (dig > 0)
        {
            basepower *= 2;
        }

        UsedLevel = magLevel;

        return basepower;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.BattleQuery.Add(new TextQueryObject("Magnitude " + UsedLevel.ToString() + "!"));
    }

}
