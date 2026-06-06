using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class SleepTalk : Attack
{
    public SleepTalk()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 214;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Sleep Talk");
        Description = "While it is asleep, the user randomly uses one of the moves it knows.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = false;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = false;
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


        isAffectedBySubstitute = false;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        int[] disabledMoves = {214, 274, 117, 340, 448, 383, 91, 291, 19, 264, 382, 118, 119, 467, 166, 130, 143, 76, 13, 253};

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (p.Status == Pokemon.StatusProblems.Sleep)
        {
            int countedDisabledMoves = 0;

            for (int i = 0; i <= p.Attacks.Count - 1; i++)
            {
                if (disabledMoves.Contains(p.Attacks[i].ID) == true)
                {
                    countedDisabledMoves += 1;
                }
            }

            if (countedDisabledMoves < p.Attacks.Count)
            {
                int s = Core.Random.Next(0, p.Attacks.Count);
                while (disabledMoves.Contains(p.Attacks[s].ID) == true)
                {
                    s = Core.Random.Next(0, p.Attacks.Count);
                }

                Attack m = Attack.GetAttackByID(p.Attacks[s].ID);
                battleScreen.Battle.DoAttackRound(battleScreen, own, m);
            }
            else
            {
                // fail:
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
        else
        {
            // fail:
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
