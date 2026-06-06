using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Ice;

public class SheerCold : Attack
{
    public SheerCold()
    {
        // #Definitions
        type = new Element(Element.Types.Ice);
        ID = 329;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Special;
        contestCategory = ContestCategories.Beauty;
        Name = Localization.GetString($"move_name_{ID}", "Sheer Cold");
        Description = "The target is attacked with a blast of absolute-zero cold. The target instantly faints if it hits.";
        criticalChance = 0;
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
        useEffectiveness = false;
        immunityAffected = true;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = true;
        isWonderGuardAffected = true;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.Damage;
        aiField2 = AIField.OHKO;
    }

    public override int GetAccuracy(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        int acc = ((p.Level - op.Level) + 30);
        if (p.IsType(Element.Types.Ice) == false)
        {
            acc = (int)(acc * 0.8F);
        }
        return acc;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        if (op.Level > p.Level)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }
        else
        {
            return false;
        }
    }

}
