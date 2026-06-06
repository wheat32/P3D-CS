using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Payback : Attack
{
    public Payback()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 371;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 50;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Payback");
        Description = "If the user moves after the target, this attack's power will be doubled";
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
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {

            if (own == true)
            {
                if (battleScreen.FieldEffects.TurnCounts.Self < battleScreen.FieldEffects.TurnCounts.Opponent)
                {
                    return Power * 2;
                }
            }
            else
            {
                if (battleScreen.FieldEffects.TurnCounts.Opponent < battleScreen.FieldEffects.TurnCounts.Self)
                {
                    return Power * 2;
                }
            }


        return Power;
    }

}
