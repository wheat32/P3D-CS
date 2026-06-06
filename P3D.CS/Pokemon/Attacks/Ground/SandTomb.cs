using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ground;

public class SandTomb : Attack
{
    public SandTomb()
    {
        // #Definitions
        type = new Element(Element.Types.Ground);
        ID = 328;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 35;
        Accuracy = 85;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Sand Tomb");
        Description = "The user traps the target inside a harshly raging sandstorm for four to five turns.";
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

        aiField1 = AIField.Damage;
        aiField2 = AIField.Trap;
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

        int turns = 4;
        if (Core.Random.Next(0, 100) < 50)
        {
            turns = 5;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "grip claw" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                turns = 5;
            }
        }

        if (own == true)
        {
            if (battleScreen.FieldEffects.SandTomb.Opponent == 0)
            {
                battleScreen.FieldEffects.SandTomb.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " became trapped by Sand Tomb!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.SandTomb.Self == 0)
            {
                battleScreen.FieldEffects.SandTomb.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " became trapped by Sand Tomb!"));
            }
        }
    }

}
