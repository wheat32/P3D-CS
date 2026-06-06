using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class CrushClaw : Attack
{
    public CrushClaw()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 306;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 75;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Crush Claw");
        Description = "The user slashes the target with hard and sharp claws. This may also lower the target's Defense stat.";
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
        kingsrockAffected = false;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
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
        aiField2 = AIField.CanFlinch;

        effectChances.Add(50);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int substitute = battleScreen.FieldEffects.Substitute.Opponent;
        if (own == false)
        {
            substitute = battleScreen.FieldEffects.Substitute.Self;
        }

        if (substitute == 0)
        {
            int chance = GetEffectChance(0, own, battleScreen);

            if (Core.Random.Next(0, 100) < chance)
            {
                battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Defense", 1, "", "move:crushclaw");
            }
        }
    }

}
