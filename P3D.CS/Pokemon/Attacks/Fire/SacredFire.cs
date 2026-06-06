using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class SacredFire : Attack
{
    public SacredFire()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 221;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 100;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Sacred Fire");
        Description = "The target is razed with a mystical fire of great intensity. It may also leave the target with a burn.";
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
        kingsrockAffected = false;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = true;
        removesSelfFrozen = true;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Burn;

        effectChances.Add(50);
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int chance = GetEffectChance(0, own, battleScreen);
        if (Core.Random.Next(0, 100) < chance)
        {
            battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:sacredfire");
        }
    }

}
