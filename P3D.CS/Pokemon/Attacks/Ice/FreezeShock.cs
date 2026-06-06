using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class FreezeShock : Attack
{
    public FreezeShock()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 553;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 140;
        Accuracy = 90;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Freeze Shock");
        Description = "On the second turn, the user hits the target with electrically charged ice. It may leave the target with paralysis.";
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
        aiField3 = AIField.CanParalyze;

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

        int freezeshock = battleScreen.FieldEffects.FreezeShockCounter.Self;
        if (own == false)
        {
            freezeshock = battleScreen.FieldEffects.FreezeShockCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Freeze Shock!", "move:freezeshock") == true)
                {
                    freezeshock = 1;
                }
            }
        }

        if (freezeshock == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " became cloaked in a harsh light!"));
            if (own == true)
            {
                battleScreen.FieldEffects.FreezeShockCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.FreezeShockCounter.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.FreezeShockCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.FreezeShockCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int freezeshock = battleScreen.FieldEffects.FreezeShockCounter.Self;
        if (own == false)
        {
            freezeshock = battleScreen.FieldEffects.FreezeShockCounter.Opponent;
        }

        if (freezeshock == 0)
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
            battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:freezeshock");
        }
    }

}
