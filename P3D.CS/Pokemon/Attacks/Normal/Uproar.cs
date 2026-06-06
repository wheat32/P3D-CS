using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Uproar : Attack
{
    public Uproar()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 253;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 90;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Uproar");
        Description = "The user attacks in an uproar for three turns. Over that time, no one can fall asleep.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override void MoveMultiTurn(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.Uproar.Self == 0)
            {
                battleScreen.FieldEffects.Uproar.Self = 3;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Uproar.Opponent == 0)
            {
                battleScreen.FieldEffects.Uproar.Opponent = 3;
            }
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int uproar = battleScreen.FieldEffects.Uproar.Self;
        if (own == false)
        {
            uproar = battleScreen.FieldEffects.Uproar.Self;
        }

        if (uproar > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Interruption(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (own == true)
        {
            battleScreen.FieldEffects.Uproar.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.Uproar.Opponent = 0;
        }
        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s uproar stopped."));
    }

    public override void MoveHasNoEffect(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveFailsSoundproof(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void IsSleeping(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void HurtItselfInConfusion(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void IsParalyzed(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void IsAttracted(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

}
