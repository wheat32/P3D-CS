using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class FakeOut : Attack
{
    public FakeOut()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 252;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 40;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Fake Out");
        Description = "An attack that hits first and makes the target flinch. It only works the first turn the user is in battle.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 3;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = true;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = false;
        counterAffected = true;

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
        aiField2 = AIField.Flinch;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        int turns = battleScreen.FieldEffects.PokemonTurns.Self;
        if (own == false)
        {
            turns = battleScreen.FieldEffects.PokemonTurns.Opponent;
        }

        if (turns > 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        battleScreen.Battle.InflictFlinch(own == false, own, battleScreen, "", "move:fakeout");
    }

}
