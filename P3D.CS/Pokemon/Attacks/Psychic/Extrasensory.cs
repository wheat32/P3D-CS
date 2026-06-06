using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class Extrasensory : Attack
{
    public Extrasensory()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 326;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 80;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Extrasensory");
        Description = "The user attacks with an odd, unseeable power. This may also make the target flinch.";
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

        effectChances.Add(10);
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
                battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:extrasensory");
            }
        }
    }

}
