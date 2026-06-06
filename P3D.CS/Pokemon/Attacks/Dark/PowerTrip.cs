using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class PowerTrip : Attack
{
    public PowerTrip()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 681;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 20;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Power Trip");
        Description = "The user boasts its strength and attacks the target. The more the user's stats are raised, the greater the move's power.";
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
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int powermult = 1;

        if (p.StatAttack > 0)
        {
            powermult += p.StatAttack;
        }

        if (p.StatDefense > 0)
        {
            powermult += p.StatDefense;
        }

        if (p.StatSpAttack > 0)
        {
            powermult += p.StatSpAttack;
        }

        if (p.StatSpDefense > 0)
        {
            powermult += p.StatSpDefense;
        }

        if (p.StatSpeed > 0)
        {
            powermult += p.StatSpeed;
        }

        if (p.Accuracy > 0)
        {
            powermult += p.Accuracy;
        }

        if (p.Evasion > 0)
        {
            powermult += p.Evasion;
        }

        return (20 * (powermult));
    }

}
