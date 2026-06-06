using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fire;

public class Inferno : Attack
{
    public Inferno()
    {
        // #Definitions
        type = new Element(Element.Types.Fire);
        ID = 517;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 100;
        Accuracy = 50;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Inferno");
        Description = "The user attacks by engulfing the target in an intense fire. It leaves the target with a burn.";
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
        aiField2 = AIField.Burn;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.InflictBurn(own == false, own, battleScreen, "", "move:inferno");
    }

}
