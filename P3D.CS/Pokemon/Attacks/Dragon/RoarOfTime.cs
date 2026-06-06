using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dragon;

public class RoarOfTime : Attack
{
    public RoarOfTime()
    {
        // #Definitions
        type = new Element(Element.Types.Dragon);
        ID = 459;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 150;
        Accuracy = 90;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Roar of Time");
        Description = "The user blasts the target with power that distorts even time. The user can't move on the next turn.";
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

        aiField1 = AIField.Damage;
        aiField2 = AIField.Recharge;
    }

    public override void MoveRecharge(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.Recharge.Self += 1;
        }
        else
        {
            battleScreen.FieldEffects.Recharge.Opponent += 1;
        }
    }

}
