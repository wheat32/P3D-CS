using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class RockWrecker : Attack
{
    public RockWrecker()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 439;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 150;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Rock Wrecker");
        Description = "The user launches a huge boulder at the target to attack. The user can't move on the next turn.";
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
        isBulletMove = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
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
