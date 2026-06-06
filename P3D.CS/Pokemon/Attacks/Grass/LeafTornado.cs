using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class LeafTornado : Attack
{
    public LeafTornado()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 536;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 65;
        Accuracy = 90;
        category = Categories.Special;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Leaf Tornado");
        Description = "The user attacks its target by encircling it in sharp leaves. This attack may also lower the target's accuracy.";
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
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;
        isWindMove = true;
        isSlicingMove = true;
        isDamagingMove = true;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanLowerAccuracy;

        effectChances.Add(50);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Accuracy", 1, "", "move:leaftornado");
        }
    }

}
