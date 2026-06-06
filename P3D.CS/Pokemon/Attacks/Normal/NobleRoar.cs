using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class NobleRoar : Attack
{
    public NobleRoar()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 568;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Noble Roar");
        Description = "Letting out a noble roar, the user intimidates the target and lowers its Attack and Sp. Atk. stats.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.LowerAttack;
        aiField2 = AIField.LowerSpAttack;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Attack", 1, "", "move:nobleroar");
        bool d = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Attack", 1, "", "move:nobleroar");
        if (b == false || d == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
