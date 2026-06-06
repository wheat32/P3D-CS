using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class RockSlide : Attack
{
    public RockSlide()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 157;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 75;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Rock Slide");
        Description = "Large boulders are hurled at the opposing team to inflict damage. They may also make the targets flinch.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.AllAdjacentFoes;
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

        effectChances.Add(30);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (Core.Random.Next(0, 100) < GetEffectChance(0, own, battleScreen))
        {
            battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:rockslide");
        }
    }

}
