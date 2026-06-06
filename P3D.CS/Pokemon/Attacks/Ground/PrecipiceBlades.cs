using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class PrecipiceBlades : Attack
{
    public PrecipiceBlades()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 619;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 120;
        Accuracy = 85;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Precipice Blades");
        Description = "The user attacks opposing Pokémon by manifesting the power of the land in fearsome blades of stone.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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
        // #End
    }

}
