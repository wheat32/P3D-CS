using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class Spore : Attack
{
    public Spore()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 147;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Spore");
        Description = "The user scatters bursts of spores that induce sleep.";
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
        isHealingMove = false;
        removesSelfFrozen = false;
        isRecoilMove = false;

        immunityAffected = true;
        isDamagingMove = false;
        isProtectMove = false;

        hasSecondaryEffect = false;
        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        isPowderMove = true;
        // #End

        aiField1 = AIField.Sleep;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (battleScreen.Battle.InflictSleep(own == false, own, battleScreen, -1, "", "move:spore") == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
