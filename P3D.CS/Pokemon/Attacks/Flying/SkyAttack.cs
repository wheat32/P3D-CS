using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Flying;

public class SkyAttack : Attack
{
    public SkyAttack()
    {
        // #Definitions
        type = new Element(Element.Types.Flying);
        ID = 143;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 140;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Sky Attack");
        Description = "A second-turn attack move where critical hits land more easily. It may also make the target flinch.";
        criticalChance = 2;
        isHMMove = false;
        target = Targets.OneTarget;
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
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
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
        aiField2 = AIField.MultiTurn;

        effectChances.Add(30);
    }

    public override bool GetUseAccEvasion(bool own, BattleScreen battleScreen)
    {
        int SkyAttack = battleScreen.FieldEffects.SkyAttackCounter.Self;
        if (own == false)
        {
            SkyAttack = battleScreen.FieldEffects.SkyAttackCounter.Opponent;
        }

        if (SkyAttack == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void PreAttack(bool own, BattleScreen battleScreen)
    {
        int SkyAttack = battleScreen.FieldEffects.SkyAttackCounter.Self;
        if (own == false)
        {
            SkyAttack = battleScreen.FieldEffects.SkyAttackCounter.Opponent;
        }

        if (SkyAttack == 0)
        {
            focusOpponentPokemon = false;
        }
        else
        {
            focusOpponentPokemon = true;
        }
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int skyattack = battleScreen.FieldEffects.SkyAttackCounter.Self;
        if (own == false)
        {
            skyattack = battleScreen.FieldEffects.SkyAttackCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Sky Attack!", "move:skyattack") == true)
                {
                    skyattack = 1;
                }
            }
        }

        if (skyattack == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " became cloaked in a harsh light!"));
            if (own == true)
            {
                battleScreen.FieldEffects.SkyAttackCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.SkyAttackCounter.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.SkyAttackCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.SkyAttackCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.SkyAttackCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SkyAttackCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int SkyAttack = battleScreen.FieldEffects.SkyAttackCounter.Self;
        if (own == false)
        {
            SkyAttack = battleScreen.FieldEffects.SkyAttackCounter.Opponent;
        }

        if (SkyAttack == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void MoveFails(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.SkyAttackCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.SkyAttackCounter.Opponent = 0;
        }
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void AbsorbedBySubstitute(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        MoveFails(own, battleScreen);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:skyattack");
        }
    }

}
