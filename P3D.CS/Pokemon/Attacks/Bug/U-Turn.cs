using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Bug;

public class UTurn : Attack
{
    public UTurn()
    {
        // #Definitions
        type = new Element(Element.Types.Bug);
        ID = 369;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 70;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "U-turn");
        Description = "After making its attack, the user rushes back to switch places with a party Pokémon in waiting.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        swapsOutSelfPokemon = true;
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
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = true;
        isProtectMove = false;


        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = true;
        // #End
    }

    public override void MoveSwitch(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.SelfPokemon.Status != Pokemon.StatusProblems.Fainted)
            {
                if (Core.Player.CountFightablePokemon > 1 && battleScreen.FieldEffects.SwapIndex.Self != battleScreen.SelfPokemonIndex && battleScreen.FieldEffects.SwapIndex.Self != -1)
                {
                    battleScreen.Battle.SwitchOutSelf(battleScreen, battleScreen.FieldEffects.SwapIndex.Self, -1);
                    battleScreen.FieldEffects.SwapIndex.Self = -1;
                }
                else
                {
                    battleScreen.FieldEffects.SwapIndex.Self = -1;
                }
            }
            else
            {
                battleScreen.FieldEffects.SwapIndex.Self = -1;
            }
        }
        else
        {
            if (battleScreen.IsTrainerBattle == true)
            {
                if (battleScreen.IsRemoteBattle == true || battleScreen.IsPVPBattle == true)
                {
                    if (battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.Fainted)
                    {
                        if (battleScreen.Trainer.CountUseablePokemon > 1 && battleScreen.FieldEffects.SwapIndex.Opponent != battleScreen.OpponentPokemonIndex && battleScreen.FieldEffects.SwapIndex.Opponent != -1)
                        {

                            battleScreen.Battle.SwitchOutOpponent(battleScreen, battleScreen.FieldEffects.SwapIndex.Opponent);
                            battleScreen.FieldEffects.SwapIndex.Opponent = -1;
                        }
                        else
                        {
                            battleScreen.FieldEffects.SwapIndex.Opponent = -1;
                        }
                    }
                    else
                    {
                        battleScreen.FieldEffects.SwapIndex.Opponent = -1;
                    }
                }
                else
                {
                    if (battleScreen.Trainer.CountUseablePokemon > 1)
                    {

                        battleScreen.Battle.SwitchOutOpponent(battleScreen, GetPokemonIndex(battleScreen, own));
                    }
                    else
                    {
                        battleScreen.FieldEffects.SwapIndex.Opponent = -1;
                    }
                }
            }
            else
            {
                battleScreen.FieldEffects.SwapIndex.Opponent = -1;
            }
        }
    }

    private int GetPokemonIndex(BattleScreen battleScreen, bool own)
    {
        if (own == true)
        {
            int i = 0;
            while (Core.Player.Pokemons[i].HP <= 0 || Core.Player.Pokemons[i].Status == Pokemon.StatusProblems.Fainted || i == battleScreen.SelfPokemonIndex || Core.Player.Pokemons[i].IsEgg == true)
            {
                i += 1;
            }
            return i;
        }
        else
        {
            int i = 0;
            while (battleScreen.Trainer.Pokemons[i].HP <= 0 || battleScreen.Trainer.Pokemons[i].Status == Pokemon.StatusProblems.Fainted || i == battleScreen.OpponentPokemonIndex || battleScreen.Trainer.Pokemons[i].IsEgg == true)
            {
                i += 1;
            }
            return i;
        }
        return -1;
    }

}
