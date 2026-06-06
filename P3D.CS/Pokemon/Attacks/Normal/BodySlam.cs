using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class BodySlam : Attack
{
    public BodySlam()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 34;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 85;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Body Slam");
        Description = "The user drops onto the target with its full body weight. It may also leave the target with paralysis.";
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
        aiField2 = AIField.CanParalyze;

        effectChances.Add(30);
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
        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:bodyslam");
        }
    }

}
