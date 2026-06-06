using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class WringOut : Attack
{
    public WringOut()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 378;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 100;
        category = Categories.Special;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Wring Out");
        Description = "The user powerfully wrings the target. The more HP the target has, the greater the move's power.";
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
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.SelfPokemon;
        }

        return (int)(120 * (p.HP / p.MaxHP));

    }

}
