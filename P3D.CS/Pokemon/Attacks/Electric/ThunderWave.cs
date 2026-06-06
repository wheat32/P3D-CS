using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Electric;

public class ThunderWave : Attack
{
    public ThunderWave()
    {
        // #Definitions
        type = new Element(Element.Types.Electric);
        ID = 86;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 90;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Thunder Wave");
        Description = "A weak electric charge is launched at the target. It causes paralysis if it hits.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Paralysis;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.Battle.InflictParalysis(own == false, own, battleScreen, "", "move:thunderwave") == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
