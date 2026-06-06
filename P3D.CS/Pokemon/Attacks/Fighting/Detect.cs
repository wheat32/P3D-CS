using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class Detect : Attack
{
    public Detect()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 197;
        originalPP = 5;
        currentPP = 5;
        maxPP = 5;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Detect");
        Description = "It enables the user to evade all attacks. Its chance of failing rises if it is used in succession.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 4;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
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
        isProtectMove = true;


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override bool MoveFailBeforeAttack(bool own, BattleScreen battleScreen)
    {
        double chance = 100D;
        int protects = battleScreen.FieldEffects.ProtectMovesCount.Self;
        if (own == false)
        {
            protects = battleScreen.FieldEffects.ProtectMovesCount.Opponent;
        }

        if (protects > 0)
        {
            for (int i = 1; i <= protects; i++)
            {
                chance /= 2;
            }
        }

        if (Core.Random.Next(0, 100) < chance)
        {
            return false;
        }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            return true;
        }
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            battleScreen.FieldEffects.ProtectMovesCount.Self += 1;

            battleScreen.FieldEffects.DetectCounter.Self = 1;
        }
        else
        {
            battleScreen.FieldEffects.ProtectMovesCount.Opponent += 1;

            battleScreen.FieldEffects.DetectCounter.Opponent = 1;
        }
        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " protected itself!"));
    }

}
