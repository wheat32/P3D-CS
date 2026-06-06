using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class FireSpin : Attack
{
    public FireSpin()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 83;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 35;
        Accuracy = 85;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Fire Spin");
        Description = "The target becomes trapped within a fierce vortex of fire that rages for four to five turns.";
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
            if (battleScreen.FieldEffects.FireSpin.Opponent == 0)
            {
                battleScreen.FieldEffects.FireSpin.Opponent = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was trapped in the fiery vortex!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.FireSpin.Self == 0)
            {
                battleScreen.FieldEffects.FireSpin.Self = turns;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was trapped in the fiery vortex!"));
            }
        }
    }

}
