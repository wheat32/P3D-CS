using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class RageFist : Attack
{
    public RageFist()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 889;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 50;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Rage Fist");
        Description = "The user converts its rage into energy to attack. The more times the user has been hit by attacks, the greater the move's power.";
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

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

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
        aiField2 = AIField.Nothing;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            return Power + battleScreen.FieldEffects.RageFistPower.Self;
        }
        else
        {
            return Power + battleScreen.FieldEffects.RageFistPower.Opponent;
        }
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.RageFistPower.Self < 350)
            {
                battleScreen.FieldEffects.RageFistPower.Self += 50;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.RageFistPower.Opponent < 350)
            {
                battleScreen.FieldEffects.RageFistPower.Opponent += 50;
            }
        }

    }

}
