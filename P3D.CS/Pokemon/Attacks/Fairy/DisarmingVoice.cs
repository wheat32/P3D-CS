using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fairy;

public class DisarmingVoice : Attack
{
    public DisarmingVoice()
    {
        // #Definitions
        type = new Element(Element.Types.Fairy);
        ID = 574;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 40;
        Accuracy = 0;
        category = Categories.Special;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Disarming Voice");
        Description = "Letting out a charming cry, the user does emotional damage to opposing Pokémon. This attack never misses.";
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
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CannotMiss;
    }

}
