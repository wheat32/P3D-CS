using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves;

public class ConfusionAttack : Attack
{
    public ConfusionAttack()
    {
        // #Definitions
        type = new Element(Element.Types.Blank);
        ID = 0;
        originalPP = 1;
        currentPP = 1;
        maxPP = 1;
        Power = 40;
        Accuracy = -1;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "ConfusionAttack");
        Description = "Hits to the face.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = false;
        kingsrockAffected = false;
        counterAffected = false;
        disabledWhileGravity = false;
        useEffectiveness = false;
        isHealingMove = false;
        removesSelfFrozen = false;
        isRecoilMove = false;

        immunityAffected = true;
        isDamagingMove = true;
        isProtectMove = false;

        hasSecondaryEffect = false;
        isAffectedBySubstitute = false;
        // #End
    }

}
