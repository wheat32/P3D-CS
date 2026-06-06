using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ghost;

public class Astonish : Attack
{
    public Astonish()
    {
        // #Definitions
        type = new Element(Element.Types.Ghost);
        ID = 310;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 30;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Astonish");
        Description = "The user attacks the target while shouting in a startling fashion. This may also make the target flinch.";
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

        effectChances.Add(30);
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
                battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:astonish");
            }
        }
    }

}
