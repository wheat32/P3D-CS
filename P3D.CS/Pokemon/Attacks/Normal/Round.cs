using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Round : Attack
{
    public Round()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 496;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 60;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Round");
        Description = "The user attacks the target with a song. Others can join in the Round to increase the power of the attack.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;
        isSoundMove = true;

        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.TurnCounts.Opponent > battleScreen.FieldEffects.TurnCounts.Self)
            {
                if (battleScreen.FieldEffects.LastMove.Opponent != null)
                {
                    if (battleScreen.FieldEffects.LastMove.Opponent.ID == 496)
                    {
                        return Power * 2;
                    }
                }
            }
        }
        else
        {
            if (battleScreen.FieldEffects.TurnCounts.Self > battleScreen.FieldEffects.TurnCounts.Opponent)
            {
                if (battleScreen.FieldEffects.LastMove.Self != null)
                {
                    if (battleScreen.FieldEffects.LastMove.Self.ID == 496)
                    {
                        return Power * 2;
                    }
                }
            }
        }
        return Power;
    }

}
