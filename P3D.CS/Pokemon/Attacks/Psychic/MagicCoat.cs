using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class MagicCoat : Attack
{
    public MagicCoat()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 277;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Magic Coat");
        Description = "A barrier reflects back to the target moves like Leech Seed and moves that damage status.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 4;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.MagicCoat.Self = 1;
        }
        else
        {
            battleScreen.FieldEffects.MagicCoat.Opponent = 1;
        }
    }

}
