using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Water;

public class MuddyWater : Attack
{
    public MuddyWater()
    {
        // #Definitions
        type = new Element(Element.Types.Water);
        ID = 330;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 90;
        Accuracy = 85;
        category = Categories.Special;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Muddy Water");
        Description = "The user attacks by shooting muddy water at the opposing team. It may also lower the targets' accuracy.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = false;
        kingsrockAffected = true;
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
        aiField2 = AIField.CanLowerAccuracy;

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Accuracy", 1, "", "move:muddywater");
        }
    }

}
