using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class IronTail : Attack
{
    public IronTail()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 231;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 100;
        Accuracy = 75;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Iron Tail");
        Description = "The target is slammed with a steel-hard tail. It may also lower the target's Defense stat.";
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
        aiField2 = AIField.CanLowerDefense;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Defense", 1, "", "move:irontail");
        }
    }

}
