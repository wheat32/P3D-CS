using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class Submission : Attack
{
    public Submission()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 66;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 80;
        Accuracy = 80;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Submission");
        Description = "The user grabs the target and recklessly dives for the ground. It also hurts the user slightly.";
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
        int recoilDamage = (int)(Math.Floor((double)(lastDamage / 4)));
        if (recoilDamage <= 0)
        {
            recoilDamage = 1;
        }

        battleScreen.Battle.InflictRecoil(own, own, battleScreen, this, recoilDamage, "", "move:submission");
    }

}
