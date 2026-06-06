using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class PowerUpPunch : Attack
{
    public PowerUpPunch()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 612;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Power-Up Punch");
        Description = "Striking opponents over and over makes the user's fists harder. Hitting a target raises the Attack stat.";
        criticalChance = 0;
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isPunchingMove = true;
        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.RaiseAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Attack", 1, "", "move:power-uppunch");
    }

}
