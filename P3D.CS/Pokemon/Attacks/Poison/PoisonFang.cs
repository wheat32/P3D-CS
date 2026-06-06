using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Poison;

public class PoisonFang : Attack
{
    public PoisonFang()
    {
        // #Definitions
        type = new Element(Element.Types.Poison);
        ID = 305;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 50;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Poison Fang");
        Description = "The user bites the target with toxic fangs. It may also leave the target badly poisoned.";
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
        isJawMove = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanPoison;

        effectChances.Add(50);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);

        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictPoison(own == false, own, battleScreen, true, "", "move:poisonfang");
        }
    }

}
