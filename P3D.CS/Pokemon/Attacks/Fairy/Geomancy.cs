using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fairy;

public class Geomancy : Attack
{
    public Geomancy()
    {
        // #Definitions
        type = new Element(Element.Types.Fairy);
        ID = 601;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Geomancy");
        Description = "The user absorbs energy and sharply raises its Sp. Atk, Sp. Def, and Speed stats on the next turn.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = false;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.CanRaiseSpAttack;
        aiField2 = AIField.CanRaiseSpDefense;
        aiField3 = AIField.CanRaiseSpeed;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {

        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Attack", 2, "", "move:geomancy");
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Special Defense", 2, "", "move:geomancy");
        battleScreen.Battle.RaiseStat(own, own, battleScreen, "Speed", 2, "", "move:geomancy");
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

        int geomancy = battleScreen.FieldEffects.GeomancyCounter.Self;
        if (own == false)
        {
            geomancy = battleScreen.FieldEffects.GeomancyCounter.Opponent;
        }

        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "power herb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseHeldItem(own, battleScreen) == true)
            {
                if (battleScreen.Battle.RemoveHeldItem(own, own, battleScreen, "Power Herb pushed the use of Geomancy!", "move:geomancy") == true)
                {
                    geomancy = 1;
                }
            }
        }

        if (geomancy == 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is absorbing power!"));
            if (own == true)
            {
                battleScreen.FieldEffects.GeomancyCounter.Self = 1;
            }
            else
            {
                battleScreen.FieldEffects.GeomancyCounter.Opponent = 1;
            }
            return true;
        }
        else
        {
            if (own == true)
            {
                battleScreen.FieldEffects.GeomancyCounter.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.GeomancyCounter.Opponent = 0;
            }
            return false;
        }
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.GeomancyCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.GeomancyCounter.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int geomancy = battleScreen.FieldEffects.GeomancyCounter.Self;
        if (own == false)
        {
            geomancy = battleScreen.FieldEffects.GeomancyCounter.Opponent;
        }

        if (geomancy == 0)
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
            battleScreen.FieldEffects.GeomancyCounter.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.GeomancyCounter.Opponent = 0;
        }
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

}
