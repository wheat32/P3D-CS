using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class Hex : Attack
{
    public Hex()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 506;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 65;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Hex");
        Description = "This relentless attack does massive damage to a target affected by status problems.";
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
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        if (op.Status != Pokemon.StatusProblems.None && op.Status != Pokemon.StatusProblems.Fainted)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

}
