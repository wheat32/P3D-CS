using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class SteelWing : Attack
{
    public SteelWing()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 211;
        originalPP = 25;
        currentPP = 25;
        maxPP = 25;
        Power = 70;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Steel Wing");
        Description = "The target is hit with wings of steel. It may also raise the user's Defense stat.";
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
        aiField2 = AIField.CanRaiseDefense;

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.RaiseStat(own, own, battleScreen, "Defense", 1, "", "move:steelwing");
        }
    }

}
