using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class BatonPass : Attack
{
    public BatonPass()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 226;
        originalPP = 40;
        currentPP = 40;
        maxPP = 40;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Cute;
        Name = Localization.GetString($"move_name_{ID}", "Baton Pass");
        Description = "The user switches places with a party Pokémon in waiting and passes along any stat changes.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.Self;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        swapsOutSelfPokemon = true;
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
        if (own == true)
        {
            if (battleScreen.SelfPokemon.Status != Pokemon.StatusProblems.Fainted)
            {
                if (Core.Player.CountFightablePokemon > 1 && battleScreen.FieldEffects.BatonPassIndex.Self != battleScreen.SelfPokemonIndex && battleScreen.FieldEffects.BatonPassIndex.Self != -1)
                {
                    battleScreen.FieldEffects.UsedBatonPass.Self = true;

                    battleScreen.Battle.SwitchOutOwn(battleScreen, battleScreen.FieldEffects.BatonPassIndex.Self, -1);
                    battleScreen.FieldEffects.BatonPassIndex.Self = -1;
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                }
            }
            else
            {
                battleScreen.FieldEffects.BatonPassIndex.Self = -1;
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
                        battleScreen.FieldEffects.UsedBatonPass.Opponent = true;
                        if (battleScreen.Trainer.CountUseablePokemon > 1 && battleScreen.FieldEffects.BatonPassIndex.Opponent != battleScreen.OpponentPokemonIndex && battleScreen.FieldEffects.BatonPassIndex.Opponent != -1)
                        {
                            battleScreen.FieldEffects.UsedBatonPass.Opponent = true;

                            battleScreen.Battle.SwitchOutOpp(battleScreen, battleScreen.FieldEffects.BatonPassIndex.Opponent);
                            battleScreen.FieldEffects.BatonPassIndex.Opponent = -1;
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                        }
                    }
                    else
                    {
                        battleScreen.FieldEffects.BatonPassIndex.Opponent = -1;
                    }
                }
                else
                {
                    if (battleScreen.Trainer.CountUseablePokemon > 1)
                    {
                        battleScreen.FieldEffects.UsedBatonPass.Opponent = true;

                        battleScreen.Battle.SwitchOutOpp(battleScreen, GetPokemonIndex(battleScreen, own));
                    }
                    else
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                    }
                }
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
            }
        }
    }

    private int GetPokemonIndex(BattleScreen battleScreen, bool own)
    {
        if (own == true)
        {
            int i = 0;
            while (Core.Player.Pokemons[i].HP <= 0 || Core.Player.Pokemons[i].Status == Pokemon.StatusProblems.Fainted || i == battleScreen.SelfPokemonIndex || Core.Player.Pokemons[i].IsEgg)
            {
                i += 1;
            }
            return i;
        }
        else
        {
            int i = 0;
            while (battleScreen.Trainer.Pokemons[i].HP <= 0 || battleScreen.Trainer.Pokemons[i].Status == Pokemon.StatusProblems.Fainted || i == battleScreen.SelfPokemonIndex || battleScreen.Trainer.Pokemons[i].IsEgg)
            {
                i += 1;
            }
            return i;
        }
        return -1;
    }

}
