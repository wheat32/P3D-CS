using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Punishment : Attack
{
    public Punishment()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 386;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Punishment");
        Description = "This attack's power increases the more the target has powered up with stat changes.";
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
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.SelfPokemon;
        }

        int powerup = 0;

        if (p.StatAttack > 0)
        {
            powerup += p.StatAttack;
        }
        if (p.StatDefense > 0)
        {
            powerup += p.StatAttack;
        }
        if (p.StatSpAttack > 0)
        {
            powerup += p.StatAttack;
        }
        if (p.StatSpDefense > 0)
        {
            powerup += p.StatAttack;
        }
        if (p.StatSpeed > 0)
        {
            powerup += p.StatAttack;
        }

        return (60 + (20 * powerup));
    }

}
