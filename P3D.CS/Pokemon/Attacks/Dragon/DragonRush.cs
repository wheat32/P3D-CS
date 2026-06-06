using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dragon;

public class DragonRush : Attack
{
    public DragonRush()
    {
        // #Definitions
        type = new Element(Element.Types.Dragon);
        ID = 407;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 100;
        Accuracy = 75;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Dragon Rush");
        Description = "The user tackles the target while exhibiting overwhelming menace. It may also make the target flinch.";
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
        aiField2 = AIField.CanFlinch;

        effectChances.Add(20);
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        int minimize = battleScreen.FieldEffects.Minimize.Opponent;
        if (own == false)
        {
            minimize = battleScreen.FieldEffects.Minimize.Self;
        }
        if (minimize > 0)
        {
            return Power * 2;
        }
        else
        {
            return Power;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:dragonrush");
        }
    }

}
