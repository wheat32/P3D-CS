using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Psychic;

public class LusterPurge : Attack
{
    public LusterPurge()
    {
        // #Definitions
        type = new Element(Element.Types.Psychic);
        ID = 295;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 70;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Luster Purge");
        Description = "The user lets loose a damaging burst of light. It may also reduce the target's Sp. Def. stat.";
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
        removesSelfFrozen = false;
        hasSecondaryEffect = true;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.CanLowerSpDefense;

        effectChances.Add(50);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Defense", 1, "", "move:lusterpurge");
        }
    }

}
