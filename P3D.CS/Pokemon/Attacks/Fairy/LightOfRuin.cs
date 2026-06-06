using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fairy;

public class LightOfRuin : Attack
{
    public LightOfRuin()
    {
        // #Definitions
        type = new Element(Element.Types.Fairy);
        ID = 617;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 140;
        Accuracy = 90;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Light of Ruin");
        Description = "Drawing power from the Eternal Flower, the user fires a powerful beam of light. This also damages the user quite a lot.";
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
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = true;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.Recoil;
    }

    public override void MoveRecoil(bool own, BattleScreen battleScreen)
    {
        int lastDamage = battleScreen.FieldEffects.LastDamage.Self;
        if (own == false)
        {
            lastDamage = battleScreen.FieldEffects.LastDamage.Opponent;
        }
        int recoilDamage = (int)(Math.Floor((double)(lastDamage / 2)));
        if (recoilDamage <= 0)
        {
            recoilDamage = 1;
        }

        battleScreen.Battle.InflictRecoil(own, own, battleScreen, this, recoilDamage, "", "move:lightofruin");
    }

}
