using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Facade : Attack
{
    public Facade()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 263;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 70;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Facade");
        Description = "An attack move that doubles its power if the user is poisoned, burned, or has paralysis.";
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
        kingsrockAffected = false;
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

        if (p.Status != Pokemon.StatusProblems.None && p.Status != Pokemon.StatusProblems.Fainted)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

}
