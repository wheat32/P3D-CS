using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class FusionFlare : Attack
{
    public FusionFlare()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 558;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 100;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Fusion Flare");
        Description = "The user brings down a giant flame. This attack does greater damage when influenced by an enormous thunderbolt.";
        criticalChance = 1;
        isHMMove = false;
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
        removesSelfFrozen = true;

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
        Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
        if (own == false)
        {
            lastMove = battleScreen.FieldEffects.LastMove.Opponent;
        }

        if (lastMove != null)
        {
            if (lastMove.Name.ToLower() == "fusion bolt" || lastMove.Name.ToLower() == "bolt strike")
            {
                return Power * 2;
            }
        }

        return Power;
    }

}
