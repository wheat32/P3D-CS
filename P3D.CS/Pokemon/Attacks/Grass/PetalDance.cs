using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class PetalDance : Attack
{
    public PetalDance()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 80;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 120;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Petal Dance");
        Description = "The user attacks the target by scattering petals for two to three turns. The user then becomes confused.";
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

        isDanceMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.MultiTurn;
        aiField3 = AIField.ConfuseOwn;
    }

    public override void MoveMultiTurn(bool own, BattleScreen battleScreen)
    {
        int currentTurns = battleScreen.FieldEffects.PetalDance.Self;
        if (own == false)
        {
            currentTurns = battleScreen.FieldEffects.PetalDance.Opponent;
        }

        if (currentTurns == 0)
        {
            int turns = Core.Random.Next(2, 4);
            if (own == true)
            {
                battleScreen.FieldEffects.PetalDance.Self = turns;
            }
            else
            {
                battleScreen.FieldEffects.PetalDance.Opponent = turns;
            }
        }
    }

    private void Interruption(bool own, BattleScreen battleScreen)
    {
        int petalDance = 0;
        Pokemon p = null;
        if (own == true)
        {
            petalDance = battleScreen.FieldEffects.PetalDance.Self;
            p = battleScreen.SelfPokemon;
        }
        else
        {
            petalDance = battleScreen.FieldEffects.PetalDance.Opponent;
            p = battleScreen.OpponentPokemon;
        }

        if (petalDance == 1)
        {
            battleScreen.Battle.InflictConfusion(own, own, battleScreen, p.GetDisplayName() + "'s PetalDance stopped.", "move:petaldance");
        }

        if (own == true)
        {
            battleScreen.FieldEffects.PetalDance.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.PetalDance.Opponent = 0;
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int petalDance = battleScreen.FieldEffects.PetalDance.Self;
        if (own == false)
        {
            petalDance = battleScreen.FieldEffects.PetalDance.Opponent;
        }

        if (petalDance > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void MoveHasNoEffect(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        Interruption(own, battleScreen);
    }

    public override void InflictedFlinch(bool own, BattleScreen battleScreen)
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
