using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class JumpKick : Attack
{
    public JumpKick()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 26;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 100;
        Accuracy = 95;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Jump Kick");
        Description = "The user jumps up high, then strikes with a kick. If the kick misses, the user hurts itself.";
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

        disabledWhileGravity = true;
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

    private void InflictCrashDamage(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        battleScreen.Battle.InflictRecoil(own, own, battleScreen, this, (int)(Math.Floor((double)(p.MaxHP / 2))), p.GetDisplayName() + " struggled && crashed!", "move:jumpkick");
    }

    public override void MoveMisses(bool own, BattleScreen battleScreen)
    {
        InflictCrashDamage(own, battleScreen);
    }

    public override void MoveProtectedDetected(bool own, BattleScreen battleScreen)
    {
        InflictCrashDamage(own, battleScreen);
    }

    public override void MoveHasNoEffect(bool own, BattleScreen battleScreen)
    {
        InflictCrashDamage(own, battleScreen);
    }

}
