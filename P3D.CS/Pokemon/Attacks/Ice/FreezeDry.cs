using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class FreezeDry : Attack
{
    public FreezeDry()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 573;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 70;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Freeze Dry");
        Description = "The user rapidly cools the target. This may also leave the target frozen. This move is super effective on Water types.";
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
        kingsrockAffected = false;
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
        aiField2 = AIField.CanFreeze;

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
            battleScreen.Battle.InflictFreeze(own == false, own, battleScreen, "", "move:freezedry");
        }
    }

}
