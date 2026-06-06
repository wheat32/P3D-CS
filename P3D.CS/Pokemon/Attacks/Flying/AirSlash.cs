using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class AirSlash : Attack
{
    public AirSlash()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 403;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 75;
        Accuracy = 95;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Air Slash");
        Description = "The user attacks with a blade of air that slices even the sky. It may also make the target flinch.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isSlicingMove = true;
        isWindMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:airslash");
        }
    }

}
