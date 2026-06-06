using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class FalseSwipe : Attack
{
    public FalseSwipe()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 206;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "False Swipe");
        Description = "A restrained attack that prevents the target from fainting. The target is left with at least 1 HP.";
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

        aiField1 = AIField.Nothing;
        aiField2 = AIField.Nothing;
    }

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        int d = base.GetDamage(critical, own, targetPokemon, battleScreen);

        int subst = battleScreen.FieldEffects.Substitute.Opponent;
        if (own == false)
        {
            subst = battleScreen.FieldEffects.Substitute.Self;
        }

        if (subst == 0)
        {
            Pokemon op = battleScreen.OpponentPokemon;
            if (own == false)
            {
                op = battleScreen.SelfPokemon;
            }

            if (d >= op.HP)
            {
                d = op.HP - 1;
            }
        }

        return d;
    }

}
