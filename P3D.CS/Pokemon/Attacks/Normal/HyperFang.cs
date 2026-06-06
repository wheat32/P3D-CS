using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class HyperFang : Attack
{
    public HyperFang()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 158;
        originalPP = 15;
        currentPP = 15;
        maxPP = 15;
        Power = 80;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Hyper Fang");
        Description = "The user bites hard on the target with its sharp front fangs. It may also make the target flinch.";
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
        isJawMove = true;
        // #End

        effectChances.Add(10);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);

        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:hyperfang");
        }
    }

}
