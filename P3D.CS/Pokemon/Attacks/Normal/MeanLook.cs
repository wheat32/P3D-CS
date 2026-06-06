using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class MeanLook : Attack
{
    public MeanLook()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 212;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Mean Look");
        Description = "The user pins the target with a dark, arresting look. The target becomes unable to flee.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;

        canHitSleeping = false;
        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        isTrappingMove = true;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int trapped = battleScreen.FieldEffects.TrappedCounter.Opponent;
        if (own == false)
        {
            trapped = battleScreen.FieldEffects.TrappedCounter.Self;
        }

        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            op = battleScreen.SelfPokemon;
        }

        if (trapped == 0)
        {
            if (own == true)
            {
                battleScreen.FieldEffects.TrappedCounter.Opponent = 1;
            }
            else
            {
                battleScreen.FieldEffects.TrappedCounter.Self = 1;
            }
            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " can no longer escape!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
