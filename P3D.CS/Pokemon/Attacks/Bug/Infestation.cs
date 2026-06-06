using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class Infestation : Attack
{
    public Infestation()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 611;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 20;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Infestation");
        Description = "The target is infested and attacked for four to five turns. The target can't flee during this time.";
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
        mirrorMoveAffected = false;
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
            if (battleScreen.FieldEffects.Infestation.Opponent == 0)
            {
                battleScreen.FieldEffects.Infestation.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was trapped by " + p.GetDisplayName() + "!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Infestation.Self == 0)
            {
                battleScreen.FieldEffects.Infestation.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was trapped by " + p.GetDisplayName() + "!"));
            }
        }
    }

}
