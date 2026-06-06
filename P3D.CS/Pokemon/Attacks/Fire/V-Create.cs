using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class VCreate : Attack
{
    public VCreate()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 557;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 180;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "V-Create");
        Description = "With a hot flame on its forehead, the user hurls itself at its target. This lowers the user's Defense, Sp. Def, and Speed stats.";
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
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
                battleScreen.Battle.LowerStat(own, own, battleScreen, "Defense", 1, "", "move:v-create");
                battleScreen.Battle.LowerStat(own, own, battleScreen, "Special Defense", 1, "", "move:v-create");
                battleScreen.Battle.LowerStat(own, own, battleScreen, "Speed", 1, "", "move:v-create");
    }

}
