using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Poison;

public class GunkShot : Attack
{
    public GunkShot()
    {
        // #Definitions
        type = new Element(Element.Types.Poison);
        ID = 441;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 120;
        Accuracy = 80;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Gunk Shot");
        Description = "The user shoots filthy garbage at the target to attack. It may also poison the target.";
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
        aiField2 = AIField.CanPoison;

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);

        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictPoison(own == false, own, battleScreen, false, "", "move:gunkshot");
        }
    }

}
