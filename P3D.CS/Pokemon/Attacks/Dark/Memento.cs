using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class Memento : Attack
{
    public Memento()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 262;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Tough;
        Name = Localization.GetString($"move_name_{ID}", "Memento");
        Description = "The user faints when using this move. In return, it harshly lowers the target's Attack and Sp. Atk.";
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
        immunityAffected = false;
        hasSecondaryEffect = false;
        removesSelfFrozen = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        battleScreen.Battle.ReduceHP(p.HP, own, own, battleScreen, "", "move:memento");

        battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Attack", 2, "", "move:memento");
        battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Attack", 2, "", "move:memento");
    }

}
