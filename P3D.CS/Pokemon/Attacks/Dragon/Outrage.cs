using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dragon;

public class Outrage : Attack
{
    public Outrage()
    {
        // #Definitions
        type = new Element(Element.Types.Dragon);
        ID = 200;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 120;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Outrage");
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
        aiField3 = AIField.ConfuseOwn;
    }

    public override void MoveMultiTurn(bool own, BattleScreen battleScreen)
    {
        int currentTurns = battleScreen.FieldEffects.Outrage.Self;
        if (own == false)
        {
            currentTurns = battleScreen.FieldEffects.Outrage.Opponent;
        }

        if (currentTurns == 0)
        {
            int turns = Core.Random.Next(2, 4);
            if (own == true)
            {
                battleScreen.FieldEffects.Outrage.Self = turns;
            }
            else
            {
                battleScreen.FieldEffects.Outrage.Opponent = turns;
            }
        }
    }

    public override bool DeductPP(bool own, BattleScreen battleScreen)
    {
        int outrage = battleScreen.FieldEffects.Outrage.Self;
        if (own == false)
        {
            outrage = battleScreen.FieldEffects.Outrage.Opponent;
        }

        if (outrage > 0)
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
        int outrage = 0;
        Pokemon p = null;
        if (own == true)
        {
            outrage = battleScreen.FieldEffects.Outrage.Self;
            p = battleScreen.SelfPokemon;
        }
        else
        {
            outrage = battleScreen.FieldEffects.Outrage.Opponent;
            p = battleScreen.OpponentPokemon;
        }

        if (outrage == 1)
        {
            battleScreen.Battle.InflictConfusion(own, own, battleScreen, p.GetDisplayName() + "'s Outrage stopped.", "move:outrage");
        }

        if (own == true)
        {
            battleScreen.FieldEffects.Outrage.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.Outrage.Opponent = 0;
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
