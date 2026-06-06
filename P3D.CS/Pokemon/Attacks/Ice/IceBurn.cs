using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class IceBurn : Attack
{
    public IceBurn()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 554;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 140;
        Accuracy = 90;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Ice Burn");
        Description = "On the second turn, an ultracold, freezing wind surrounds the target. This may leave the target with a burn.";
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
        aiField2 = AIField.MultiTurn;
        aiField3 = AIField.CanBurn;

        effectChances.Add(30);
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

        int iceburn = battleScreen.FieldEffects.IceBurnCounter.Self;
        if (own == false)
        {
            iceburn = battleScreen.FieldEffects.IceBurnCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Ice Burn!", "move:iceburn") == true)
                {
                    iceburn = 1;
                }
            }
        }

        if (iceburn == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " became cloaked in a harsh light!"));
            if (own == true)
            {
                battleScreen.FieldEffects.IceBurnCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.IceBurnCounter.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.IceBurnCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.IceBurnCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int iceburn = battleScreen.FieldEffects.IceBurnCounter.Self;
        if (own == false)
        {
            iceburn = battleScreen.FieldEffects.IceBurnCounter.Opponent;
        }

        if (iceburn == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:iceburn");
        }
    }

}
