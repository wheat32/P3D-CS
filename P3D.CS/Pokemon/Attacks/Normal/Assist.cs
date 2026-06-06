using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Assist : Attack
{
    public Assist()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 274;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Assist");
        Description = "The user hurriedly and randomly uses a move among those known by other Pokémon in the party.";
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
        int[] disabledMoves = {274, 562, 448, 509, 383, 68, 343, 194, 197, 525, 203, 364, 264, 266, 270, 588, 561, 382, 118, 102, 243, 119, 267, 566, 182, 46, 166, 214, 289, 596, 564, 165, 415, 168, 144, 271, 18};

        Pokemon p = battleScreen.SelfPokemon;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
        }

        int countedDisabledMoves = 0;

        List<Attack> attackList = [];
        if (own == true)
        {
            foreach (Pokemon pp in Core.Player.Pokemons)
            {
                attackList.AddRange(pp.Attacks);
            }
        }
        else
        {
            if (battleScreen.IsTrainerBattle)
            {
                foreach (Pokemon pp in battleScreen.Trainer.Pokemons)
                {
                    attackList.AddRange(pp.Attacks);
                }
            }
            else
            {
                attackList.AddRange(battleScreen.OpponentPokemon.Attacks);
            }
        }

        for (int i = 0; i <= attackList.Count - 1; i++)
        {
            if (disabledMoves.Contains(attackList[i].ID) == true)
            {
                countedDisabledMoves += 1;
            }
        }

        if (countedDisabledMoves < attackList.Count)
        {
            int s = Core.Random.Next(0, attackList.Count);
            while (disabledMoves.Contains(attackList[s].ID) == true)
            {
                s = Core.Random.Next(0, attackList.Count);
            }

            Attack m = Attack.GetAttackByID(attackList[s].ID);

            battleScreen.Battle.DoAttackRound(battleScreen, own, m);
        }
        else
        {
            // fail:
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

}
