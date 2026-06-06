using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Steel;

public class GyroBall : Attack
{
    public GyroBall()
    {
        // #Definitions
        type = new Element(Element.Types.Steel);
        ID = 360;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Gyro Ball");
        Description = "The user tackles the target with a high-speed spin. The slower the user, the greater the damage.";
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
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        isBulletMove = true;
        // #End
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int p_Speed = BattleCalculation.DetermineBattleSpeed(own, battleScreen);
        int op_Speed = BattleCalculation.DetermineBattleSpeed(own == false, battleScreen);

        int basepower = (int)(Math.Ceiling((double)(25 * (op_Speed / p_Speed))));

        basepower = basepower.Clamp(1, 150);

        return basepower;
    }

}
