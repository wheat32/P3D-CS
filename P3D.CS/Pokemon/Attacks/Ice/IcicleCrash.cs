using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class IcicleCrash : Attack
{
    public IcicleCrash()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 556;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 85;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Icicle Crash");
        Description = "The user attacks by harshly dropping large icicles onto the target. This may also make the target flinch.";
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
                battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:iciclecrash");
            }
        }
    }

}
