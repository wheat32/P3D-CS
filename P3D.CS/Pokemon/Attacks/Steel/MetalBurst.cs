using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class MetalBurst : Attack
{
    public MetalBurst()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 368;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Metal Burst");
        Description = "The user retaliates with much greater power against the target that last inflicted damage on it.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.Self;
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
        aiField2 = AIField.Nothing;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.MovesFirst(own))
        {
            return true;
        }
        int damage = battleScreen.FieldEffects.LastDamage.Self;
        if (own == true)
        {
            damage = battleScreen.FieldEffects.LastDamage.Opponent;
        }
        if (damage > 0)
        {
            Attack lastMove = battleScreen.FieldEffects.LastMove.Self;
            if (own == true)
            {
                lastMove = battleScreen.FieldEffects.LastMove.Opponent;
            }
            if (lastMove != null)
            {
                if (lastMove.Category == Categories.Special || lastMove.Category == Categories.Physical)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override int GetDamage(bool critical, bool own, bool targetPokemon, BattleScreen battleScreen, String extraParameter = "", Attack typeEffectivenessAttack = null)
    {
        if (own == true)
        {
            return (int)(battleScreen.FieldEffects.LastDamage.Opponent * 1.5);
        }
        else
        {
            return (int)(battleScreen.FieldEffects.LastDamage.Self * 1.5);
        }
    }

}
