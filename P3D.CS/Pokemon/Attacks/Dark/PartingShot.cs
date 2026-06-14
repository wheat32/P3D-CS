using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class PartingShot : Attack
{
    public PartingShot()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 575;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 100;
        category = Categories.Status;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Parting Shot");
        Description = "With a parting threat, the user lowers the target's Attack and Sp. Atk. stats. Then it switches with a party Pokémon.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        swapsOutSelfPokemon = true;
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = true;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = false;

        disabledWhileGravity = false;
        useEffectiveness = false;
        immunityAffected = false;
        removesSelfFrozen = false;
        hasSecondaryEffect = false;

        isHealingMove = false;
        isRecoilMove = false;

        isDamagingMove = false;
        isProtectMove = false;
        isSoundMove = true;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        // #End
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        bool b = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Attack", 1, "", "move:partingshot");
        bool d = battleScreen.Battle.LowerStat(own == false, own, battleScreen, "Special Attack", 1, "", "move:partingshot");
        if (b == false && d == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
        }
    }

    public override void MoveSwitch(bool own, BattleScreen battleScreen)
    {
        if (own == true)
        {
            if (battleScreen.SelfPokemon.Status != Pokemon.StatusProblems.Fainted)
            {
                if (Core.Player.CountFightablePokemon > 1 && battleScreen.FieldEffects.SwapIndex.Self != battleScreen.SelfPokemonIndex && battleScreen.FieldEffects.SwapIndex.Self != -1)
                {
                    battleScreen.Battle.SwitchOutOwn(battleScreen, battleScreen.FieldEffects.SwapIndex.Self, -1);
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

                            battleScreen.Battle.SwitchOutOpp(battleScreen, battleScreen.FieldEffects.SwapIndex.Opponent);
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

                        battleScreen.Battle.SwitchOutOpp(battleScreen, GetPokemonIndex(battleScreen, own));
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
