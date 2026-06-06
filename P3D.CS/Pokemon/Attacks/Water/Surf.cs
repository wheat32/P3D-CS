using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class Surf : Attack
{
    public Surf()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 57;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 90;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Surf");
        Description = "It swamps the area around the user with a giant wave. It can also be used for crossing water.";
        criticalChance = 1;
        isHMMove = true;
        target = Targets.AllAdjacentTargets;
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
        canHitUnderwater = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int dive = battleScreen.FieldEffects.DiveCounter.Opponent;
        if (own == false)
        {
            dive = battleScreen.FieldEffects.DiveCounter.Self;
        }

        if (dive > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

}
