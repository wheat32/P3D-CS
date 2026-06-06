using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fairy;

public class Moonblast : Attack
{
    public Moonblast()
    {
        // #Definitions
        type = new Element(Element.Types.Fairy);
        ID = 585;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 95;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Moonblast");
        Description = "Borrowing the power of the moon, the user attacks the target. This may also lower the target's Sp. Atk. stat.";
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
        hasSecondaryEffect = true;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanLowerSpAttack;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Attack", 1, "", "move:moonblast");
        }
    }

}
