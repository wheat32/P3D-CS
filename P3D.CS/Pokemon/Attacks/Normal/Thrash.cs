using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Thrash : Attack
{
    public Thrash()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 37;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 120;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Thrash");
        Description = "The user rampages and attacks for two to three turns. It then becomes confused, however.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneTarget;
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
        counterAffected = true;

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
        aiField2 = AIField.MultiTurn;
        aiField2 = AIField.ConfuseOwn;
    }

    public override void MoveMultiTurn(bool own, BattleScreen battleScreen)
    {
        int currentTurns = battleScreen.FieldEffects.Thrash.Self;
        if (own == false)
        {
            currentTurns = battleScreen.FieldEffects.Thrash.Opponent;
        }

        if (currentTurns == 0)
        {
            int turns = Core.Random.Next(2, 4);
            if (own == true)
            {
                battleScreen.FieldEffects.Thrash.Self = turns;
            }
            else
            {
                battleScreen.FieldEffects.Thrash.Opponent = turns;
            }
        }
    }

    private void Interruption(bool own, BattleScreen battleScreen)
    {
        int thrash = 0;
        Pokemon p = null;
        if (own == true)
        {
            thrash = battleScreen.FieldEffects.Thrash.Self;
            p = battleScreen.SelfPokemon;
        }
        else
        {
            thrash = battleScreen.FieldEffects.Thrash.Opponent;
            p = battleScreen.OpponentPokemon;
        }

        if (thrash == 1)
        {
            battleScreen.Battle.InflictConfusion(own, own, battleScreen, p.GetDisplayName() + "'s Thrash stopped.", "move:thrash");
        }
        if (own == true)
        {
            battleScreen.FieldEffects.Thrash.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.Thrash.Opponent = 0;
        }

    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int thrash = battleScreen.FieldEffects.Thrash.Self;
        if (own == false)
        {
            thrash = battleScreen.FieldEffects.Thrash.Opponent;
        }

        if (thrash > 0)
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
