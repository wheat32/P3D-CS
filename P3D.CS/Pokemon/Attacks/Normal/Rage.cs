using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Rage : Attack
{
    public Rage()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 99;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 20;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Rage");
        Description = "As long as this move is in use, the power of rage raises the Attack stat each time the user is hit in battle.";
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


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override void MoveSelected(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.LastMove.Self != null)
            {
                if (battleScreen.FieldEffects.LastMove.Self.ID != ID)
                {
                    battleScreen.FieldEffects.RageCounter.Self = 0;
                }
            }
            else
            {
                battleScreen.FieldEffects.RageCounter.Self = 0;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.LastMove.Opponent != null)
            {
                if (battleScreen.FieldEffects.LastMove.Opponent.ID != ID)
                {
                    battleScreen.FieldEffects.RageCounter.Opponent = 0;
                }
            }
            else
            {
                battleScreen.FieldEffects.RageCounter.Opponent = 0;
            }
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.RageCounter.Self == 0)
            {
                battleScreen.FieldEffects.RageCounter.Self = 1;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.RageCounter.Opponent == 0)
            {
                battleScreen.FieldEffects.RageCounter.Opponent = 1;
            }
        }
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int addPower = 0;

        if (own == true)
        {
            addPower = (battleScreen.FieldEffects.RageCounter.Self.Clamp(1, 9) - 1) * 10;
        }
        else
        {
            addPower = (battleScreen.FieldEffects.RageCounter.Opponent.Clamp(1, 9) - 1) * 10;
        }

        return Power + addPower;
    }

}
