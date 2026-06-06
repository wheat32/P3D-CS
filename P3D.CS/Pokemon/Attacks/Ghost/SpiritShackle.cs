using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class SpiritShackle : Attack
{
    public SpiritShackle()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 662;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 80;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Spirit Shackle");
        Description = "The user attacks while simultaneously stitching the target's shadow to the ground to prevent the target from escaping.";
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
    }

}
