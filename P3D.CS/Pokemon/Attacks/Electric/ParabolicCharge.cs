using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class ParabolicCharge : Attack
{
    public ParabolicCharge()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 570;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 65;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Parabolic Charge");
        Description = "The user attacks everything around it. The user's HP is restored by half the damage taken by those hit.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Absorbing;
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

        int damage = battleScreen.FieldEffects.LastDamage.Self;
        if (own == false)
        {
            damage = battleScreen.FieldEffects.LastDamage.Opponent;
        }
        int heal = (int)(Math.Ceiling((double)(damage / 2)));

        if (heal <= 0)
        {
            heal = 1;
        }

        if (op.Ability.Name.ToLower() == "liquid ooze" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
        {
            battleScreen.Battle.ReduceHP(heal, own, own, battleScreen, "Liquid Ooze damaged " + p.GetDisplayName() + "!", "liquidooze");
        }
        else
        {
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
                {
                    heal = (int)(Math.Ceiling((double)(damage * (80 / 100))));
                }
            }

            int healBlock = battleScreen.FieldEffects.HealBlock.Opponent;
            if (own == false)
            {
                healBlock = battleScreen.FieldEffects.HealBlock.Self;
            }
            if (healBlock == 0)
            {
                battleScreen.Battle.GainHP(heal, own, own, battleScreen, op.GetDisplayName() + " had its energy drained!", "move:paraboliccharge");
            }
        }
    }

}
