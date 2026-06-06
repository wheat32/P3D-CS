using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class MirrorShot : Attack
{
    public MirrorShot()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 429;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 65;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Mirror Shot");
        Description = "The user lets loose a flash of energy at the target from its polished body. This may also lower the target's accuracy.";
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

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Accuracy", 1, "", "move:mirrorshot");
        }
    }

}
