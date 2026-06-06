using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class BubbleBeam : Attack
{
    public BubbleBeam()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 61;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 65;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Bubble Beam");
        Description = "A spray of bubbles is forcefully ejected at the opposing team. It may also lower their Speed stats.";
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

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanLowerSpeed;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Speed", 1, "", "move:bubblebeam");
        }
    }

}
