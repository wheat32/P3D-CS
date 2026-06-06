using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class GrassWhistle : Attack
{
    public GrassWhistle()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 320;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 55;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Grass Whistle");
        Description = "The user plays a pleasant melody that lulls the target into a deep sleep.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentTargets;
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
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Sleep;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.InflictSleep(own == false, own, battleScreen, -1, "", "move:grasswhistle");
        if (b == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
