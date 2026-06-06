using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Rock;

public class HeadSmash : Attack
{
    public HeadSmash()
    {
        // #Definitions
        type = new Element(Element.Types.Rock);
        ID = 457;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 150;
        Accuracy = 80;
        category = Categories.Physical;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Head Smash");
        Description = "The user attacks the target with a hazardous, full-power headbutt. This also damages the user terribly.";
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
        kingsrockAffected = true;
        counterAffected = true;

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

        battleScreen.Battle.InflictRecoil(own, own, battleScreen, this, recoilDamage, "", "move:headsmash");
    }

}
