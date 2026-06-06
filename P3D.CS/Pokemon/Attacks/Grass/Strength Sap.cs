using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class StrengthSap : Attack
{
    public StrengthSap()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 668;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Strength Sap");
        Description = "The user restores its HP by the same amount as the target's Attack stat. It also lowers the target's Attack stat.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = true;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
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

        bool b = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Attack", 1, "", "move:strengthsap");

        int op_Attack = BattleCalculation.DetermineBattleAttack(own == false, battleScreen);
        int heal = op_Attack;

        if (b == true)
        {
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
                        heal = (int)(Math.Ceiling((double)(heal * (130 / 100))));
                    }
                }

                int healBlock = battleScreen.FieldEffects.HealBlock.Opponent;
                if (own == false)
                {
                    healBlock = battleScreen.FieldEffects.HealBlock.Self;
                }
                if (healBlock == 0)
                {
                    battleScreen.Battle.GainHP(heal, own, own, battleScreen, op.GetDisplayName() + " had its energy drained!", "move:strengthsap");
                }
            }
        }
        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
