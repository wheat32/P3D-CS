using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class SeismicToss : Attack
{
    public SeismicToss()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 69;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Seismic Toss");
        Description = "The target is thrown using the power of gravity. It inflicts damage equal to the user's level.";
        criticalChance = 0;
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
        useEffectiveness = false;
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

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        return p.Level;
    }

}
