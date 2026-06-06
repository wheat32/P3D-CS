using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class StompingTantrum : Attack
{
    public StompingTantrum()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 707;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 75;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Stomping Tantrum");
        Description = "Driven by frustration, the user attacks the target. If the user's previous move has failed, the power of this move doubles.";
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

        aiField1 = AIField.Damage;
        aiField2 = AIField.Nothing;

    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;

        bool lastMoveFailed = battleScreen.FieldEffects.LastMoveFailed.Self;

        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;

            lastMoveFailed = battleScreen.FieldEffects.LastMoveFailed.Opponent;
        }

        int basePower = Power;
        if (lastMoveFailed == true)
        {
            basePower *= 2;
        }

        return basePower;
    }

}
