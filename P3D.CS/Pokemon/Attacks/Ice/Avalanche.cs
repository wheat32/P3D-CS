using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class Avalanche : Attack
{
    public Avalanche()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 419;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 60;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Avalanche");
        Description = "The power of this attack move is doubled if the user has been hurt by the target in the same turn.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = -4;
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

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.TurnCounts.Opponent > battleScreen.FieldEffects.TurnCounts.Self)
            {
                if (battleScreen.FieldEffects.LastMove.Opponent != null)
                {
                    if (battleScreen.FieldEffects.LastMove.Opponent.isDamagingMove == true)
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
                    if (battleScreen.FieldEffects.LastMove.Self.isDamagingMove == true)
                    {
                        return Power * 2;
                    }
                }
            }
        }
        return Power;
    }

}
