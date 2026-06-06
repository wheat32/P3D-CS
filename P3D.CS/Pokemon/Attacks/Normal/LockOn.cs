using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class LockOn : Attack
{
    public LockOn()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 199;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Lock-On");
        Description = "The user takes sure aim at the target. It ensures the next attack does not fail to hit the target.";
        criticalChance = 0;
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
        kingsrockAffected = false;
        counterAffected = false;
        disabledWhileGravity = false;
        useEffectiveness = false;
        isHealingMove = false;
        removesSelfFrozen = false;
        isRecoilMove = false;

        immunityAffected = false;
        isDamagingMove = false;
        isProtectMove = false;

        hasSecondaryEffect = false;
        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int lockedOn = battleScreen.FieldEffects.LockOn.Opponent;
        if (own == false)
        {
            lockedOn = battleScreen.FieldEffects.LockOn.Self;
        }

        if (lockedOn == 0)
        {
            Pokemon p = battleScreen.SelfPokemon;
            Pokemon op = battleScreen.OpponentPokemon;
            if (own == false)
            {
                p = battleScreen.OpponentPokemon;
                op = battleScreen.SelfPokemon;
            }

            if (own == true)
            {
                battleScreen.FieldEffects.LockOn.Opponent = 1;
            }
            else
            {
                battleScreen.FieldEffects.LockOn.Self = 1;
            }

            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " took aim on " + op.GetDisplayName() + "!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
