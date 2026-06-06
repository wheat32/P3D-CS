using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Grass;

public class LeechSeed : Attack
{
    public LeechSeed()
    {
        // #Definitions
        type = new Element(Element.Types.Grass);
        ID = 73;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 90;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Leech Seed");
        Description = "A seed is planted on the target. It steals some HP from the target every turn.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
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

        isDamagingMove = false;
        isProtectMove = false;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        isPowderMove = true;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        Pokemon op = battleScreen.OpponentPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
        }

        bool failed = false;

        if (op.Type1.Type != Element.Types.Grass && op.Type2.Type != Element.Types.Grass)
        {
            if (own == true)
            {
                if (battleScreen.FieldEffects.LeechSeed.Opponent == 0)
                {
                    battleScreen.FieldEffects.LeechSeed.Opponent = 1;
                }
                else
                {
                    failed = true;
                }
            }
            else
            {
                if (battleScreen.FieldEffects.LeechSeed.Self == 0)
                {
                    battleScreen.FieldEffects.LeechSeed.Self = 1;
                }
                else
                {
                    failed = true;
                }
            }
        }
        else
        {
            failed = true;
        }

        if (failed == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " was seeded!"));
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
