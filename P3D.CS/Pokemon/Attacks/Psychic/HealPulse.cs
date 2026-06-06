using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class HealPulse : Attack
{
    public HealPulse()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 505;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Heal Pulse");
        Description = "The user emits a healing pulse which restores the target's HP by up to half of its max HP.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneTarget;
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
        isWonderGuardAffected = false;
        isPulseMove = true;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
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

        if (own == true)
        {
            if (battleScreen.FieldEffects.HealBlock.Opponent > 0)
            {
                int heal = (int)(Math.Ceiling((double)(op.MaxHP / 2)));
                if (p.Ability.Name.ToLower() == "mega launcher")
                {
                    heal = (int)(Math.Ceiling((double)(op.MaxHP * (3 / 4))));
                }
                heal = heal.Clamp(0, 999);

                battleScreen.Battle.GainHP(heal, own == false, own, battleScreen, op.GetDisplayName() + " had its HP restored!", "move:healpulse");
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            if (battleScreen.FieldEffects.HealBlock.Self > 0)
            {
                int heal = (int)(Math.Ceiling((double)(op.MaxHP / 2)));
                if (p.Ability.Name.ToLower() == "mega launcher")
                {
                    heal = (int)(Math.Ceiling((double)(op.MaxHP * (3 / 4))));
                }
                heal = heal.Clamp(0, 999);

                battleScreen.Battle.GainHP(heal, own == false, own, battleScreen, op.GetDisplayName() + " had its HP restored!", "move:healpulse");
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

}
