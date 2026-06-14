using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D;
using P3D.BattleSystem.Moves;
using P3D.Items;

namespace P3D.BattleSystem;

public class Battle
{
    public static bool Won = false;
    public static bool Fled = false;
    public static bool Caught = false;
    public static BattleRoundConst OwnStep = new BattleRoundConst();
    public static BattleRoundConst OppStep = new BattleRoundConst();

    public bool skipTurn = false;
    public bool isAfterFaint = false;
    public bool wildHasEscaped = false;
    public bool selectedMoveOwn = true;
    public bool selectedMoveOpp = true;

    private bool _hasSwitchedInOwn = false;
    private bool _hasSwitchedInOpp = false;

    public enum EndBattleReasons
    {
        WinWild, LoseWild, WinTrainer, LoseTrainer, WinPvP, LosePvP
    }

    private Attack GetPokemonMoveFromID(Pokemon pokemon, int moveID, BattleScreen battleScreen, bool own)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.UsedRandomMove.Self == true)
            {
                foreach (Attack a in pokemon.Attacks)
                {
                    if (a.Name == Localization.GetString("move_name_118", "Metronome") || a.isGameModeMove && a.gmUseRandomMove == true)
                    {
                        if (a.CurrentPP > 0)
                        {
                            a.CurrentPP -= 1;
                            break;
                        }
                    }
                }
            }
            if (battleScreen.FieldEffects.UsedMirrorMove.Self == true)
            {
                foreach (Attack a in pokemon.Attacks)
                {
                    if (a.Name == Localization.GetString("move_name_119", "Mirror Move"))
                    {
                        if (a.CurrentPP > 0)
                        {
                            a.CurrentPP -= 1;
                            break;
                        }
                    }
                }
            }
            if (battleScreen.FieldEffects.UsedRandomMove.Self == true || battleScreen.FieldEffects.UsedMirrorMove.Self == true)
            {
                return Attack.GetAttackByID(moveID);
            }
        }
        else
        {
            if (battleScreen.FieldEffects.UsedRandomMove.Opponent == true)
            {
                foreach (Attack a in pokemon.Attacks)
                {
                    if (a.Name == Localization.GetString("move_name_118", "Metronome") || a.isGameModeMove && a.gmUseRandomMove == true)
                    {
                        if (a.CurrentPP > 0)
                        {
                            a.CurrentPP -= 1;
                            break;
                        }
                    }
                }
            }
            if (battleScreen.FieldEffects.UsedMirrorMove.Opponent == true)
            {
                foreach (Attack a in pokemon.Attacks)
                {
                    if (a.Name == Localization.GetString("move_name_119", "Mirror Move"))
                    {
                        if (a.CurrentPP > 0)
                        {
                            a.CurrentPP -= 1;
                            break;
                        }
                    }
                }
            }
            if (battleScreen.FieldEffects.UsedRandomMove.Opponent == true || battleScreen.FieldEffects.UsedMirrorMove.Opponent == true)
            {
                return Attack.GetAttackByID(moveID);
            }
        }

        foreach (Attack a in pokemon.Attacks)
        {
            if (a.ID == moveID)
            {
                return a;
            }
        }
        return pokemon.Attacks[0];
    }

    public void StartMultiTurnAction(BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.Recharge.Self > 0)
        {
            battleScreen.FieldEffects.Recharge.Self -= 1;
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Text, Argument = battleScreen.SelfPokemon.GetDisplayName() + " needs to recharge!" });
            return;
        }

        if (battleScreen.FieldEffects.RolloutCounter.Self > 0)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 205, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.IceBallCounter.Self > 0)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 301, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.FlyCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 19, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.DigCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 91, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.Outrage.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 200, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.Thrash.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 37, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.PetalDance.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 80, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.BounceCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 340, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.DiveCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 291, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.ShadowForceCounter.Self == 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 467, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.PhantomForceCounter.Self == 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 566, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.GeomancyCounter.Self == 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 601, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.SolarBeam.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 76, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.SolarBlade.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 669, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.SkyAttackCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 143, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.SkullBashCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 130, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.RazorWindCounter.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 13, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.Uproar.Self >= 1)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 253, battleScreen, true) });
            return;
        }

        if (battleScreen.FieldEffects.BideCounter.Self > 0)
        {
            selectedMoveOwn = false;
            DeleteHostQuery(battleScreen);
            InitializeRound(battleScreen, new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.SelfPokemon, 117, battleScreen, true) });
            return;
        }

        if (battleScreen.IsRemoteBattle == true)
        {
            battleScreen.BattleMenu.Visible = true;
        }
    }

    public void DeleteHostQuery(BattleScreen battleScreen)
    {
        if (battleScreen.IsRemoteBattle)
        {
            battleScreen.BattleQuery.Clear();
            battleScreen.TempPVPBattleQuery.Clear();
        }
    }

    public void StartRound(BattleScreen battleScreen)
    {
        if (battleScreen.OwnFaint || (battleScreen.OppFaint && battleScreen.IsRemoteBattle))
        {
            isAfterFaint = true;
        }
        battleScreen.BattleMenu.MenuState = BattleMenu.MenuStates.Main;
        selectedMoveOwn = true;
        selectedMoveOpp = true;

        if (battleScreen.IsRemoteBattle == false)
        {
            if (Core.Player.BattleStyle == 1 || battleScreen.ShiftCanContinue == true)
            {
                if (battleScreen.HasSwitchedOwn == false)
                {
                    StartMultiTurnAction(battleScreen);
                }
            }
        }

        if (battleScreen.IsRemoteBattle && battleScreen.IsHost)
        {
            battleScreen.BattleQuery.Add(new TriggerNewRoundPVPQueryObject());
            battleScreen.SendHostQuery();
        }
        battleScreen.BattleQuery.Add(new ToggleMenuQueryObject(false));
        for (int i = 0; i <= 99; i++)
        {
            battleScreen.InsertCasualCameramove();
        }
        battleScreen.HasSwitchedOwn = false;
    }

    public BattleRoundConst GetOppStep(BattleScreen battleScreen, BattleRoundConst ownStep)
    {
        if (BattleScreen.RoamingBattle)
        {
            battleScreen.FieldEffects.RoamingFled = false;
            if (BattleCalculation.CanSwitch(battleScreen, false))
            {
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Flee, Argument = battleScreen.OpponentPokemon.GetDisplayName() + " fled!" };
            }
        }

        if (battleScreen.BattleMode == BattleScreen.BattleModes.Safari)
        {
            return BattleCalculation.SafariRound(battleScreen);
        }

        if ((battleScreen.IsRemoteBattle == false) && battleScreen.OwnFaint)
        {
            return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Text, Argument = "You have sent your next Pokemon!" };
        }

        if (isAfterFaint == false)
        {
            if (battleScreen.FieldEffects.Recharge.Opponent > 0)
            {
                selectedMoveOpp = false;
                battleScreen.FieldEffects.Recharge.Opponent -= 1;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Text, Argument = battleScreen.OpponentPokemon.GetDisplayName() + " needs to recharge!" };
            }

            if (battleScreen.FieldEffects.RolloutCounter.Opponent > 0)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 205, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.IceBallCounter.Opponent > 0)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 301, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.FlyCounter.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 19, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.DigCounter.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 91, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.Outrage.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 200, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.Thrash.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 37, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.PetalDance.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 80, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.BounceCounter.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 340, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.DiveCounter.Opponent == 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 291, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.ShadowForceCounter.Opponent == 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 467, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.PhantomForceCounter.Opponent == 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 566, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.GeomancyCounter.Opponent == 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 601, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.SolarBeam.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 76, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.SolarBlade.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 669, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.SkyAttackCounter.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 143, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.SkullBashCounter.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 130, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.RazorWindCounter.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 13, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.Uproar.Opponent >= 1)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 253, battleScreen, false) };
            }

            if (battleScreen.FieldEffects.BideCounter.Opponent > 0)
            {
                selectedMoveOpp = false;
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, 117, battleScreen, false) };
            }
        }

        if (battleScreen.IsRemoteBattle && battleScreen.IsHost)
        {
            battleScreen.OpponentStatistics.Turns += 1;
            battleScreen.SelfStatistics.Turns += 1;
            if (BattleScreen.ReceivedInput.StartsWith("MOVE|") || BattleScreen.ReceivedInput.StartsWith("MEGA|"))
            {
                battleScreen.OpponentStatistics.Moves += 1;
                if (BattleScreen.ReceivedInput.StartsWith("MEGA|"))
                {
                    battleScreen.IsMegaEvolvingOpp = true;
                }
                int moveID;
                String inputString = BattleScreen.ReceivedInput.Remove(0, 5);
                if (inputString.Contains(";BATON;"))
                {
                    battleScreen.FieldEffects.BatonPassIndex.Opponent = int.Parse(inputString.GetSplit(2, ";"));
                    moveID = int.Parse(inputString.GetSplit(0, ";"));
                }
                else if (inputString.Contains(";SWAP;"))
                {
                    battleScreen.FieldEffects.SwapIndex.Opponent = int.Parse(inputString.GetSplit(2, ";"));
                    moveID = int.Parse(inputString.GetSplit(0, ";"));
                }
                else
                {
                    moveID = int.Parse(inputString);
                }
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = GetPokemonMoveFromID(battleScreen.OpponentPokemon, moveID, battleScreen, false) };
            }
            else if (BattleScreen.ReceivedInput.StartsWith("SWITCH|"))
            {
                battleScreen.OpponentStatistics.Switches += 1;
                int switchID = int.Parse(BattleScreen.ReceivedInput.Remove(0, 7));
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Switch, Argument = switchID.ToString() };
            }
            else if (BattleScreen.ReceivedInput.StartsWith("TEXT|"))
            {
                String text = BattleScreen.ReceivedInput.Remove(0, 5);
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Text, Argument = text };
            }
        }

        if (battleScreen.IsTrainerBattle && (battleScreen.IsRemoteBattle == false))
        {
            AI_MegaEvolve(battleScreen);
            return TrainerAI.GetAIMove(battleScreen, ownStep);
        }
        else
        {
            List<int> availableAttacks = new List<int>();
            for (int i = 0; i <= battleScreen.OpponentPokemon.Attacks.Count - 1; i++)
            {
                availableAttacks.Add(i);
            }
            int oppAttackChoice = Core.Random.Next(0, availableAttacks.Count);
            if (battleScreen.FieldEffects.Encore.Opponent > 0)
            {
                int attackIndex = -1;
                for (int a = 0; a <= battleScreen.OpponentPokemon.Attacks.Count - 1; a++)
                {
                    if (battleScreen.OpponentPokemon.Attacks[a].ID == battleScreen.FieldEffects.EncoreMove.Opponent.ID)
                    {
                        attackIndex = a;
                    }
                }
                if (attackIndex != -1 && battleScreen.OpponentPokemon.Attacks[attackIndex].CurrentPP > 0)
                {
                    return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = battleScreen.FieldEffects.EncoreMove.Opponent };
                }
                else
                {
                    battleScreen.FieldEffects.EncoreMove.Opponent = null;
                    battleScreen.FieldEffects.Encore.Opponent = 0;
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s encore stopped."));
                }
            }
            bool ready = false;
            while (ready == false)
            {
                if (battleScreen.OpponentPokemon.Attacks[oppAttackChoice] == battleScreen.FieldEffects.TormentMove.Opponent || battleScreen.OpponentPokemon.Attacks[oppAttackChoice].Disabled > 0 || battleScreen.FieldEffects.Taunt.Opponent > 0 && battleScreen.OpponentPokemon.Attacks[oppAttackChoice].Category == Attack.Categories.Status || battleScreen.OpponentPokemon.Attacks[oppAttackChoice].CurrentPP <= 0)
                {
                    availableAttacks.Remove(oppAttackChoice);
                    if (availableAttacks.Count > 0)
                    {
                        oppAttackChoice = availableAttacks[Core.Random.Next(0, availableAttacks.Count)];
                    }
                    else
                    {
                        return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = Attack.GetAttackByID(165) };
                    }
                }
                else
                {
                    ready = true;
                }
            }
            return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = battleScreen.OpponentPokemon.Attacks[oppAttackChoice] };
        }
    }

    private BattleRoundConst GetAttack(BattleScreen battleScreen, bool own, Attack move)
    {
        if (own == true)
        {
            if (battleScreen.FieldEffects.UsedRandomMove.Self == true && (move.Name.ToLower() != Localization.GetString("move_name_118", "Metronome").ToLower() || move.isGameModeMove == true && move.gmUseRandomMove == false))
            {
                battleScreen.FieldEffects.UsedRandomMove.Self = false;
                battleScreen.FieldEffects.UsedRandomMoveAttack.Opponent = null;
            }
            if (battleScreen.FieldEffects.UsedMirrorMove.Self == true && (move.Name.ToLower() != Localization.GetString("move_name_119", "Mirror Move").ToLower()))
            {
                battleScreen.FieldEffects.UsedMirrorMove.Self = false;
                battleScreen.FieldEffects.UsedMirrorMoveAttack.Opponent = null;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.UsedRandomMove.Opponent == true && (move.Name.ToLower() != Localization.GetString("move_name_118", "Metronome").ToLower() || move.isGameModeMove == true && move.gmUseRandomMove == false))
            {
                battleScreen.FieldEffects.UsedRandomMove.Opponent = false;
                battleScreen.FieldEffects.UsedRandomMoveAttack.Opponent = null;
            }
            if (battleScreen.FieldEffects.UsedMirrorMove.Opponent == true && (move.Name.ToLower() != Localization.GetString("move_name_119", "Mirror Move").ToLower()))
            {
                battleScreen.FieldEffects.UsedMirrorMove.Opponent = false;
                battleScreen.FieldEffects.UsedMirrorMoveAttack.Opponent = null;
            }
        }

        if (move.isGameModeMove == true && move.gmUseRandomMove == true)
        {
            if (move.CurrentPP > 0)
            {
                move.CurrentPP -= 1;
            }
            if (own == true)
            {
                battleScreen.FieldEffects.UsedRandomMove.Self = true;
                battleScreen.FieldEffects.UsedRandomMoveAttack.Self = move;
            }
            else
            {
                battleScreen.FieldEffects.UsedRandomMove.Opponent = true;
                battleScreen.FieldEffects.UsedRandomMoveAttack.Opponent = move;
            }
            return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = move.GetRandomAttack() };
        }
        else
        {
            if (move.Name.ToLower() == Localization.GetString("move_name_118", "Metronome").ToLower())
            {
                if (move.CurrentPP > 0)
                {
                    move.CurrentPP -= 1;
                }
                if (own == true)
                {
                    battleScreen.FieldEffects.UsedRandomMove.Self = true;
                    battleScreen.FieldEffects.UsedRandomMoveAttack.Self = move;
                }
                else
                {
                    battleScreen.FieldEffects.UsedRandomMove.Opponent = true;
                    battleScreen.FieldEffects.UsedRandomMoveAttack.Opponent = move;
                }
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = Moves.Normal.Metronome.GetMetronomeMove() };
            }
            else if (move.Name.ToLower() == Localization.GetString("move_name_119", "Mirror Move").ToLower())
            {
                if (move.CurrentPP > 0)
                {
                    move.CurrentPP -= 1;
                }

                int id = -1;
                if (own == true)
                {
                    if (battleScreen.FieldEffects.LastMove.Opponent != null && battleScreen.FieldEffects.LastMove.Opponent.mirrorMoveAffected == true)
                    {
                        id = battleScreen.FieldEffects.LastMove.Opponent.ID;
                    }
                }
                else
                {
                    if (battleScreen.FieldEffects.LastMove.Self != null && battleScreen.FieldEffects.LastMove.Self.mirrorMoveAffected == true)
                    {
                        id = battleScreen.FieldEffects.LastMove.Self.ID;
                    }
                }

                if (id != -1)
                {
                    if (own == true)
                    {
                        battleScreen.FieldEffects.UsedMirrorMove.Self = true;
                    }
                    else
                    {
                        battleScreen.FieldEffects.UsedMirrorMove.Opponent = true;
                    }
                    return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = Attack.GetAttackByID(id) };
                }
                else
                {
                    return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Text, Argument = "Mirror Move failed!" };
                }
            }
            else
            {
                return new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Move, Argument = move };
            }
        }
    }

    public void AI_MegaEvolve(BattleScreen battleScreen)
    {
        for (int i = 0; i <= battleScreen.Trainer.Pokemons.Count - 1; i++)
        {
            String str = battleScreen.Trainer.Pokemons[i].AdditionalData;
            if (str == "mega" || str == "mega_x" || str == "mega_y")
            {
                return;
            }
        }
        Pokemon p = battleScreen.OpponentPokemon;
        if (p.Item != null)
        {
            if (p.Item.IsGameModeItem == true)
            {
                if (p.Item.gmIsMegaStone == true)
                {
                    if (p.Number == ((GameModeItem)p.Item).gmMegaPokemonNumber)
                    {
                        battleScreen.IsMegaEvolvingOpp = true;
                    }
                }
            }
            else
            {
                if (p.Item.IsMegaStone == true)
                {
                    MegaStone megaStone = (MegaStone)p.Item;
                    if (p.Number == megaStone.MegaPokemonNumber)
                    {
                        battleScreen.IsMegaEvolvingOpp = true;
                    }
                }
            }
        }
    }

    public void DoMegaEvolution(BattleScreen battleScreen, bool own)
    {
        Pokemon p = battleScreen.SelfPokemon;
        NPC pNPC = battleScreen.SelfPokemonNPC;
        if (own == false)
        {
            p = battleScreen.OpponentPokemon;
            pNPC = battleScreen.OpponentPokemonNPC;
        }
        String baseName = p.GetDisplayName();
        if (p.AdditionalData == String.Empty)
        {
            if (p.Item.IsGameModeItem == false)
            {
                switch (p.Item.ID)
                {
                    case 516:
                    case 529:
                        p.AdditionalData = "mega_x";
                        break;
                    case 517:
                    case 530:
                        p.AdditionalData = "mega_y";
                        break;
                    default:
                        p.AdditionalData = "mega";
                        break;
                }
            }
            else
            {
                p.AdditionalData = "mega";
            }
            p.ReloadDefinitions();
            p.CalculateStats();
            p.LoadAltAbility();
            ChangeCameraAngle(1, own, battleScreen);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                AnimationQueryObject megaAnimation = new AnimationQueryObject(pNPC, own == false);
                megaAnimation.AnimationPlaySound(@"Battle\Effects\MegaEvolution", 0, 0);

                for (int currentAmount = 0; currentAmount <= 16; currentAmount++)
                {
                    Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\MegaEvolution\Mega_Phase1");
                    float xPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                    float zPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                    Vector3 position = new Vector3(xPos, 0.8f, zPos);
                    Vector3 scale = new Vector3(0.5f);
                    float startDelay = (float)(5.0 * Core.Random.NextDouble());
                    Entity phase1Entity = megaAnimation.SpawnEntity(position, texture, scale, 1.0f, startDelay);
                    Vector3 destination = new Vector3(0, 0, 0);
                    megaAnimation.AnimationMove(phase1Entity, true, destination.X, destination.Y, destination.Z, 0.05f, false, true, startDelay, 0.0f);
                }

                Entity phase2Entity = megaAnimation.SpawnEntity(new Vector3(0), TextureManager.GetTexture(@"Textures\Battle\MegaEvolution\Mega_Phase2"), new Vector3(0.0f), 1.0f, 4.0f, 0.0f);
                megaAnimation.AnimationScale(phase2Entity, false, 1.25f, 1.25f, 1.25f, 0.02f, 4.0f, 0.0f);
                megaAnimation.AnimationRotate(phase2Entity, true, 0, 0, 0.1f, 0, 0, 10.0f, 4, 0f, false);
                battleScreen.BattleQuery.Add(megaAnimation);
            }
            else
            {
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\MegaEvolution", false));
            }
            battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(own, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(p, true), 0, 1, -1, -1));
            battleScreen.BattleQuery.Add(new TextQueryObject(baseName + " has Mega Evolved into " + p.GetName(true) + "!"));
            TriggerAbilityEffect(battleScreen, own);
        }
    }

    public void MegaEvolCheck(BattleScreen battleScreen)
    {
        if ((battleScreen.IsMegaEvolvingOwn || battleScreen.IsMegaEvolvingOpp) == false)
        {
            return;
        }
        if (BattleCalculation.MovesFirst(battleScreen))
        {
            if (battleScreen.IsMegaEvolvingOwn)
            {
                DoMegaEvolution(battleScreen, true);
            }
            if (battleScreen.IsMegaEvolvingOpp)
            {
                DoMegaEvolution(battleScreen, false);
            }
        }
        else
        {
            if (battleScreen.IsMegaEvolvingOpp)
            {
                DoMegaEvolution(battleScreen, false);
            }
            if (battleScreen.IsMegaEvolvingOwn)
            {
                DoMegaEvolution(battleScreen, true);
            }
        }
        battleScreen.IsMegaEvolvingOwn = false;
        battleScreen.IsMegaEvolvingOpp = false;
    }

    public void DoSkipTurn(BattleScreen battleScreen)
    {
        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;
        skipTurn = false;
        battleScreen.BattleQuery.AddRange(new QueryObject[] { cq1, cq2 });
        battleScreen.Battle.StartRound(battleScreen);
    }

    public void InitializeRound(BattleScreen battleScreen, BattleRoundConst ownStep)
    {
        if (BattleHasEnded(battleScreen))
        {
            return;
        }
        BattleRoundConst oppStep = GetOppStep(battleScreen, ownStep);
        Battle.OwnStep = ownStep;
        Battle.OppStep = oppStep;
        battleScreen.OwnFaint = false;
        battleScreen.OppFaint = false;
        if (ownStep.StepType == BattleRoundConst.StepTypes.Move)
        {
            ownStep = GetAttack(battleScreen, true, (Attack)ownStep.Argument);
        }
        else
        {
            battleScreen.IsMegaEvolvingOwn = false;
        }
        if (oppStep.StepType == BattleRoundConst.StepTypes.Move)
        {
            oppStep = GetAttack(battleScreen, false, (Attack)oppStep.Argument);
        }
        else
        {
            battleScreen.IsMegaEvolvingOpp = false;
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Move && oppStep.StepType == BattleRoundConst.StepTypes.Move)
        {
            battleScreen.FieldEffects.UsedMoves.Self.Add(((Attack)ownStep.Argument).ID);
            battleScreen.FieldEffects.UsedMoves.Opponent.Add(((Attack)oppStep.Argument).ID);

            Attack ownMove = (Attack)ownStep.Argument;
            Attack oppMove = (Attack)oppStep.Argument;

            if (selectedMoveOwn == true) { ownMove.MoveSelected(true, battleScreen); }
            if (selectedMoveOpp == true) { oppMove.MoveSelected(false, battleScreen); }

            bool first = BattleCalculation.AttackFirst(ownMove, oppMove, battleScreen);
            MegaEvolCheck(battleScreen);

            if (first)
            {
                DoAttackRound(battleScreen, first, ownMove);
                EndRound(battleScreen, 1);
                if (skipTurn == false)
                {
                    DoAttackRound(battleScreen, first == false, oppMove);
                    EndRound(battleScreen, 2);
                }
                else
                {
                    DoSkipTurn(battleScreen);
                }
            }
            else
            {
                DoAttackRound(battleScreen, first, oppMove);
                EndRound(battleScreen, 2);
                if (skipTurn == false)
                {
                    DoAttackRound(battleScreen, first == false, ownMove);
                    EndRound(battleScreen, 1);
                }
                else
                {
                    DoSkipTurn(battleScreen);
                }
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Move && oppStep.StepType == BattleRoundConst.StepTypes.Text)
        {
            MegaEvolCheck(battleScreen);

            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
            EndRound(battleScreen, 2);

            battleScreen.FieldEffects.UsedMoves.Self.Add(((Attack)ownStep.Argument).ID);
            Attack ownMove = (Attack)ownStep.Argument;

            if (selectedMoveOwn == true) { ownMove.MoveSelected(true, battleScreen); }

            DoAttackRound(battleScreen, true, ownMove);
            EndRound(battleScreen, 1);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Move && oppStep.StepType == BattleRoundConst.StepTypes.Item)
        {
            MegaEvolCheck(battleScreen);

            OpponentUseItem(battleScreen, int.Parse(((String)oppStep.Argument).Split(',')[0]), int.Parse(((String)oppStep.Argument).Split(',')[1]));
            EndRound(battleScreen, 2);

            battleScreen.FieldEffects.UsedMoves.Self.Add(((Attack)ownStep.Argument).ID);
            Attack ownMove = (Attack)ownStep.Argument;
            if (selectedMoveOwn == true) { ownMove.MoveSelected(true, battleScreen); }
            DoAttackRound(battleScreen, true, ownMove);
            EndRound(battleScreen, 1);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Move && oppStep.StepType == BattleRoundConst.StepTypes.Switch)
        {
            MegaEvolCheck(battleScreen);

            if (((Attack)ownStep.Argument).ID == 228)
            {
                battleScreen.FieldEffects.Pursuit.Self = true;
                battleScreen.FieldEffects.UsedMoves.Self.Add(((Attack)ownStep.Argument).ID);
                Attack ownMove = (Attack)ownStep.Argument;
                if (selectedMoveOwn == true) { ownMove.MoveSelected(true, battleScreen); }
                DoAttackRound(battleScreen, true, ownMove);
                EndRound(battleScreen, 1);

                SwitchOutOpp(battleScreen, int.Parse((String)oppStep.Argument), String.Empty, true);
                EndRound(battleScreen, 2);
            }
            else
            {
                SwitchOutOpp(battleScreen, int.Parse((String)oppStep.Argument), String.Empty, true);
                EndRound(battleScreen, 2);

                battleScreen.FieldEffects.UsedMoves.Self.Add(((Attack)ownStep.Argument).ID);
                Attack ownMove = (Attack)ownStep.Argument;
                if (selectedMoveOwn == true) { ownMove.MoveSelected(true, battleScreen); }
                DoAttackRound(battleScreen, true, ownMove);
                EndRound(battleScreen, 1);
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Move && oppStep.StepType == BattleRoundConst.StepTypes.Flee)
        {
            MegaEvolCheck(battleScreen);

            battleScreen.FieldEffects.UsedMoves.Self.Add(((Attack)ownStep.Argument).ID);
            Attack ownMove = (Attack)ownStep.Argument;

            if (selectedMoveOwn == true) { ownMove.MoveSelected(true, battleScreen); }

            DoAttackRound(battleScreen, true, ownMove);
            EndRound(battleScreen, 1);

            if (battleScreen.OpponentPokemon.HP > 0)
            {
                if (BattleCalculation.CanSwitch(battleScreen, false) == true)
                {
                    ChangeCameraAngle(0, true, battleScreen);
                    Won = true;
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\running", false));
                    battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                    battleScreen.BattleQuery.Add(new RoamingPokemonFledQueryObject());
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                }
                else
                {
                    ChangeCameraAngle(2, true, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " is trapped!"));
                    EndRound(battleScreen, 2);
                }
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Text && oppStep.StepType == BattleRoundConst.StepTypes.Move)
        {
            MegaEvolCheck(battleScreen);

            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)ownStep.Argument));
            EndRound(battleScreen, 1);

            Attack oppMove = (Attack)oppStep.Argument;
            battleScreen.FieldEffects.UsedMoves.Opponent.Add(oppMove.ID);
            if (selectedMoveOpp == true) { oppMove.MoveSelected(false, battleScreen); }
            DoAttackRound(battleScreen, false, oppMove);
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Text && oppStep.StepType == BattleRoundConst.StepTypes.Text)
        {
            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)ownStep.Argument));
            EndRound(battleScreen, 1);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Text && oppStep.StepType == BattleRoundConst.StepTypes.Item)
        {
            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)ownStep.Argument));
            EndRound(battleScreen, 1);

            ChangeCameraAngle(2, true, battleScreen);
            OpponentUseItem(battleScreen, int.Parse(((String)oppStep.Argument).Split(',')[0]), int.Parse(((String)oppStep.Argument).Split(',')[1]));
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Text && oppStep.StepType == BattleRoundConst.StepTypes.Switch)
        {
            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)ownStep.Argument));
            EndRound(battleScreen, 1);

            ChangeCameraAngle(2, true, battleScreen);
            SwitchOutOpp(battleScreen, int.Parse((String)oppStep.Argument), String.Empty, true);
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Text && oppStep.StepType == BattleRoundConst.StepTypes.Flee)
        {
            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)ownStep.Argument));

            if (battleScreen.OpponentPokemon.HP > 0)
            {
                if (BattleCalculation.CanSwitch(battleScreen, false) == true)
                {
                    ChangeCameraAngle(0, true, battleScreen);
                    Won = true;
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\running", false));
                    battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                    battleScreen.BattleQuery.Add(new RoamingPokemonFledQueryObject());
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                }
                else
                {
                    ChangeCameraAngle(2, true, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " is trapped!"));
                    EndRound(battleScreen, 2);
                }
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Switch && oppStep.StepType == BattleRoundConst.StepTypes.Move)
        {
            MegaEvolCheck(battleScreen);

            if (BattleCalculation.CanSwitch(battleScreen, true) == true)
            {
                if (((Attack)oppStep.Argument).ID == 228)
                {
                    battleScreen.FieldEffects.Pursuit.Opponent = true;
                    Attack oppMove = (Attack)oppStep.Argument;
                    battleScreen.FieldEffects.UsedMoves.Opponent.Add(oppMove.ID);
                    if (selectedMoveOpp == true) { oppMove.MoveSelected(false, battleScreen); }
                    DoAttackRound(battleScreen, false, oppMove);
                    EndRound(battleScreen, 2);

                    SwitchOutOwn(battleScreen, int.Parse((String)ownStep.Argument), -1, String.Empty, true);
                    EndRound(battleScreen, 1);
                }
                else
                {
                    SwitchOutOwn(battleScreen, int.Parse((String)ownStep.Argument), -1, String.Empty, true);
                    EndRound(battleScreen, 1);

                    Attack oppMove = (Attack)oppStep.Argument;
                    battleScreen.FieldEffects.UsedMoves.Opponent.Add(oppMove.ID);
                    if (selectedMoveOpp == true) { oppMove.MoveSelected(false, battleScreen); }
                    DoAttackRound(battleScreen, false, oppMove);
                    EndRound(battleScreen, 2);
                }
            }
            else
            {
                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is trapped!"));

                Attack oppMove = (Attack)oppStep.Argument;
                battleScreen.FieldEffects.UsedMoves.Opponent.Add(oppMove.ID);
                if (selectedMoveOpp == true) { oppMove.MoveSelected(false, battleScreen); }
                DoAttackRound(battleScreen, false, oppMove);
                EndRound(battleScreen, 2);
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Switch && oppStep.StepType == BattleRoundConst.StepTypes.Text)
        {
            if (BattleCalculation.CanSwitch(battleScreen, true) == true)
            {
                SwitchOutOwn(battleScreen, int.Parse((String)ownStep.Argument), -1, String.Empty, true);
                EndRound(battleScreen, 1);

                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                EndRound(battleScreen, 2);
            }
            else
            {
                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is trapped!"));

                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                EndRound(battleScreen, 2);
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Switch && oppStep.StepType == BattleRoundConst.StepTypes.Item)
        {
            if (BattleCalculation.CanSwitch(battleScreen, true) == true)
            {
                SwitchOutOwn(battleScreen, int.Parse((String)ownStep.Argument), -1, String.Empty, true);
                EndRound(battleScreen, 1);

                ChangeCameraAngle(2, true, battleScreen);
                OpponentUseItem(battleScreen, int.Parse(((String)oppStep.Argument).Split(',')[0]), int.Parse(((String)oppStep.Argument).Split(',')[1]));
                EndRound(battleScreen, 2);
            }
            else
            {
                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is trapped!"));

                ChangeCameraAngle(2, true, battleScreen);
                OpponentUseItem(battleScreen, int.Parse(((String)oppStep.Argument).Split(',')[0]), int.Parse(((String)oppStep.Argument).Split(',')[1]));
                EndRound(battleScreen, 2);
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Switch && oppStep.StepType == BattleRoundConst.StepTypes.Switch)
        {
            if (BattleCalculation.CanSwitch(battleScreen, true) == true)
            {
                SwitchOutOwn(battleScreen, int.Parse((String)ownStep.Argument), -1, String.Empty, true);
                EndRound(battleScreen, 1);

                ChangeCameraAngle(2, true, battleScreen);
                SwitchOutOpp(battleScreen, int.Parse((String)oppStep.Argument), String.Empty, true);
                EndRound(battleScreen, 2);
            }
            else
            {
                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is trapped!"));

                ChangeCameraAngle(2, true, battleScreen);
                SwitchOutOpp(battleScreen, int.Parse((String)oppStep.Argument), String.Empty, true);
                EndRound(battleScreen, 2);
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Switch && oppStep.StepType == BattleRoundConst.StepTypes.Flee)
        {
            if (BattleCalculation.CanSwitch(battleScreen, true) == true)
            {
                SwitchOutOwn(battleScreen, int.Parse((String)ownStep.Argument), -1);
                EndRound(battleScreen, 1);

                ChangeCameraAngle(0, true, battleScreen);
                Won = true;
                battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
            }
            else
            {
                ChangeCameraAngle(0, true, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is trapped!"));
            }

            if (battleScreen.OpponentPokemon.HP > 0)
            {
                if (BattleCalculation.CanSwitch(battleScreen, false) == true)
                {
                    ChangeCameraAngle(0, true, battleScreen);
                    Won = true;
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\running", false));
                    battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                    battleScreen.BattleQuery.Add(new RoamingPokemonFledQueryObject());
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                }
                else
                {
                    ChangeCameraAngle(2, true, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " is trapped!"));
                    EndRound(battleScreen, 2);
                }
            }
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Item && oppStep.StepType == BattleRoundConst.StepTypes.Move)
        {
            MegaEvolCheck(battleScreen);

            EndRound(battleScreen, 1);

            Attack oppMove = (Attack)oppStep.Argument;
            battleScreen.FieldEffects.UsedMoves.Opponent.Add(oppMove.ID);
            if (selectedMoveOpp == true) { oppMove.MoveSelected(false, battleScreen); }
            DoAttackRound(battleScreen, false, oppMove);
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Item && oppStep.StepType == BattleRoundConst.StepTypes.Text)
        {
            EndRound(battleScreen, 1);
            ChangeCameraAngle(0, true, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Item && oppStep.StepType == BattleRoundConst.StepTypes.Switch)
        {
            EndRound(battleScreen, 1);

            ChangeCameraAngle(2, true, battleScreen);
            SwitchOutOpp(battleScreen, int.Parse((String)oppStep.Argument));
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Item && oppStep.StepType == BattleRoundConst.StepTypes.Item)
        {
            EndRound(battleScreen, 1);

            ChangeCameraAngle(2, true, battleScreen);
            OpponentUseItem(battleScreen, int.Parse(((String)oppStep.Argument).Split(',')[0]), int.Parse(((String)oppStep.Argument).Split(',')[1]));
            EndRound(battleScreen, 2);
        }

        if (ownStep.StepType == BattleRoundConst.StepTypes.Item && oppStep.StepType == BattleRoundConst.StepTypes.Flee)
        {
            EndRound(battleScreen, 1);

            if (battleScreen.OpponentPokemon.HP > 0)
            {
                if (BattleCalculation.CanSwitch(battleScreen, false) == true)
                {
                    ChangeCameraAngle(0, true, battleScreen);
                    Won = true;
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\running", false));
                    battleScreen.BattleQuery.Add(new TextQueryObject((String)oppStep.Argument));
                    battleScreen.BattleQuery.Add(new RoamingPokemonFledQueryObject());
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                }
                else
                {
                    ChangeCameraAngle(2, true, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " is trapped!"));
                    EndRound(battleScreen, 2);
                }
            }
        }

        EndRound(battleScreen, 0);
    }

    private bool BattleHasEnded(BattleScreen battleScreen)
    {
        foreach (QueryObject b in battleScreen.BattleQuery)
        {
            if (b.QueryType == QueryObject.QueryTypes.EndBattle)
            {
                return true;
            }
        }
        return false;
    }

    private void OpponentUseItem(BattleScreen battleScreen, int itemID, int target)
    {
        Pokemon p = battleScreen.OpponentPokemon;

        if (target != -1)
        {
            p = battleScreen.Trainer.Pokemons[target];
        }

        battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));

        switch (itemID)
        {
            case 18:
                GainHP(20, false, false, battleScreen, battleScreen.Trainer.Name + " used a Potion on " + p.GetDisplayName() + "!", "item:potion");
                break;
            case 17:
                GainHP(50, false, false, battleScreen, battleScreen.Trainer.Name + " used a Super Potion on " + p.GetDisplayName() + "!", "item:superpotion");
                break;
            case 16:
                GainHP(100, false, false, battleScreen, battleScreen.Trainer.Name + " used a Hyper Potion on " + p.GetDisplayName() + "!", "item:hyperpotion");
                break;
            case 15:
                GainHP(p.MaxHP, false, false, battleScreen, battleScreen.Trainer.Name + " used a Max Potion on " + p.GetDisplayName() + "!", "item:maxpotion");
                break;
            case 14:
            {
                String message = String.Empty;
                if (p.HP >= p.MaxHP)
                {
                    message = battleScreen.Trainer.Name + " used a Full Restore on " + p.GetDisplayName() + "!";
                }
                GainHP(p.MaxHP, false, false, battleScreen, battleScreen.Trainer.Name + " used a Full Restore on " + p.GetDisplayName() + "!", "item:fullrestore");
                CureStatusProblem(false, false, battleScreen, message, "item:fullrestore");
                p.RemoveVolatileStatus(Pokemon.VolatileStatus.Confusion);
                break;
            }
            case 38:
                CureStatusProblem(false, false, battleScreen, battleScreen.Trainer.Name + " used a Full Heal on " + p.GetDisplayName() + "!", "item:fullheal");
                p.RemoveVolatileStatus(Pokemon.VolatileStatus.Confusion);
                break;
            case 9:
                CureStatusProblem(false, false, battleScreen, battleScreen.Trainer.Name + " used an Antidote on " + p.GetDisplayName() + "!", "item:antidote");
                break;
            case 10:
                CureStatusProblem(false, false, battleScreen, battleScreen.Trainer.Name + " used a Burn Heal on " + p.GetDisplayName() + "!", "item:burnheal");
                break;
            case 11:
                CureStatusProblem(false, false, battleScreen, battleScreen.Trainer.Name + " used an Ice Heal on " + p.GetDisplayName() + "!", "item:iceheal");
                break;
            case 12:
                CureStatusProblem(false, false, battleScreen, battleScreen.Trainer.Name + " used an Awakening on " + p.GetDisplayName() + "!", "item:awakening");
                break;
            case 13:
                CureStatusProblem(false, false, battleScreen, battleScreen.Trainer.Name + " used a Paralyze Heal on " + p.GetDisplayName() + "!", "item:paralyzeheal");
                break;
            case 39:
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.Trainer.Name + " used a Revive on " + p.GetDisplayName() + "!"));
                p.Status = Pokemon.StatusProblems.None;
                p.HP = (int)Math.Ceiling((double)p.MaxHP / 2);
                break;
            case 40:
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.Trainer.Name + " used a Revive on " + p.GetDisplayName() + "!"));
                p.Status = Pokemon.StatusProblems.None;
                p.HP = p.MaxHP;
                break;
        }

        battleScreen.Trainer.TrainerItemUse(itemID);
    }

    public bool IsChargingTurn(BattleScreen battleScreen, bool own, Attack moveUsed)
    {
        int fly;
        int bounce;
        int dig;
        int dive;
        int skyDrop;
        int geomancy;
        int shadowForce;
        int phantomForce;
        int skullBash;
        int skyAttack;
        int solarBeam;
        int solarBlade;
        int razorWind;
        int bide;

        if (own)
        {
            fly = battleScreen.FieldEffects.FlyCounter.Self;
            bounce = battleScreen.FieldEffects.BounceCounter.Self;
            dig = battleScreen.FieldEffects.DigCounter.Self;
            dive = battleScreen.FieldEffects.DiveCounter.Self;
            skyDrop = battleScreen.FieldEffects.SkyDropCounter.Self;
            geomancy = battleScreen.FieldEffects.GeomancyCounter.Self;
            shadowForce = battleScreen.FieldEffects.ShadowForceCounter.Self;
            phantomForce = battleScreen.FieldEffects.PhantomForceCounter.Self;
            skullBash = battleScreen.FieldEffects.SkullBashCounter.Self;
            skyAttack = battleScreen.FieldEffects.SkyAttackCounter.Self;
            solarBeam = battleScreen.FieldEffects.SolarBeam.Self;
            solarBlade = battleScreen.FieldEffects.SolarBlade.Self;
            razorWind = battleScreen.FieldEffects.RazorWindCounter.Self;
            bide = battleScreen.FieldEffects.BideCounter.Self;
        }
        else
        {
            fly = battleScreen.FieldEffects.FlyCounter.Opponent;
            bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
            dig = battleScreen.FieldEffects.DigCounter.Opponent;
            dive = battleScreen.FieldEffects.DiveCounter.Opponent;
            skyDrop = battleScreen.FieldEffects.SkyDropCounter.Opponent;
            geomancy = battleScreen.FieldEffects.GeomancyCounter.Opponent;
            shadowForce = battleScreen.FieldEffects.ShadowForceCounter.Opponent;
            phantomForce = battleScreen.FieldEffects.PhantomForceCounter.Opponent;
            skullBash = battleScreen.FieldEffects.SkullBashCounter.Opponent;
            skyAttack = battleScreen.FieldEffects.SkyAttackCounter.Opponent;
            solarBeam = battleScreen.FieldEffects.SolarBeam.Opponent;
            solarBlade = battleScreen.FieldEffects.SolarBlade.Opponent;
            razorWind = battleScreen.FieldEffects.RazorWindCounter.Opponent;
            bide = battleScreen.FieldEffects.BideCounter.Opponent;
        }

        if (battleScreen.SelfPokemon.Item != null && battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "power herb" && BattleScreen.CanUseItems)
        {
            return false;
        }

        String moveName = moveUsed.Name.ToLower();
        if (moveName == Localization.GetString("move_name_19", "Fly").ToLower())
        {
            if (fly == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_340", "Bounce").ToLower())
        {
            if (bounce == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_91", "Dig").ToLower())
        {
            if (dig == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_291", "Dive").ToLower())
        {
            if (dive == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_507", "Sky Drop").ToLower())
        {
            if (skyDrop == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_601", "Geomancy").ToLower())
        {
            if (geomancy == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_467", "Shadow Force").ToLower())
        {
            if (shadowForce == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_566", "Phantom Force").ToLower())
        {
            if (phantomForce == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_130", "Skull Bash").ToLower())
        {
            if (skullBash == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_143", "Sky Attack").ToLower())
        {
            if (skyAttack == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_76", "Solar Beam").ToLower())
        {
            if (solarBeam == 0 && battleScreen.FieldEffects.Weather != BattleWeather.WeatherTypes.Sunny) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_669", "Solar Blade").ToLower())
        {
            if (solarBlade == 0 && battleScreen.FieldEffects.Weather != BattleWeather.WeatherTypes.Sunny) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_13", "Razor Wind").ToLower())
        {
            if (razorWind == 0) { return true; }
        }
        else if (moveName == Localization.GetString("move_name_117", "Bide").ToLower())
        {
            if (bide == 0 || bide == 1) { return true; }
        }
        return false;
    }
    public void DoAttackRound(BattleScreen battleScreen, bool own, Attack moveUsed)
    {
        Pokemon p;
        NPC pNPC;
        Pokemon op;
        NPC opNPC;
        if (own)
        {
            p = battleScreen.SelfPokemon;
            op = battleScreen.OpponentPokemon;
            pNPC = battleScreen.SelfPokemonNPC;
            opNPC = battleScreen.OpponentPokemonNPC;
            if ((own && battleScreen.FieldEffects.LastMove.Self != null && battleScreen.FieldEffects.LastMove.Self.ID == 214) == false)
            {
                battleScreen.FieldEffects.LastMove.Self = moveUsed;
            }
        }
        else
        {
            p = battleScreen.OpponentPokemon;
            op = battleScreen.SelfPokemon;
            pNPC = battleScreen.OpponentPokemonNPC;
            opNPC = battleScreen.SelfPokemonNPC;
            if ((own == false && battleScreen.FieldEffects.LastMove.Opponent != null && battleScreen.FieldEffects.LastMove.Opponent.ID == 214) == false)
            {
                battleScreen.FieldEffects.LastMove.Opponent = moveUsed;
            }
        }
        if (wildHasEscaped)
        {
            wildHasEscaped = false;
            return;
        }

        if (battleScreen.FieldEffects.TurnCounts.Self == 0)
        {
            _hasSwitchedInOwn = false;
        }
        if (battleScreen.FieldEffects.TurnCounts.Opponent == 0)
        {
            _hasSwitchedInOpp = false;
        }

        TriggerItemEffect(battleScreen, true);
        TriggerItemEffect(battleScreen, false);

        if (p.Ability.Name.ToLower() == "stance change" && p.Number == 681)
        {
            if (p.AdditionalData == String.Empty)
            {
                if (moveUsed.isDamagingMove)
                {
                    p.AdditionalData = "blade";
                    p.ReloadDefinitions();
                    p.CalculateStats();
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(own, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(p, true), 0, 1, -1, -1));
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into Blade Forme!"));
                }
            }
            else
            {
                if (moveUsed.ID == 588)
                {
                    p.AdditionalData = String.Empty;
                    p.ReloadDefinitions();
                    p.CalculateStats();
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(own, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(p, true), 0, 1, -1, -1));
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into Shield Forme!"));
                }
            }
        }

        if (moveUsed.ID != 194)
        {
            if (own)
            {
                battleScreen.FieldEffects.DestinyBond.Self = false;
            }
            else
            {
                battleScreen.FieldEffects.DestinyBond.Opponent = false;
            }
        }

        if (p.HP <= 0)
        {
            return;
        }

        CameraQueryObject focusQ = (CameraQueryObject)battleScreen.FocusBattle();
        focusQ.ApplyCurrentCamera = true;
        battleScreen.BattleQuery.Add(focusQ);
        battleScreen.BattleQuery.Add(new DelayQueryObject(20));

        if (p.Status == Pokemon.StatusProblems.Freeze)
        {
            if (moveUsed.removesSelfFrozen == true)
            {
                CureStatusProblem(own, own, battleScreen, p.GetDisplayName() + " got defrosted by " + moveUsed.Name + ".", "defrostmove");
            }
        }

        if (p.Status == Pokemon.StatusProblems.Freeze)
        {
            if (Core.Random.Next(0, 100) < 20)
            {
                CureStatusProblem(own, own, battleScreen, p.GetDisplayName() + " thawed out.", "own defrost");
            }
            else
            {
                ChangeCameraAngle(1, own, battleScreen);
                if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                {
                    AnimationQueryObject frozenAnimation = new AnimationQueryObject(pNPC, own == false);
                    frozenAnimation.AnimationPlaySound(@"Battle\Effects\Frozen", 0, 0);
                    for (int currentAmount = 0; currentAmount <= 8; currentAmount++)
                    {
                        Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Frozen", new Rectangle(0, 0, 32, 32), String.Empty);
                        float xPos;
                        float zPos;
                        if (own == false)
                        {
                            xPos = (float)Core.Random.Next(-2, 4) / 8;
                            zPos = (float)Core.Random.Next(-2, 4) / 8;
                        }
                        else
                        {
                            xPos = (float)Core.Random.Next(-4, 2) / 8;
                            zPos = (float)Core.Random.Next(-4, 2) / 8;
                        }
                        Vector3 position = new Vector3(xPos, -0.25f, zPos);
                        Vector3 scale = new Vector3(0.25f);
                        float startDelay = (float)(5.0 * Core.Random.NextDouble());
                        Entity snowflakeEntity = frozenAnimation.SpawnEntity(position, texture, scale, 1.0f, startDelay);
                        frozenAnimation.AnimationFade(snowflakeEntity, true, 0.02f, 0.0f, startDelay, 0.0f);
                    }
                    battleScreen.BattleQuery.Add(frozenAnimation);
                }
                else
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Frozen", false));
                }
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is frozen solid!"));
                return;
            }
        }

        if (p.Status == Pokemon.StatusProblems.Sleep)
        {
            moveUsed.IsSleeping(own, battleScreen);
            int sleepTurns = battleScreen.FieldEffects.SleepTurns.Self;
            if (own == false)
            {
                sleepTurns = battleScreen.FieldEffects.SleepTurns.Opponent;
            }

            if (moveUsed.ID == 214)
            {
                if (sleepTurns > 0)
                {
                    if (own)
                    {
                        battleScreen.FieldEffects.LastMove.Self = moveUsed;
                    }
                    else
                    {
                        battleScreen.FieldEffects.LastMove.Opponent = moveUsed;
                    }
                }
                else
                {
                    CureStatusProblem(own, own, battleScreen, p.GetDisplayName() + " woke up!", "sleepturns");
                    battleScreen.BattleQuery.Add(new TextQueryObject("Sleep Talk failed!"));
                    return;
                }
            }
            else
            {
                if ((own && battleScreen.FieldEffects.LastMove.Self != null && battleScreen.FieldEffects.LastMove.Self.ID == 214) || (own == false && battleScreen.FieldEffects.LastMove.Opponent != null && battleScreen.FieldEffects.LastMove.Opponent.ID == 214))
                {
                    if (own)
                    {
                        battleScreen.FieldEffects.LastMove.Self = moveUsed;
                    }
                    else
                    {
                        battleScreen.FieldEffects.LastMove.Opponent = moveUsed;
                    }
                }
                else
                {
                    if ((sleepTurns > 0 && moveUsed.ID == 173) == false)
                    {
                        if (sleepTurns > 0)
                        {
                            ChangeCameraAngle(1, own, battleScreen);
                            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                            {
                                AnimationQueryObject sleepAnimation = new AnimationQueryObject(pNPC, own == false);
                                sleepAnimation.AnimationPlaySound(@"Battle\Effects\Asleep", 0, 0);
                                Entity sleepEntity1 = sleepAnimation.SpawnEntity(new Vector3(0, 0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.5f), 1, 0, 1);
                                sleepAnimation.AnimationChangeTexture(sleepEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 16, 16, 16), String.Empty), 1f, 1f);
                                sleepAnimation.AnimationMove(sleepEntity1, true, 0, 0.5f, 0.25f, 0.01f, false, false, 0, 0);
                                Entity sleepEntity2 = sleepAnimation.SpawnEntity(new Vector3(0, 0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.5f), 1, 1.5f, 1);
                                sleepAnimation.AnimationChangeTexture(sleepEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 16, 16, 16), String.Empty), 2.5f, 1);
                                sleepAnimation.AnimationMove(sleepEntity2, true, 0, 0.5f, 0.25f, 0.01f, false, false, 2, 0);
                                battleScreen.BattleQuery.Add(sleepAnimation);
                            }
                            else
                            {
                                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Asleep", false));
                            }
                            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is fast asleep."));
                            return;
                        }
                        else
                        {
                            CureStatusProblem(own, own, battleScreen, p.GetDisplayName() + " woke up!", "sleepturns");
                        }
                    }
                }
            }
        }

        if (p.Ability.Name.ToLower() == "truant")
        {
            int truantTurn = battleScreen.FieldEffects.TruantRound.Self;
            if (own == false)
            {
                truantTurn = battleScreen.FieldEffects.TruantRound.Opponent;
            }
            if (truantTurn == 1)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is loafing around!"));
                return;
            }
        }

        if (moveUsed.Disabled > 0)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(moveUsed.Name + " is disabled!"));
            return;
        }

        if (own == true)
        {
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "choice band" || p.Item.OriginalName.ToLower() == "choice specs" || p.Item.OriginalName.ToLower() == "choice scarf")
                {
                    if (battleScreen.FieldEffects.ChoiceMove.Self != null)
                    {
                        if (moveUsed != battleScreen.FieldEffects.ChoiceMove.Self)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s move was prevented due to " + p.Item.OneLineName() + "!"));
                            return;
                        }
                    }
                    else
                    {
                        battleScreen.FieldEffects.ChoiceMove.Self = moveUsed;
                    }
                }
            }
        }
        else
        {
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "choice band" || p.Item.OriginalName.ToLower() == "choice specs" || p.Item.OriginalName.ToLower() == "choice scarf")
                {
                    if (battleScreen.FieldEffects.ChoiceMove.Opponent != null)
                    {
                        if (moveUsed != battleScreen.FieldEffects.ChoiceMove.Opponent)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s move was prevented due to " + p.Item.OneLineName() + "!"));
                            return;
                        }
                    }
                    else
                    {
                        battleScreen.FieldEffects.ChoiceMove.Opponent = moveUsed;
                    }
                }
            }
        }

        int imprisoned = battleScreen.FieldEffects.Imprison.Self;
        if (own == false)
        {
            imprisoned = battleScreen.FieldEffects.Imprison.Opponent;
        }
        if (imprisoned > 0)
        {
            bool hasMove = false;
            foreach (Attack a in op.Attacks)
            {
                if (a.ID == moveUsed.ID)
                {
                    hasMove = true;
                    break;
                }
            }
            if (hasMove == true)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s move is sealed by " + op.GetDisplayName() + "!"));
                return;
            }
        }

        int healBlock = battleScreen.FieldEffects.HealBlock.Opponent;
        if (own == false)
        {
            healBlock = battleScreen.FieldEffects.HealBlock.Self;
        }
        if (healBlock > 0)
        {
            if (moveUsed.isHealingMove == true)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " was prevented from healing!"));
                return;
            }
        }

        if (p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted)
        {
            if (op.Ability.Name.ToLower() == "cacophony" && moveUsed.isSoundMove == true)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " prevented the sound-based move with Cacophony!"));
                    moveUsed.MoveFailsSoundproof(own, battleScreen);
                    return;
                }
            }

            if (op.Ability.Name.ToLower() == "soundproof" && moveUsed.isSoundMove == true)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " prevented the sound-based move with Soundproof!"));
                    moveUsed.MoveFailsSoundproof(own, battleScreen);
                    return;
                }
            }

            if (op.Ability.Name.ToLower() == "sturdy" && moveUsed.isOneHitKOMove == true)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Sturdy prevented any damage from the 1-Hit-KO move."));
                return;
            }
        }

        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true)
        {
            int confusionTurns;
            if (own == true)
            {
                confusionTurns = battleScreen.FieldEffects.ConfusionTurns.Self;
                battleScreen.FieldEffects.ConfusionTurns.Self -= 1;
            }
            else
            {
                confusionTurns = battleScreen.FieldEffects.ConfusionTurns.Opponent;
                battleScreen.FieldEffects.ConfusionTurns.Opponent -= 1;
            }
            if (confusionTurns == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is no longer confused!"));
                p.RemoveVolatileStatus(Pokemon.VolatileStatus.Confusion);
            }
            else
            {
                ChangeCameraAngle(1, own, battleScreen);
                if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                {
                    AnimationQueryObject confusedAnimation = new AnimationQueryObject(pNPC, own);
                    confusedAnimation.AnimationPlaySound(@"Battle\Effects\Confused", 0, 0);
                    Entity duckEntity1 = confusedAnimation.SpawnEntity(new Vector3(-0.25f, 0.25f, -0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.25f), 1, 0, 0);
                    Entity duckEntity2 = confusedAnimation.SpawnEntity(new Vector3(0, 0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.25f), 1, 0, 0);
                    Entity duckEntity3 = confusedAnimation.SpawnEntity(new Vector3(0.25f, 0.25f, 0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.25f), 1, 0, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 0.75f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 0.75f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 0.75f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 1.5f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 1.5f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 1.5f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 2.25f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 2.25f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 2.25f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), 3.0f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), 3.0f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), 3.0f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 3.75f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 3.75f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 3.75f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 4.5f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 4.5f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 4.5f, 0);
                    confusedAnimation.AnimationChangeTexture(duckEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 5.25f, 1);
                    confusedAnimation.AnimationChangeTexture(duckEntity2, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 5.25f, 1);
                    confusedAnimation.AnimationChangeTexture(duckEntity3, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 5.25f, 1);
                    battleScreen.BattleQuery.Add(confusedAnimation);
                }
                else
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Confused", false));
                }
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is confused!"));
                if (Core.Random.Next(0, 3) == 0)
                {
                    Attack confusionAttack = new ConfusionAttack();
                    int confusionDamage = BattleCalculation.CalculateDamage(confusionAttack, false, true, true, battleScreen);
                    ReduceHP(confusionDamage, own, own, battleScreen, p.GetDisplayName() + " hurt itself in confusion.", "confusiondamage");
                    moveUsed.HurtItselfInConfusion(own, battleScreen);
                    if (own == true)
                    {
                        battleScreen.FieldEffects.LastMoveFailed.Self = true;
                    }
                    else
                    {
                        battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
                    }
                    return;
                }
            }
        }

        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Flinch) == true)
        {
            p.RemoveVolatileStatus(Pokemon.VolatileStatus.Flinch);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " flinched and couldn't move!"));
            moveUsed.InflictedFlinch(own, battleScreen);
            if (own == true)
            {
                battleScreen.FieldEffects.LastMoveFailed.Self = true;
            }
            else
            {
                battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
            }
            if (p.Ability.Name.ToLower() == "steadfast")
            {
                RaiseStat(own, own == false, battleScreen, "Speed", 1, String.Empty, "steadfast");
            }
            return;
        }

        int taunt = battleScreen.FieldEffects.Taunt.Self;
        if (own == false)
        {
            taunt = battleScreen.FieldEffects.Taunt.Opponent;
        }
        if (taunt > 0)
        {
            if (moveUsed.Category == Attack.Categories.Status)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s move was prevented due to Taunt!"));
                return;
            }
        }

        int gravity = battleScreen.FieldEffects.Gravity;
        if (gravity > 0)
        {
            if (moveUsed.disabledWhileGravity == true)
            {
                int fly = battleScreen.FieldEffects.FlyCounter.Self;
                if (own == false)
                {
                    fly = battleScreen.FieldEffects.FlyCounter.Opponent;
                }
                if (fly > 0)
                {
                    moveUsed.MoveMisses(own, battleScreen);
                    if (own == true)
                    {
                        battleScreen.FieldEffects.LastMoveFailed.Self = true;
                    }
                    else
                    {
                        battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
                    }
                }
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s move was prevented due to Gravity!"));
                return;
            }
        }

        if (op.HP > 0 && op.Status != Pokemon.StatusProblems.Fainted)
        {
            if (p.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == true)
            {
                if (Core.Random.Next(0, 2) == 0)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject heartAnimation = new AnimationQueryObject(pNPC, own == false);
                        for (int i = 0; i <= 6; i += 2)
                        {
                            Entity heartEntity = heartAnimation.SpawnEntity(new Vector3(0.0f, 0.0f, 0.0f), TextureManager.GetTexture(@"Textures\Battle\Normal\Attract"), new Vector3(0.25f), 1.0f, (float)(i * 0.2));
                            float zPos = (float)(Core.Random.Next(-2, 2) * 0.2);
                            heartAnimation.AnimationMove(heartEntity, false, 0.0f, 0.25f, zPos, 0.01f, false, false, (float)(i * 0.2), 0.0f);
                            heartAnimation.AnimationFade(heartEntity, true, 0.02f, 0.0f, (float)(1 + i * 0.2), 0.0f);
                        }
                        battleScreen.BattleQuery.Add(heartAnimation);
                    }
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is in love with " + op.GetDisplayName() + "!"));
                    moveUsed.IsAttracted(own, battleScreen);
                    if (own == true)
                    {
                        battleScreen.FieldEffects.LastMoveFailed.Self = true;
                    }
                    else
                    {
                        battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
                    }
                    return;
                }
            }
        }

        if (p.Status == Pokemon.StatusProblems.Paralyzed)
        {
            if (Core.Random.Next(0, 4) == 0)
            {
                ChangeCameraAngle(1, own, battleScreen);
                if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                {
                    AnimationQueryObject paralyzedAnimation = new AnimationQueryObject(pNPC, own == false);
                    paralyzedAnimation.AnimationPlaySound(@"Battle\Effects\Paralyzed", 0, 0);
                    for (int currentAmount = 0; currentAmount <= 4; currentAmount++)
                    {
                        Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(0, 0, 16, 16), String.Empty);
                        float xPos = (float)Core.Random.Next(-4, 4) / 8;
                        float zPos = (float)Core.Random.Next(-4, 4) / 8;
                        Vector3 position = new Vector3(xPos, -0.25f, zPos);
                        Vector3 destination = new Vector3(xPos - xPos * 2, 0, zPos - zPos * 2);
                        Vector3 scale = new Vector3(0.25f);
                        float startDelay = (float)(5.0 * Core.Random.NextDouble());
                        Entity shockEntity = paralyzedAnimation.SpawnEntity(position, texture, scale, 1.0f, startDelay);
                        paralyzedAnimation.AnimationMove(shockEntity, true, destination.X, destination.Y, destination.Z, 0.025f, false, true, startDelay, 0.0f);
                        paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(16, 0, 16, 16), String.Empty), startDelay + 1, 1);
                        paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(32, 0, 16, 16), String.Empty), startDelay + 2, 1);
                        paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(48, 0, 16, 16), String.Empty), startDelay + 3, 1);
                        paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(64, 0, 16, 16), String.Empty), startDelay + 4, 1);
                        paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(72, 0, 16, 16), String.Empty), startDelay + 5, 1);
                    }
                    battleScreen.BattleQuery.Add(paralyzedAnimation);
                }
                else
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Paralyzed", false));
                }
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is fully paralyzed!" + Environment.NewLine + "It cannot move!"));
                moveUsed.IsParalyzed(own, battleScreen);
                if (own == true)
                {
                    battleScreen.FieldEffects.LastMoveFailed.Self = true;
                }
                else
                {
                    battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
                }
                return;
            }
        }

        if (op.Status == Pokemon.StatusProblems.Sleep && moveUsed.canHitSleeping == false)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(moveUsed.Name + " failed because " + op.GetDisplayName() + " is asleep!"));
            moveUsed.MoveMisses(own, battleScreen);
            if (own == true)
            {
                battleScreen.FieldEffects.LastMoveFailed.Self = true;
            }
            else
            {
                battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
            }
            return;
        }

        if (own == true)
        {
            battleScreen.FieldEffects.LastMove.Self = moveUsed;
        }
        else
        {
            battleScreen.FieldEffects.LastMove.Opponent = moveUsed;
        }

        if (own == true)
        {
            if (battleScreen.FieldEffects.Torment.Self > 0)
            {
                if (moveUsed == battleScreen.FieldEffects.TormentMove.Self)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(moveUsed.Name + " failed!"));
                    return;
                }
                battleScreen.FieldEffects.TormentMove.Self = battleScreen.FieldEffects.LastMove.Self;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.Torment.Opponent > 0)
            {
                if (moveUsed == battleScreen.FieldEffects.TormentMove.Opponent)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(moveUsed.Name + " failed!"));
                    return;
                }
                battleScreen.FieldEffects.TormentMove.Opponent = battleScreen.FieldEffects.LastMove.Opponent;
            }
        }

        if (own == true)
        {
            int obedienceCheck = BattleCalculation.ObedienceCheck(moveUsed, battleScreen);
            if (obedienceCheck > 0)
            {
                switch (obedienceCheck)
                {
                    case 1:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " ignores orders while asleep!"));
                        return;
                    case 2:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " ignores orders!"));
                        moveUsed = (Attack)GetAttack(battleScreen, own, p.Attacks[Core.Random.Next(0, p.Attacks.Count)]).Argument;
                        return;
                    case 3:
                        InflictSleep(own, own, battleScreen, -1, p.GetDisplayName() + " began to nap!", "obeynap");
                        return;
                    case 4:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " won't obey!"));
                        return;
                    case 5:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " won't obey!"));
                        return;
                    case 6:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " turned away!"));
                        return;
                    case 7:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is loafing around!"));
                        return;
                    case 8:
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " pretended to not notice!"));
                        return;
                }
            }
        }

        moveUsed.PreAttack(own, battleScreen);

        int substitute = battleScreen.FieldEffects.Substitute.Opponent;
        if (own == false)
        {
            substitute = battleScreen.FieldEffects.Substitute.Self;
        }

        int allDamage = 0;
        bool koed = false;
        bool directKoed = false;

        ChangeCameraAngle(1, own, battleScreen);
        String moveUsedText = p.GetDisplayName() + " used " + moveUsed.Name + "!";

        int bide = battleScreen.FieldEffects.BideCounter.Self;
        if (own == false)
        {
            bide = battleScreen.FieldEffects.BideCounter.Opponent;
        }
        if (bide > 0)
        {
            if (bide < 3)
            {
                moveUsedText = p.GetDisplayName() + " is storing energy!";
            }
            else
            {
                moveUsedText = p.GetDisplayName() + " released energy!";
            }
        }

        int thrash = battleScreen.FieldEffects.Thrash.Self;
        if (own == false)
        {
            thrash = battleScreen.FieldEffects.Thrash.Opponent;
        }
        if (thrash > 0)
        {
            moveUsedText = p.GetDisplayName() + " is thrashing about!";
        }

        String randomMoveText = String.Empty;
        if (own == true)
        {
            if (battleScreen.FieldEffects.UsedRandomMove.Self == true && battleScreen.FieldEffects.UsedRandomMoveAttack.Self != null)
            {
                randomMoveText = p.GetDisplayName() + " used " + battleScreen.FieldEffects.UsedRandomMoveAttack.Self.Name + "!";
            }
        }
        else
        {
            if (battleScreen.FieldEffects.UsedRandomMove.Opponent == true && battleScreen.FieldEffects.UsedRandomMoveAttack.Opponent != null)
            {
                randomMoveText = p.GetDisplayName() + " used " + battleScreen.FieldEffects.UsedRandomMoveAttack.Opponent.Name + "!";
            }
        }
        if (randomMoveText != String.Empty)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(randomMoveText));
        }
        battleScreen.BattleQuery.Add(new TextQueryObject(moveUsedText));

        if (moveUsed.DeductPP(own, battleScreen) == true)
        {
            if (moveUsed.CurrentPP > 0)
            {
                moveUsed.CurrentPP -= 1;
                if (op.Ability.Name.ToLower() == "pressure" && moveUsed.CurrentPP > 0)
                {
                    moveUsed.CurrentPP -= 1;
                }
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("But it failed..."));
                moveUsed.MoveMisses(own, battleScreen);
                if (own == true)
                {
                    battleScreen.FieldEffects.LastMoveFailed.Self = true;
                }
                else
                {
                    battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
                }
                return;
            }
        }

        bool noTargetCheck = true;
        if (moveUsed.protectAffected == false)
        {
            noTargetCheck = false;
            String moveNameLower = moveUsed.Name.ToLower();
            if (moveNameLower == Localization.GetString("move_name_367", "Acupressure").ToLower() ||
                moveNameLower == Localization.GetString("move_name_590", "Confide").ToLower() ||
                moveNameLower == Localization.GetString("move_name_364", "Feint").ToLower() ||
                moveNameLower == Localization.GetString("move_name_607", "Hold Hands").ToLower() ||
                moveNameLower == Localization.GetString("move_name_621", "Hyperspace Fury").ToLower() ||
                moveNameLower == Localization.GetString("move_name_593", "Hyperspace Hole").ToLower() ||
                moveNameLower == Localization.GetString("move_name_566", "Phantom Force").ToLower() ||
                moveNameLower == Localization.GetString("move_name_244", "Psych Up").ToLower() ||
                moveNameLower == Localization.GetString("move_name_589", "Play Nice").ToLower() ||
                moveNameLower == Localization.GetString("move_name_46", "Roar").ToLower() ||
                moveNameLower == Localization.GetString("move_name_272", "Role Play").ToLower() ||
                moveNameLower == Localization.GetString("move_name_467", "Shadow Force").ToLower() ||
                moveNameLower == Localization.GetString("move_name_166", "Sketch").ToLower() ||
                moveNameLower == Localization.GetString("move_name_144", "Transform").ToLower() ||
                moveNameLower == Localization.GetString("move_name_18", "Whirlwind").ToLower())
            {
                noTargetCheck = true;
            }
        }
        if (IsChargingTurn(battleScreen, own, moveUsed))
        {
            noTargetCheck = false;
        }
        if (noTargetCheck == true)
        {
            if (op.HP <= 0 || op.Status == Pokemon.StatusProblems.Fainted)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("But there was no target..."));
                return;
            }
        }

        moveUsed.UserPokemonMoveAnimation(battleScreen, own);

        if (moveUsed.target != Attack.Targets.Self && moveUsed.focusOpponentPokemon == true)
        {
            if (own == true)
            {
                QueryObject ca = battleScreen.FocusOpponentPokemon();
                ((CameraQueryObject)ca).SetTargetToStart();
                ScreenFadeQueryObject fa1 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.CloseLeft, Color.Black, true, 110);
                ScreenFadeQueryObject fa2 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.CloseRight, Color.Black, false, 110);
                battleScreen.BattleQuery.AddRange(new QueryObject[] { fa1, ca, fa2 });
            }
            else
            {
                QueryObject ca = battleScreen.FocusSelfPokemon();
                ((CameraQueryObject)ca).SetTargetToStart();
                ScreenFadeQueryObject fa1 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.CloseRight, Color.Black, true, 110);
                ScreenFadeQueryObject fa2 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.CloseLeft, Color.Black, false, 110);
                battleScreen.BattleQuery.AddRange(new QueryObject[] { fa1, ca, fa2 });
            }
        }
        bool doesNotMiss = BattleCalculation.AccuracyCheck(moveUsed, own, battleScreen);

        int lockon = battleScreen.FieldEffects.LockOn.Self;
        if (own == false)
        {
            lockon = battleScreen.FieldEffects.LockOn.Opponent;
        }
        if (lockon > 0)
        {
            doesNotMiss = true;
        }

        int minimize = battleScreen.FieldEffects.Minimize.Opponent;
        if (own == false)
        {
            minimize = battleScreen.FieldEffects.Minimize.Self;
        }

        String[] minimizeMoveList = {
            Localization.GetString("move_name_34", "Body Slam").ToLower(),
            Localization.GetString("move_name_23", "Stomp").ToLower(),
            Localization.GetString("move_name_407", "Dragon Rush").ToLower(),
            Localization.GetString("move_name_467", "Shadow Force").ToLower(),
            Localization.GetString("move_name_537", "Steam Roller").ToLower(),
            Localization.GetString("move_name_535", "Heat Crash").ToLower(),
            Localization.GetString("move_name_566", "Phantom Force").ToLower(),
            Localization.GetString("move_name_9999", "Flying Press").ToLower()
        };

        if (minimize > 0 && minimizeMoveList.Contains(moveUsed.Name.ToLower()))
        {
            doesNotMiss = true;
        }

        bool useTwoTurnCheck = true;
        if (moveUsed.protectAffected == false)
        {
            useTwoTurnCheck = false;
            String moveNameLower2 = moveUsed.Name.ToLower();
            if (moveNameLower2 == Localization.GetString("move_name_367", "Acupressure").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_590", "Confide").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_364", "Feint").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_607", "Hold Hands").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_621", "Hyperspace Fury").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_593", "Hyperspace Hole").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_566", "Phantom Force").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_244", "Psych Up").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_589", "Play Nice").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_46", "Roar").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_272", "Role Play").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_467", "Shadow Force").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_166", "Sketch").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_144", "Transform").ToLower() ||
                moveNameLower2 == Localization.GetString("move_name_18", "Whirlwind").ToLower())
            {
                useTwoTurnCheck = true;
            }
        }

        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int dig = battleScreen.FieldEffects.DigCounter.Opponent;
            if (own == false) { dig = battleScreen.FieldEffects.DigCounter.Self; }
            if (dig > 0 && moveUsed.canHitUnderground == false) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int fly = battleScreen.FieldEffects.FlyCounter.Opponent;
            if (own == false) { fly = battleScreen.FieldEffects.FlyCounter.Self; }
            if (fly > 0 && moveUsed.canHitInMidAir == false) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int bounce = battleScreen.FieldEffects.BounceCounter.Opponent;
            if (own == false) { bounce = battleScreen.FieldEffects.BounceCounter.Self; }
            if (bounce > 0 && moveUsed.canHitInMidAir == false) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int dive = battleScreen.FieldEffects.DiveCounter.Opponent;
            if (own == false) { dive = battleScreen.FieldEffects.DiveCounter.Self; }
            if (dive > 0 && moveUsed.canHitUnderwater == false) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int shadowforce = battleScreen.FieldEffects.ShadowForceCounter.Opponent;
            if (own == false) { shadowforce = battleScreen.FieldEffects.ShadowForceCounter.Self; }
            if (shadowforce > 0) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int phantomforce = battleScreen.FieldEffects.PhantomForceCounter.Opponent;
            if (own == false) { phantomforce = battleScreen.FieldEffects.PhantomForceCounter.Self; }
            if (phantomforce > 0) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int skydrop = battleScreen.FieldEffects.SkyDropCounter.Opponent;
            if (own == false) { skydrop = battleScreen.FieldEffects.SkyDropCounter.Self; }
            if (skydrop > 0 && moveUsed.canHitInMidAir == false) { doesNotMiss = false; }
        }
        if (doesNotMiss == true && useTwoTurnCheck)
        {
            int geomancy = battleScreen.FieldEffects.GeomancyCounter.Opponent;
            if (own == false) { geomancy = battleScreen.FieldEffects.GeomancyCounter.Self; }
            if (geomancy > 0) { doesNotMiss = false; }
        }

        if (IsChargingTurn(battleScreen, own, moveUsed))
        {
            doesNotMiss = true;
        }

        if (doesNotMiss == true)
        {
            float effectiveness = BattleCalculation.CalculateEffectiveness(own, moveUsed, battleScreen);

            int oppHealblock = battleScreen.FieldEffects.HealBlock.Self;
            if (own == false)
            {
                oppHealblock = battleScreen.FieldEffects.HealBlock.Opponent;
            }
            bool moveWorks = true;

            if (moveUsed.MoveFailBeforeAttack(own, battleScreen) == true)
            {
                moveWorks = false;
            }
            if (moveUsed.Name.ToLower() == Localization.GetString("move_name_389", "Sucker Punch").ToLower())
            {
                if (own)
                {
                    if (Battle.OppStep.StepType != BattleRoundConst.StepTypes.Move || ((Attack)Battle.OppStep.Argument).Category == Attack.Categories.Status)
                    {
                        moveWorks = false;
                        battleScreen.BattleQuery.Add(new TextQueryObject("But it failed!"));
                    }
                }
                else
                {
                    if (Battle.OwnStep.StepType != BattleRoundConst.StepTypes.Move || ((Attack)Battle.OwnStep.Argument).Category == Attack.Categories.Status)
                    {
                        moveWorks = false;
                        battleScreen.BattleQuery.Add(new TextQueryObject("But it failed!"));
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "volt absorb" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Electric && moveWorks == true && moveUsed.Category != Attack.Categories.Status)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    if (oppHealblock > 0)
                    {
                        ReduceHP((int)(op.MaxHP / 4), own == false, own, battleScreen, "Heal Block blocked Volt Absorb!", "healblock");
                    }
                    else
                    {
                        if (op.HP == op.MaxHP)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Volt Absorb made " + moveUsed.Name + " useless!"));
                        }
                        else
                        {
                            GainHP((int)(op.MaxHP / 4), own == false, own == false, battleScreen, op.GetDisplayName() + "'s Volt Absorb absorbed the attack!", "volatabsorb");
                        }
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "motor drive" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Electric && moveWorks == true)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    ChangeCameraAngle(2, own, battleScreen);
                    if (op.StatSpeed == 6)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Motor Drive made " + moveUsed.Name + " useless!"));
                    }
                    else
                    {
                        RaiseStat(own == false, own == false, battleScreen, "Speed", 1, op.GetDisplayName() + "'s Motor Drive absorbed the attack!", "motordrive");
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "water absorb" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Water && moveWorks == true && moveUsed.Category != Attack.Categories.Status)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    if (oppHealblock > 0)
                    {
                        ReduceHP((int)(op.MaxHP / 4), own == false, own, battleScreen, "Heal Block blocked Water Absorb!", "healblock");
                    }
                    else
                    {
                        if (op.HP == op.MaxHP)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Water Absorb made " + moveUsed.Name + " useless!"));
                        }
                        else
                        {
                            GainHP((int)(op.MaxHP / 4), own == false, own == false, battleScreen, op.GetDisplayName() + "'s Water Absorb absorbed the attack!", "waterabsorb");
                        }
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "dry skin" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Water && moveWorks == true && moveUsed.Category != Attack.Categories.Status)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    if (oppHealblock > 0)
                    {
                        ReduceHP((int)(op.MaxHP / 4), own == false, own, battleScreen, "Heal Block blocked Dry Skin!", "healblock");
                    }
                    else
                    {
                        if (op.HP == op.MaxHP)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Dry Skin made " + moveUsed.Name + " useless!"));
                        }
                        else
                        {
                            GainHP((int)(op.MaxHP / 4), own == false, own == false, battleScreen, op.GetDisplayName() + "'s Dry Skin absorbed the attack!", "dryskin");
                        }
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "sap sipper" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Grass && moveWorks == true && moveUsed.Category != Attack.Categories.Status)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    ChangeCameraAngle(2, own, battleScreen);
                    if (op.StatAttack == 6)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Sap Sipper made " + moveUsed.Name + " useless!"));
                    }
                    else
                    {
                        RaiseStat(own == false, own == false, battleScreen, "Attack", 1, op.GetDisplayName() + "'s Sap Sipper absorbed the attack!", "sapsipper");
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "flash fire" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Fire && moveWorks == true)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    ChangeCameraAngle(2, own, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Flash Fire made " + moveUsed.Name + " useless!"));
                    if (own == true)
                    {
                        battleScreen.FieldEffects.FlashFire.Opponent = 1;
                    }
                    else
                    {
                        battleScreen.FieldEffects.FlashFire.Self = 1;
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "storm drain" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Water && moveWorks == true && moveUsed.Category != Attack.Categories.Status)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    ChangeCameraAngle(2, own, battleScreen);
                    if (op.StatSpAttack == 6)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Storm Drain made " + moveUsed.Name + " useless!"));
                    }
                    else
                    {
                        RaiseStat(own == false, own == false, battleScreen, "Special Attack", 1, op.GetDisplayName() + "'s Storm Drain absorbed the attack!", "stormdrain");
                    }
                }
            }
            if (op.Ability.Name.ToLower() == "lightningrod" && moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Electric && moveWorks == true && moveUsed.Category != Attack.Categories.Status)
            {
                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                {
                    moveWorks = false;
                    ChangeCameraAngle(2, own, battleScreen);
                    if (op.StatSpAttack == 6)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Lightningrod made " + moveUsed.Name + " useless!"));
                    }
                    else
                    {
                        RaiseStat(own == false, own == false, battleScreen, "Special Attack", 1, op.GetDisplayName() + "'s Lightningrod absorbed the attack!", "lightningrod");
                    }
                }
            }

            if (op.Ability.Name.ToLower() == "overcoat" && moveUsed.isPowderMove == true)
            {
                moveWorks = false;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " is not affected by " + moveUsed.Name + "!"));
            }

            if (op.Type1.Type == Element.Types.Grass || op.Type2.Type == Element.Types.Grass)
            {
                if (moveUsed.isPowderMove == true)
                {
                    moveWorks = false;
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " is not affected by " + moveUsed.Name + "!"));
                }
            }

            if (op.Type1.Type == Element.Types.Ghost || op.Type2.Type == Element.Types.Ghost)
            {
                if (moveUsed.isTrappingMove == true)
                {
                    moveWorks = false;
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " is not affected by " + moveUsed.Name + "!"));
                }
            }

            if (op.Ability.Name.ToLower() == "bulletproof" && moveUsed.isBulletMove == true)
            {
                moveWorks = false;
                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " prevents damage with its Bulletproof ability!"));
            }

            if (battleScreen.FieldEffects.PsychicTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own == false, battleScreen) == true)
            {
                if (moveUsed.priority > 0)
                {
                    moveWorks = false;
                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " surrounds itself with Psychic Terrain!"));
                }
            }

            if (op.Ability.Name.ToLower() == "armor tail")
            {
                String[] exceptionAbilities = { "mold breaker", "turboblaze", "teravolt" };
                String[] exceptionAttacks = {
                    Localization.GetString("move_name_195", "Perish Song").ToLower(),
                    Localization.GetString("move_name_274", "Assist").ToLower(),
                    Localization.GetString("move_name_383", "Copycat").ToLower(),
                    Localization.GetString("move_name_382", "Me First").ToLower(),
                    Localization.GetString("move_name_267", "Nature Power").ToLower(),
                    Localization.GetString("move_name_214", "Sleep Talk").ToLower(),
                    Localization.GetString("move_name_289", "Snatch").ToLower()
                };
                if (moveUsed.priority > 0 && exceptionAttacks.Contains(moveUsed.Name.ToLower()) == false &&
                    moveUsed.target != Attack.Targets.All && moveUsed.target != Attack.Targets.AllFoes &&
                    exceptionAbilities.Contains(p.Ability.Name.ToLower()) == false &&
                    (battleScreen.FieldEffects.UsedRandomMove.Self == false || battleScreen.FieldEffects.UsedRandomMoveAttack.Self.priority > 0) &&
                    (battleScreen.FieldEffects.UsedMirrorMove.Self == false || battleScreen.FieldEffects.UsedMirrorMoveAttack.Self.priority > 0))
                {
                    moveWorks = false;
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " cannot use " + moveUsed.Name + " because of Armor Tail!"));
                }
            }

            if (moveWorks == true)
            {
                if (op.HP > 0 && op.Status != Pokemon.StatusProblems.Fainted)
                {
                    int protect = battleScreen.FieldEffects.ProtectCounter.Opponent;
                    if (own == false) { protect = battleScreen.FieldEffects.ProtectCounter.Self; }
                    if (protect > 0 && moveUsed.protectAffected == true)
                    {
                        bool protectWorks = true;
                        if (p.Ability.Name.ToLower() == "no guard")
                        {
                            if (Core.Random.Next(0, 100) < (100 - moveUsed.GetAccuracy(own, battleScreen)))
                            {
                                protectWorks = false;
                            }
                        }
                        if (protectWorks == true)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " protected itself!"));
                            moveUsed.MoveProtectedDetected(own, battleScreen);
                            return;
                        }
                    }

                    int detect = battleScreen.FieldEffects.DetectCounter.Opponent;
                    if (own == false) { detect = battleScreen.FieldEffects.DetectCounter.Self; }
                    if (detect > 0 && moveUsed.protectAffected == true)
                    {
                        bool detectWorks = true;
                        if (p.Ability.Name.ToLower() == "no guard")
                        {
                            if (Core.Random.Next(0, 100) < (100 - moveUsed.GetAccuracy(own, battleScreen)))
                            {
                                detectWorks = false;
                            }
                        }
                        if (detectWorks == true)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " protected itself!"));
                            moveUsed.MoveProtectedDetected(own, battleScreen);
                            return;
                        }
                    }

                    int kingsshield = battleScreen.FieldEffects.KingsShieldCounter.Opponent;
                    if (own == false) { kingsshield = battleScreen.FieldEffects.KingsShieldCounter.Self; }
                    if (kingsshield > 0 && moveUsed.protectAffected == true && moveUsed.Category != Attack.Categories.Status)
                    {
                        bool kingsshieldWorks = true;
                        if (p.Ability.Name.ToLower() == "no guard")
                        {
                            if (Core.Random.Next(0, 100) < (100 - moveUsed.GetAccuracy(own, battleScreen)))
                            {
                                kingsshieldWorks = false;
                            }
                        }
                        if (kingsshieldWorks == true)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " protected itself!"));
                            if (moveUsed.makesContact == true)
                            {
                                LowerStat(own, own == false, battleScreen, "Attack", 1, String.Empty, "move:kingsshield");
                            }
                            moveUsed.MoveProtectedDetected(own, battleScreen);
                            return;
                        }
                    }
                }

                if (p.Ability.Name.ToLower() == "protean" && moveUsed.ID != 165)
                {
                    if (p.Type1.Type != moveUsed.Type.Type || p.Type2.Type != Element.Types.Blank)
                    {
                        if (p.OriginalType1 == null)
                        {
                            p.OriginalType1 = GameModeElementLoader.GetElementByID(p.Type1.Type);
                        }
                        p.Type1.Type = moveUsed.Type.Type;
                        if (p.Type2.Type != Element.Types.Blank)
                        {
                            if (p.OriginalType2 == null)
                            {
                                p.OriginalType2 = GameModeElementLoader.GetElementByID(p.Type2.Type);
                            }
                            p.Type2.Type = Element.Types.Blank;
                        }
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s type changed to " + p.Type1.ToString() + " due to Protean."));
                    }
                }

                moveUsed.OpponentPokemonMoveAnimation(battleScreen, own);

                if (own == true && battleScreen.FieldEffects.FlyCounter.Self == 2)
                {
                    battleScreen.FieldEffects.FlyCounter.Self = 0;
                }
                if (own == false && battleScreen.FieldEffects.FlyCounter.Opponent == 2)
                {
                    battleScreen.FieldEffects.FlyCounter.Opponent = 0;
                }
                if (moveUsed.isDamagingMove == true)
                {
                    ChangeCameraAngle(2, own, battleScreen);
                    if (op.Ability.Name.ToLower() == "wonder guard" && effectiveness <= 1.0f && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true && moveUsed.isWonderGuardAffected == true)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "s Wonder Guard blocked the attack!"));
                        return;
                    }

                    int hits = 0;
                    int timesToAttack = moveUsed.GetTimesToAttack(own, battleScreen);

                    bool useParentalBond = false;
                    if (timesToAttack == 1 && p.Ability != null && p.Ability.Name.ToLower() == "parental bond")
                    {
                        String[] pbMoveList = {
                            Localization.GetString("move_name_283", "Endeavor").ToLower(),
                            Localization.GetString("move_name_374", "Fling").ToLower(),
                            Localization.GetString("move_name_153", "Explosion").ToLower(),
                            Localization.GetString("move_name_120", "Self-Destruct").ToLower(),
                            Localization.GetString("move_name_515", "Final Gambit").ToLower(),
                            Localization.GetString("move_name_340", "Bounce").ToLower(),
                            Localization.GetString("move_name_91", "Dig").ToLower(),
                            Localization.GetString("move_name_291", "Dive").ToLower(),
                            Localization.GetString("move_name_19", "Fly").ToLower(),
                            Localization.GetString("move_name_553", "Freeze Shock").ToLower(),
                            Localization.GetString("move_name_601", "Geomancy").ToLower(),
                            Localization.GetString("move_name_554", "Ice Burn").ToLower(),
                            Localization.GetString("move_name_566", "Phantom Force").ToLower(),
                            Localization.GetString("move_name_13", "Razor Wind").ToLower(),
                            Localization.GetString("move_name_467", "Shadow Force").ToLower(),
                            Localization.GetString("move_name_130", "Skull Bash").ToLower(),
                            Localization.GetString("move_name_143", "Sky Attack").ToLower(),
                            Localization.GetString("move_name_507", "Sky Drop").ToLower(),
                            Localization.GetString("move_name_76", "Solar Beam").ToLower(),
                            Localization.GetString("move_name_669", "Solar Blade").ToLower(),
                            Localization.GetString("move_name_117", "Bide").ToLower(),
                            Localization.GetString("move_name_165", "Struggle").ToLower()
                        };
                        if (pbMoveList.Contains(moveUsed.Name.ToLower()) == false)
                        {
                            useParentalBond = true;
                            timesToAttack += 1;
                        }
                    }

                    for (int i = 1; i <= timesToAttack; i++)
                    {
                        bool critical = BattleCalculation.IsCriticalHit(moveUsed, own, battleScreen);
                        int damage = 0;
                        if (useParentalBond && i == timesToAttack)
                        {
                            damage = moveUsed.GetDamage(critical, own, own == false, battleScreen, "parental bond");
                        }
                        else
                        {
                            damage = moveUsed.GetDamage(critical, own, own == false, battleScreen);
                        }

                        if (effectiveness != 0)
                        {
                            if (damage == 0 && moveUsed.ID == 117)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject("But it failed..."));
                                moveUsed.MoveMisses(own, battleScreen);
                                if (own == true)
                                {
                                    battleScreen.FieldEffects.LastMoveFailed.Self = true;
                                }
                                else
                                {
                                    battleScreen.FieldEffects.LastMoveFailed.Opponent = true;
                                }
                                effectiveness = 0;
                                break;
                            }

                            bool sturdyWorked = false;

                            if (substitute == 0)
                            {
                                if (op.HP == op.MaxHP && op.MaxHP <= damage)
                                {
                                    directKoed = true;
                                }
                                else
                                {
                                    directKoed = false;
                                }

                                if (directKoed == true && op.Ability.Name.ToLower() == "sturdy" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                                {
                                    directKoed = false;
                                    sturdyWorked = true;
                                    damage = op.MaxHP - 1;
                                }

                                if (op.HP <= damage)
                                {
                                    koed = true;
                                }
                                else
                                {
                                    koed = false;
                                }
                            }

                            allDamage += damage;

                            if (substitute == 0)
                            {
                                if (own == true)
                                {
                                    int didDamage = damage;
                                    if (didDamage > op.HP) { didDamage = op.HP; }
                                    battleScreen.FieldEffects.LastDamage.Self = didDamage;
                                    if (battleScreen.FieldEffects.BideCounter.Opponent > 0) { battleScreen.FieldEffects.BideDamage.Opponent += didDamage; }
                                    if (battleScreen.FieldEffects.RageFistPower.Opponent < 350) { battleScreen.FieldEffects.RageFistPower.Opponent += 50; }
                                }
                                else
                                {
                                    int didDamage = damage;
                                    if (didDamage > op.HP) { didDamage = op.HP; }
                                    battleScreen.FieldEffects.LastDamage.Opponent = didDamage;
                                    if (battleScreen.FieldEffects.BideCounter.Self > 0) { battleScreen.FieldEffects.BideDamage.Self += didDamage; }
                                    if (battleScreen.FieldEffects.RageFistPower.Self < 350) { battleScreen.FieldEffects.RageFistPower.Self += 50; }
                                }
                            }

                            moveUsed.BeforeDealingDamage(own, battleScreen);

                            if (substitute == 0)
                            {
                                int endure = battleScreen.FieldEffects.Endure.Opponent;
                                if (own == false) { endure = battleScreen.FieldEffects.Endure.Self; }

                                bool endureWorked = false;
                                if (endure > 0 && effectiveness != 0)
                                {
                                    if (damage > op.HP)
                                    {
                                        damage = op.HP - 1;
                                        endureWorked = true;
                                    }
                                }

                                if (own == true)
                                {
                                    battleScreen.FieldEffects.PokemonDamagedThisTurn.Opponent = true;
                                }
                                else
                                {
                                    battleScreen.FieldEffects.PokemonDamagedThisTurn.Self = true;
                                }

                                String sound = @"Battle\Damage\Effective";
                                if (effectiveness > 1.0f) { sound = @"Battle\Damage\SuperEffective"; }
                                if (effectiveness < 1.0f && effectiveness != 0.0f) { sound = @"Battle\Damage\NotVeryEffective"; }

                                ReduceHP(damage, own == false, own, battleScreen, String.Empty, "battledamage", sound);

                                if (sturdyWorked == true)
                                {
                                    battleScreen.BattleQuery.Add(new TextQueryObject("Sturdy prevented " + op.GetDisplayName() + " from fainting!"));
                                }
                                if (endureWorked == true)
                                {
                                    battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " endured the attack."));
                                }
                            }
                            else
                            {
                                if (own == true)
                                {
                                    battleScreen.FieldEffects.Substitute.Opponent -= damage;
                                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s substitute took the damage!"));
                                    if (battleScreen.FieldEffects.Substitute.Opponent <= 0)
                                    {
                                        battleScreen.FieldEffects.Substitute.Opponent = 0;
                                        battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(false, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(battleScreen.OpponentPokemon, true), 0, 1, -1, -1));
                                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " substitute broke!"));
                                        break;
                                    }
                                }
                                else
                                {
                                    battleScreen.FieldEffects.Substitute.Self -= damage;
                                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s substitute took the damage!"));
                                    if (battleScreen.FieldEffects.Substitute.Self <= 0)
                                    {
                                        battleScreen.FieldEffects.Substitute.Self = 0;
                                        battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(battleScreen.SelfPokemon, true), 0, 1, -1, -1));
                                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " substitute broke!"));
                                        break;
                                    }
                                }
                            }
                        }

                        if (effectiveness > 1.0f)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("It's super effective!"));
                            if (battleScreen.IsRemoteBattle == true && battleScreen.IsHost == true)
                            {
                                if (own == true) { battleScreen.SelfStatistics.SuperEffective += 1; }
                                else { battleScreen.OpponentStatistics.SuperEffective += 1; }
                            }
                        }
                        else if (effectiveness < 1.0f && effectiveness != 0.0f)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("It's not very effective..."));
                            if (battleScreen.IsRemoteBattle == true && battleScreen.IsHost == true)
                            {
                                if (own == true) { battleScreen.SelfStatistics.NotVeryEffective += 1; }
                                else { battleScreen.OpponentStatistics.NotVeryEffective += 1; }
                            }
                        }
                        else if (effectiveness == 0.0f)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("It has no effect..."));
                            if (battleScreen.IsRemoteBattle == true && battleScreen.IsHost == true)
                            {
                                if (own == true) { battleScreen.SelfStatistics.NoEffect += 1; }
                                else { battleScreen.OpponentStatistics.NoEffect += 1; }
                            }
                            break;
                        }

                        if (critical == true && effectiveness != 0)
                        {
                            if (battleScreen.IsRemoteBattle == true && battleScreen.IsHost == true)
                            {
                                if (own == true) { battleScreen.SelfStatistics.Critical += 1; }
                                else { battleScreen.OpponentStatistics.Critical += 1; }
                            }
                            battleScreen.BattleQuery.Add(new TextQueryObject("It's a critical hit!"));
                            if (op.Ability.Name.ToLower() == "anger point" && op.StatAttack < 6 && op.HP > 0)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Anger Point maxed it's attack!"));
                                op.StatAttack = 6;
                            }
                        }

                        if (effectiveness != 0)
                        {
                            bool canUseEffect = true;
                            bool multiUseEffect = true;

                            if (op.Ability.Name.ToLower() == "shield dust" && moveUsed.hasSecondaryEffect == true)
                            {
                                if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                                {
                                    canUseEffect = false;
                                }
                            }
                            if (p.Ability.Name.ToLower() == "sheer force" && moveUsed.hasSecondaryEffect == true)
                            {
                                canUseEffect = false;
                            }

                            if (useParentalBond)
                            {
                                String[] pbEffectMoveList = {
                                    Localization.GetString("move_name_290", "Secret Power").ToLower(),
                                    Localization.GetString("move_name_450", "Bug Bite").ToLower(),
                                    Localization.GetString("move_name_365", "Pluck").ToLower(),
                                    Localization.GetString("move_name_265", "Smelling Salts").ToLower(),
                                    Localization.GetString("move_name_358", "Wake-Up Slap").ToLower(),
                                    Localization.GetString("move_name_282", "Knock Off").ToLower(),
                                    Localization.GetString("move_name_547", "Relic Song").ToLower(),
                                    Localization.GetString("move_name_509", "Circle Throw").ToLower(),
                                    Localization.GetString("move_name_525", "Dragon Tail").ToLower()
                                };
                                if (pbEffectMoveList.Contains(moveUsed.Name.ToLower()))
                                {
                                    multiUseEffect = false;
                                }
                            }

                            if ((canUseEffect && multiUseEffect) || (multiUseEffect == false && i == timesToAttack))
                            {
                                if (substitute == 0 || moveUsed.isAffectedBySubstitute == false)
                                {
                                    moveUsed.MoveHits(own, battleScreen);
                                    if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = false; }
                                    else { battleScreen.FieldEffects.LastMoveFailed.Opponent = false; }

                                    if (op.Status == Pokemon.StatusProblems.Freeze)
                                    {
                                        if (moveUsed.removesOpponentFrozen == true)
                                        {
                                            CureStatusProblem(own == false, own, battleScreen, op.GetDisplayName() + " got defrosted by " + moveUsed.Name + ".", "defrostmove");
                                        }
                                    }
                                }
                            }
                            if (op.HP > 0 && op.Status != Pokemon.StatusProblems.Fainted)
                            {
                                if (p.Item != null)
                                {
                                    if (p.Item.OriginalName.ToLower() == "king's rock" || p.Item.OriginalName.ToLower() == "razor fang" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                                    {
                                        if (Core.Random.Next(0, 100) < 10)
                                        {
                                            InflictFlinch(own == false, own, battleScreen, String.Empty, "item:king's rock");
                                        }
                                    }
                                }
                            }

                            moveUsed.MoveRecoil(own, battleScreen);

                            if (op.HP > 0)
                            {
                                if (own == true)
                                {
                                    if (battleScreen.FieldEffects.RageCounter.Opponent > 0)
                                    {
                                        battleScreen.FieldEffects.RageCounter.Opponent += 1;
                                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " built up its rage."));
                                    }
                                }
                                else
                                {
                                    if (battleScreen.FieldEffects.RageCounter.Self > 0)
                                    {
                                        battleScreen.FieldEffects.RageCounter.Self += 1;
                                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " built up its rage."));
                                    }
                                }
                            }
                        }
                        else
                        {
                            moveUsed.MoveHasNoEffect(own, battleScreen);
                            if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = true; }
                            else { battleScreen.FieldEffects.LastMoveFailed.Opponent = true; }
                        }

                        if (battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen))
                        {
                            switch (op.Ability.Name.ToLower())
                            {
                                case "color change":
                                    if (op.HP > 0)
                                    {
                                        if (op.Type1.Type != moveUsed.GetAttackType(own, battleScreen).Type || op.Type2.Type != Element.Types.Blank)
                                        {
                                            ChangeCameraAngle(2, own, battleScreen);
                                            op.OriginalType1 = op.Type1;
                                            op.OriginalType2 = op.Type2;
                                            op.Type1 = moveUsed.GetAttackType(own, battleScreen);
                                            op.Type2.Type = Element.Types.Blank;
                                            battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " changed its color!"));
                                        }
                                    }
                                    break;
                                case "rough skin":
                                    if (moveUsed.makesContact == true)
                                    {
                                        ReduceHP((int)Math.Floor((double)p.MaxHP / 16), own, own == false, battleScreen, p.GetDisplayName() + " was harmed by Rough Skin.", "roughskin");
                                    }
                                    break;
                                case "static":
                                    if (moveUsed.makesContact == true && p.Status == Pokemon.StatusProblems.None)
                                    {
                                        if (Core.Random.Next(0, 100) < 30)
                                        {
                                            InflictParalysis(own, own == false, battleScreen, op.GetDisplayName() + "'s Static affects " + p.GetDisplayName() + "!", "static");
                                        }
                                    }
                                    break;
                                case "effect spore":
                                    if (moveUsed.makesContact == true && p.Status == Pokemon.StatusProblems.None && p.Ability.Name.ToLower() != "overcoat")
                                    {
                                        int r = Core.Random.Next(0, 100);
                                        if (r < 30)
                                        {
                                            if (r < 9)
                                            {
                                                InflictPoison(own, own == false, battleScreen, false, op.GetDisplayName() + "'s Effect Spore affects " + p.GetDisplayName() + "!", "effectspore");
                                            }
                                            else if (r >= 9 && r < 19)
                                            {
                                                InflictParalysis(own, own == false, battleScreen, op.GetDisplayName() + "'s Effect Spore affects " + p.GetDisplayName() + "!", "effectspore");
                                            }
                                            else
                                            {
                                                InflictSleep(own, own == false, battleScreen, -1, op.GetDisplayName() + "'s Effect Spore affects " + p.GetDisplayName() + "!", "effectspore");
                                                i = timesToAttack;
                                            }
                                        }
                                    }
                                    break;
                                case "poison point":
                                    if (moveUsed.makesContact == true && p.Status == Pokemon.StatusProblems.None)
                                    {
                                        if (Core.Random.Next(0, 100) < 30)
                                        {
                                            InflictPoison(own, own == false, battleScreen, false, op.GetDisplayName() + "'s Poison Point affects " + p.GetDisplayName() + "!", "poisonpoint");
                                        }
                                    }
                                    break;
                                case "flame body":
                                    if (moveUsed.makesContact == true && p.Status == Pokemon.StatusProblems.None)
                                    {
                                        if (Core.Random.Next(0, 100) < 30)
                                        {
                                            InflictBurn(own, own == false, battleScreen, op.GetDisplayName() + "'s Flame Body affects " + p.GetDisplayName() + "!", "flamebody");
                                        }
                                    }
                                    break;
                                case "cute charm":
                                    if (moveUsed.makesContact == true && p.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == false)
                                    {
                                        if (Core.Random.Next(0, 100) < 30)
                                        {
                                            if (p.Gender != Pokemon.Genders.Genderless && op.Gender != Pokemon.Genders.Genderless && op.Gender != p.Gender)
                                            {
                                                InflictInfatuate(own, own == false, battleScreen, op.GetDisplayName() + "'s Cute Charm affects " + p.GetDisplayName() + "!", "cutecharm");
                                            }
                                        }
                                    }
                                    break;
                                case "aftermath":
                                    if (moveUsed.makesContact == true)
                                    {
                                        if (op.HP <= 0)
                                        {
                                            ReduceHP((int)(p.MaxHP / 4), own, own == false, battleScreen, "Aftermath caused damage!", "aftermath");
                                        }
                                    }
                                    break;
                                case "iron barbs":
                                    if (moveUsed.makesContact == true)
                                    {
                                        ReduceHP((int)(p.MaxHP / 8), own, own == false, battleScreen, "Iron Barbs caused damage!", "ironbarbs");
                                    }
                                    break;
                                case "cursed body":
                                    if (moveUsed.Disabled == 0)
                                    {
                                        if (substitute == 0)
                                        {
                                            if (Core.Random.Next(0, 100) < 30)
                                            {
                                                ChangeCameraAngle(2, own, battleScreen);
                                                battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Cursed Body disabled " + moveUsed.Name + "!"));
                                                moveUsed.Disabled = 4;
                                            }
                                        }
                                    }
                                    break;
                                case "mummy":
                                    if (moveUsed.makesContact == true)
                                    {
                                        if (p.Ability.Name.ToLower() != "multitype" && p.Ability.Name.ToLower() != "mummy")
                                        {
                                            p.Ability = Ability.GetAbilityByID(152);
                                            ChangeCameraAngle(1, own, battleScreen);
                                            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s ability changed to Mummy!"));
                                        }
                                    }
                                    break;
                                case "justified":
                                    if (moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Dark)
                                    {
                                        RaiseStat(own == false, own == false, battleScreen, "Attack", 1, op.GetDisplayName() + " became justified!", "justified");
                                    }
                                    break;
                                case "steam engine":
                                    if (moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Fire || moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Water)
                                    {
                                        RaiseStat(own == false, own == false, battleScreen, "Speed", 2, String.Empty, "steam engine");
                                    }
                                    break;
                                case "rattled":
                                    if (moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Dark || moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Bug || moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Ghost)
                                    {
                                        RaiseStat(own == false, own == false, battleScreen, "Speed", 1, op.GetDisplayName() + "'s Rattled affected it's clairaudience.", "rattled");
                                    }
                                    break;
                                case "gooey":
                                    if (moveUsed.makesContact == true)
                                    {
                                        LowerStat(own, own == false, battleScreen, "Speed", 1, "Gooey slowed down " + p.GetDisplayName() + "!", "gooey");
                                    }
                                    break;
                                case "tangling hair":
                                    if (moveUsed.makesContact == true)
                                    {
                                        LowerStat(own, own == false, battleScreen, "Speed", 1, "Tangling Hair slowed down " + p.GetDisplayName() + "!", "tangling hair");
                                    }
                                    break;
                                case "weak armor":
                                    if (moveUsed.Category == Attack.Categories.Physical)
                                    {
                                        RaiseStat(own == false, own == false, battleScreen, "Speed", 2, "Weak Armor causes the Speed to increase!", "weakarmor");
                                        LowerStat(own == false, own == false, battleScreen, "Defense", 1, "Weak Armor causes the Defense to decrease!", "weakarmor");
                                    }
                                    break;
                                case "pickpocket":
                                    if (moveUsed.makesContact == true)
                                    {
                                        if (p.Item != null && op.Item == null && substitute == 0)
                                        {
                                            bool canSteal = true;
                                            if (p.Item.IsMegaStone == true) { canSteal = false; }
                                            if (p.Item.IsMail == true) { canSteal = false; }
                                            if (p.Ability.Name.ToLower() == "multitype" && p.Item.OriginalName.ToLower().EndsWith(" plate")) { canSteal = false; }
                                            if (p.Item.OriginalName.ToLower() == "griseous orb" && p.Number == 487) { canSteal = false; }
                                            if (p.Item.OriginalName.ToLower().EndsWith(" drive") == true && p.Number == 649) { canSteal = false; }
                                            if (canSteal)
                                            {
                                                Item stolenItem = p.Item;
                                                if (p.OriginalItem == null) { p.OriginalItem = stolenItem; }
                                                if (battleScreen.Battle.RemoveHeldItem(own, own == false, battleScreen, op.GetDisplayName() + " stole an item from " + p.GetDisplayName() + " due to " + op.Ability.Name + "!", op.Ability.Name.ToLower()))
                                                {
                                                    op.Item = stolenItem;
                                                    if (op.Item != null && op.OriginalItem != null)
                                                    {
                                                        String opItemID = op.Item.IsGameModeItem == true ? op.Item.gmID : op.Item.ID.ToString();
                                                        String opOriginalItemID = op.OriginalItem.IsGameModeItem == true ? op.OriginalItem.gmID : op.OriginalItem.ID.ToString();
                                                        if (opItemID == opOriginalItemID && op.Item.AdditionalData == op.OriginalItem.AdditionalData)
                                                        {
                                                            op.OriginalItem = null;
                                                            if (own == true)
                                                            {
                                                                if (battleScreen.FieldEffects.StolenFromSelfItems.ContainsKey(battleScreen.SelfPokemonIndex))
                                                                {
                                                                    battleScreen.FieldEffects.StolenFromSelfItems.Remove(battleScreen.SelfPokemonIndex);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex))
                                                                {
                                                                    battleScreen.FieldEffects.StolenFromOpponentItems.Remove(battleScreen.OpponentPokemonIndex);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (p.OriginalItem.ID == p.Item.ID && p.OriginalItem.AdditionalData == p.Item.AdditionalData)
                                                    {
                                                        p.OriginalItem = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen))
                        {
                            switch (p.Ability.Name.ToLower())
                            {
                                case "poison touch":
                                    if (moveUsed.makesContact == true && op.Status == Pokemon.StatusProblems.None)
                                    {
                                        if (Core.Random.Next(0, 100) < 30)
                                        {
                                            InflictPoison(own == false, own, battleScreen, false, p.GetDisplayName() + "'s Poison Touch affects " + op.GetDisplayName() + "!", "poisontouch");
                                        }
                                    }
                                    break;
                                case "moxie":
                                    if (koed == true)
                                    {
                                        RaiseStat(own, own, battleScreen, "Attack", 1, p.GetDisplayName() + "'s Moxie got in effect!", "moxie");
                                    }
                                    break;
                                case "magician":
                                    if (op.Item != null && p.Item == null && substitute == 0)
                                    {
                                        bool canSteal = true;
                                        if (op.Item.IsMegaStone == true) { canSteal = false; }
                                        if (op.Item.IsMail == true) { canSteal = false; }
                                        if (op.Ability.Name.ToLower() == "multitype" && op.Item.OriginalName.ToLower().EndsWith(" plate")) { canSteal = false; }
                                        if (op.Item.OriginalName.ToLower() == "griseous orb" && op.Number == 487) { canSteal = false; }
                                        if (op.Item.OriginalName.ToLower().EndsWith(" drive") == true && op.Number == 649) { canSteal = false; }
                                        if (canSteal)
                                        {
                                            Item stolenItem = op.Item;
                                            if (op.OriginalItem == null) { op.OriginalItem = stolenItem; }
                                            if (battleScreen.Battle.RemoveHeldItem(own == false, own, battleScreen, p.GetDisplayName() + " stole an item from " + op.GetDisplayName() + " due to " + p.Ability.Name + "!", p.Ability.Name.ToLower()))
                                            {
                                                p.Item = stolenItem;
                                                if (p.Item != null && p.OriginalItem != null)
                                                {
                                                    String pItemID = p.Item.IsGameModeItem == true ? p.Item.gmID : p.Item.ID.ToString();
                                                    String pOriginalItemID = p.OriginalItem.IsGameModeItem == true ? p.OriginalItem.gmID : p.OriginalItem.ID.ToString();
                                                    if (pItemID == pOriginalItemID && p.Item.AdditionalData == p.OriginalItem.AdditionalData)
                                                    {
                                                        p.OriginalItem = null;
                                                        if (own == true)
                                                        {
                                                            if (battleScreen.FieldEffects.StolenFromSelfItems.ContainsKey(battleScreen.SelfPokemonIndex))
                                                            {
                                                                battleScreen.FieldEffects.StolenFromSelfItems.Remove(battleScreen.SelfPokemonIndex);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (battleScreen.FieldEffects.StolenFromOpponentItems.ContainsKey(battleScreen.OpponentPokemonIndex))
                                                            {
                                                                battleScreen.FieldEffects.StolenFromOpponentItems.Remove(battleScreen.OpponentPokemonIndex);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (op.OriginalItem.ID == op.Item.ID && op.OriginalItem.AdditionalData == op.Item.AdditionalData)
                                                {
                                                    op.OriginalItem = null;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        if (substitute == 0 && op.HP > 0)
                        {
                            if (op.Item != null)
                            {
                                if (battleScreen.FieldEffects.CanUseItem(own == false) && battleScreen.FieldEffects.CanUseOwnItem(own == false, battleScreen) == true)
                                {
                                    switch (op.Item.OriginalName.ToLower())
                                    {
                                        case "enigma":
                                            if (RemoveHeldItem(own == false, own == false, battleScreen, op.GetDisplayName() + " used the Enigma Berry to recover.", "berry:enigma") == true)
                                            {
                                                GainHP((int)Math.Ceiling((double)op.MaxHP / 4), own == false, own == false, battleScreen, String.Empty, "berry:enigma");
                                            }
                                            break;
                                        case "jaboca":
                                            if (moveUsed.Category == Attack.Categories.Physical)
                                            {
                                                if (allDamage > 0)
                                                {
                                                    if (RemoveHeldItem(own == false, own == false, battleScreen, String.Empty, "berry:jaboca") == true)
                                                    {
                                                        InflictRecoil(own, own == false, battleScreen, null, (int)Math.Ceiling((double)allDamage / 2), "The Jaboca Berry damaged " + p.GetDisplayName() + "!", "berry:jaboca");
                                                    }
                                                }
                                            }
                                            break;
                                        case "rowap":
                                            if (moveUsed.Category == Attack.Categories.Special)
                                            {
                                                if (allDamage > 0)
                                                {
                                                    if (RemoveHeldItem(own == false, own == false, battleScreen, String.Empty, "berry:rowap") == true)
                                                    {
                                                        InflictRecoil(own, own == false, battleScreen, null, (int)Math.Ceiling((double)allDamage / 2), "The Rowap Berry damaged " + p.GetDisplayName() + "!", "berry:rowap");
                                                    }
                                                }
                                            }
                                            break;
                                        case "kee":
                                            if (moveUsed.Category == Attack.Categories.Physical)
                                            {
                                                if (allDamage > 0)
                                                {
                                                    if (RemoveHeldItem(own == false, own == false, battleScreen, String.Empty, "berry:kee") == true)
                                                    {
                                                        battleScreen.Battle.RaiseStat(own == false, own == false, battleScreen, "Defense", 1, String.Empty, "berry:kee");
                                                    }
                                                }
                                            }
                                            break;
                                        case "maranga":
                                            if (moveUsed.Category == Attack.Categories.Special)
                                            {
                                                if (allDamage > 0)
                                                {
                                                    if (RemoveHeldItem(own == false, own == false, battleScreen, String.Empty, "berry:maranga") == true)
                                                    {
                                                        battleScreen.Battle.RaiseStat(own == false, own == false, battleScreen, "Special Defense", 1, String.Empty, "berry:maranga");
                                                    }
                                                }
                                            }
                                            break;
                                        case "snowball":
                                            if (moveUsed.Type.Type == Element.Types.Ice)
                                            {
                                                if (allDamage > 0 && op.StatAttack < 6)
                                                {
                                                    if (RemoveHeldItem(own == false, own == false, battleScreen, "-1", "item:snowball") == true)
                                                    {
                                                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Snowball was consumed!"));
                                                        battleScreen.Battle.RaiseStat(own == false, own == false, battleScreen, "Attack", 1, String.Empty, "item:snowball");
                                                    }
                                                }
                                            }
                                            break;
                                        case "cell battery":
                                            if (moveUsed.Type.Type == Element.Types.Electric)
                                            {
                                                if (allDamage > 0 && op.StatAttack < 6)
                                                {
                                                    if (RemoveHeldItem(own == false, own == false, battleScreen, "-1", "item:cell battery") == true)
                                                    {
                                                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s Cell Battery was consumed!"));
                                                        battleScreen.Battle.RaiseStat(own == false, own == false, battleScreen, "Attack", 1, String.Empty, "item:cell battery");
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                        hits += 1;

                        if (op.HP <= 0)
                        {
                            FaintPokemon(own == false, battleScreen, String.Empty);

                            bool destinyBond = false;
                            if (own == true)
                            {
                                destinyBond = battleScreen.FieldEffects.DestinyBond.Opponent;
                            }
                            else
                            {
                                destinyBond = battleScreen.FieldEffects.DestinyBond.Self;
                            }

                            if (destinyBond == true)
                            {
                                ReduceHP(p.HP, own, own == false, battleScreen, op.GetDisplayName() + " took its attacker with it!", "move:destinybond");
                                FaintPokemon(own, battleScreen, String.Empty);
                            }

                            break;
                        }

                        if (op.HP > 0 && effectiveness != 0)
                        {
                            if (moveUsed.GetAttackType(own, battleScreen).Type == Element.Types.Fire)
                            {
                                if (op.Status == Pokemon.StatusProblems.Freeze)
                                {
                                    CureStatusProblem(own == false, own, battleScreen, op.GetDisplayName() + " got defrosted by " + moveUsed.Name + ".", "defrostedfire");
                                }
                            }
                        }
                    }

                    if (p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted && effectiveness != 0.0f)
                    {
                        moveUsed.MoveMultiTurn(own, battleScreen);
                        moveUsed.MoveRecharge(own, battleScreen);
                        moveUsed.MoveSwitch(own, battleScreen);
                    }

                    if ((hits > 1 || timesToAttack > 1) && effectiveness != 0.0f)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject("Hit " + hits + " times!"));
                    }

                    if (p.Item != null)
                    {
                        if (p.Item.OriginalName.ToLower() == "sticky barb" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                        {
                            if (p.Ability.Name.ToLower() != "magic guard")
                            {
                                ReduceHP((int)Math.Floor((double)p.MaxHP / 8), true, true, battleScreen, p.GetDisplayName() + " was harmed by Sticky Barb.", "stickybarb");
                            }
                            if (Core.Random.Next(0, 2) == 0 && moveUsed.makesContact == true && op.Item == null && op.HP > 0)
                            {
                                ChangeCameraAngle(2, own, battleScreen);
                                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s Sticky Barb was passed over to " + op.GetDisplayName() + "."));
                                if (p.Item.IsGameModeItem == true)
                                {
                                    op.Item = Item.GetItemByID(p.Item.gmID.ToString());
                                }
                                else
                                {
                                    op.Item = Item.GetItemByID(p.Item.ID.ToString());
                                }
                                p.Item = null;
                            }
                        }
                    }

                    if (p.HP > 0)
                    {
                        if (p.Item != null && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                        {
                            switch (p.Item.OriginalName.ToLower())
                            {
                                case "shell bell":
                                    if (p.HP < p.MaxHP)
                                    {
                                        GainHP((int)(allDamage / 8), own, own, battleScreen, p.GetDisplayName() + " gains some HP due to the Shell Bell.", "shellbell");
                                    }
                                    break;
                                case "life orb":
                                    if (p.Ability.Name.ToLower() != "magic guard" && p.Ability.Name.ToLower() != "sheer force")
                                    {
                                        ReduceHP((int)(p.MaxHP / 10), own, own, battleScreen, p.GetDisplayName() + " loses HP due to Life Orb.", "lifeorb");
                                    }
                                    break;
                            }
                        }
                    }
                    if (battleScreen.FieldEffects.TempTripleKick > 0)
                    {
                        battleScreen.FieldEffects.TempTripleKick = 0;
                    }
                }
                else
                {
                    Attack lastMove = battleScreen.FieldEffects.LastMove.Opponent;
                    if (own == false)
                    {
                        lastMove = battleScreen.FieldEffects.LastMove.Self;
                    }
                    if (moveUsed.snatchAffected == true && lastMove != null && lastMove.ID == 289)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " snatched " + p.GetDisplayName() + "'s move!"));
                        moveUsed.MoveHits(own == false, battleScreen);
                        if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = false; }
                        else { battleScreen.FieldEffects.LastMoveFailed.Opponent = false; }
                    }
                    else
                    {
                        String magicReflect = String.Empty;
                        if (moveUsed.magicCoatAffected == true)
                        {
                            if (op.Ability.Name.ToLower() == "magic bounce" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen) == true)
                            {
                                magicReflect = "Magic Bounce";
                            }
                            else
                            {
                                if ((own == true && battleScreen.FieldEffects.MagicCoat.Opponent > 0) || (own == false && battleScreen.FieldEffects.MagicCoat.Self > 0))
                                {
                                    magicReflect = "Magic Coat";
                                }
                            }
                        }

                        if (magicReflect != String.Empty)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(magicReflect + " bounced the attack back!"));
                            effectiveness = BattleCalculation.CalculateEffectiveness(own == false, moveUsed, battleScreen);

                            int oppSubstitute = battleScreen.FieldEffects.Substitute.Self;
                            if (own == false) { oppSubstitute = battleScreen.FieldEffects.Substitute.Opponent; }

                            if (moveUsed.magicCoatAffected)
                            {
                                if (p.IsType(Element.Types.Dark))
                                {
                                    if (op.Ability.Name.ToLower() == "prankster" && battleScreen.FieldEffects.CanUseAbility(own == false, battleScreen))
                                    {
                                        effectiveness = 0.0f;
                                    }
                                }
                            }

                            if (effectiveness == 0.0f)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject("It has no effect..."));
                                moveUsed.MoveHasNoEffect(own == false, battleScreen);
                            }
                            else
                            {
                                if (oppSubstitute == 0 || moveUsed.isAffectedBySubstitute == false)
                                {
                                    moveUsed.MoveHits(own == false, battleScreen);
                                    if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = false; }
                                    else { battleScreen.FieldEffects.LastMoveFailed.Opponent = false; }
                                }
                                else
                                {
                                    battleScreen.BattleQuery.Add(new TextQueryObject("The substitute absorbed the move!"));
                                }
                            }
                        }
                        else
                        {
                            if (moveUsed.magicCoatAffected)
                            {
                                if (op.IsType(Element.Types.Dark))
                                {
                                    if (p.Ability.Name.ToLower() == "prankster" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen))
                                    {
                                        effectiveness = 0.0f;
                                    }
                                }
                            }
                            if (effectiveness == 0.0f)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject("It has no effect..."));
                                moveUsed.MoveHasNoEffect(own, battleScreen);
                                if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = true; }
                                else { battleScreen.FieldEffects.LastMoveFailed.Opponent = true; }
                            }
                            else
                            {
                                if (substitute == 0 || moveUsed.isAffectedBySubstitute == false)
                                {
                                    moveUsed.MoveHits(own, battleScreen);
                                    if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = false; }
                                    else { battleScreen.FieldEffects.LastMoveFailed.Opponent = false; }
                                }
                                else
                                {
                                    battleScreen.BattleQuery.Add(new TextQueryObject("The substitute absorbed the move!"));
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (moveUsed.Category == Attack.Categories.Status)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("But it failed..."));
            }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("But it missed..."));
            }
            moveUsed.MoveMisses(own, battleScreen);
            if (own == true) { battleScreen.FieldEffects.LastMoveFailed.Self = true; }
            else { battleScreen.FieldEffects.LastMoveFailed.Opponent = true; }
        }

        int encoreAttackIndex = -1;
        if (own == true && battleScreen.FieldEffects.Encore.Self > 0)
        {
            for (int a = 0; a <= battleScreen.SelfPokemon.Attacks.Count - 1; a++)
            {
                if (battleScreen.SelfPokemon.Attacks[a].ID == battleScreen.FieldEffects.EncoreMove.Self.ID)
                {
                    encoreAttackIndex = a;
                }
            }
            if (encoreAttackIndex != -1 && battleScreen.SelfPokemon.Attacks[encoreAttackIndex].CurrentPP == 0)
            {
                battleScreen.FieldEffects.EncoreMove.Self = null;
                battleScreen.FieldEffects.Encore.Self = 0;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s encore stopped."));
            }
        }
    }
    public void FaintPokemon(bool own, BattleScreen battleScreen, String message)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        p.HP = 0;
        p.Status = Pokemon.StatusProblems.Fainted;
        ChangeCameraAngle(1, own, battleScreen);
        if (message == String.Empty)
        {
            message = p.GetDisplayName() + " fainted!";
        }
        battleScreen.BattleQuery.Add(new TextQueryObject(message));
        if (battleScreen.IsTrainerBattle == false && Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            if (own == false)
            {
                float modelOffset = 0.0f;
                if (battleScreen.OpponentPokemonNPC.Model != null)
                {
                    modelOffset = 0.5f;
                }
                String crySuffixOpp = PokemonForms.GetCrySuffix(battleScreen.OpponentPokemon);
                AnimationQueryObject faintAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, true);
                faintAnimation.AnimationPlaySound(battleScreen.OpponentPokemon.Number.ToString(), 0, 2, false, true, crySuffixOpp);
                faintAnimation.AnimationMove(null, false, 0, -1 - modelOffset, 0, 0.05f, false, false, 2, 2);
                faintAnimation.AnimationFade(null, false, 1.0f, 0.0f, 4, 0);
                battleScreen.BattleQuery.Add(faintAnimation);
            }
        }
        String additionalDataLower = p.AdditionalData.ToLower();
        switch (additionalDataLower)
        {
            case "mega":
            case "mega_x":
            case "mega_y":
            case "primal":
            case "blade":
                p.AdditionalData = PokemonForms.GetInitialAdditionalData(p);
                p.ReloadDefinitions();
                p.CalculateStats();
                if (additionalDataLower != "blade")
                {
                    p.RestoreAbility();
                }
                break;
        }
        if (p.Ability != null)
        {
            p.Ability.EndBattle(p);
        }
        if (battleScreen.IsTrainerBattle == true)
        {
            if (own == true)
            {
                battleScreen.TrainerFaintedOwn += 1;
                if (battleScreen.Trainer.FaintedOwnMessage.ContainsKey(battleScreen.TrainerFaintedOwn))
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.FaintedOwnMessage[battleScreen.TrainerFaintedOwn]).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                    ChangeCameraAngle(1, own, battleScreen);
                }
            }
            else
            {
                battleScreen.TrainerFaintedOpp += 1;
                if (battleScreen.Trainer.FaintedOppMessage.ContainsKey(battleScreen.TrainerFaintedOpp))
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.FaintedOppMessage[battleScreen.TrainerFaintedOpp]).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                    ChangeCameraAngle(1, own, battleScreen);
                }
            }
        }
    }

    public bool CureStatusProblem(bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        if (message != String.Empty)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(message));
        }
        if (own == true && battleScreen.FieldEffects.PoisonCounter.Self > 0)
        {
            battleScreen.FieldEffects.PoisonCounter.Self = 0;
        }
        if (own == false && battleScreen.FieldEffects.PoisonCounter.Opponent > 0)
        {
            battleScreen.FieldEffects.PoisonCounter.Opponent = 0;
        }
        p.Status = Pokemon.StatusProblems.None;
        return true;
    }

    public bool InflictFlinch(bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (own == false)
        {
            if (battleScreen.FieldEffects.TurnCounts.Opponent > battleScreen.FieldEffects.TurnCounts.Self)
            {
                return false;
            }
        }
        else
        {
            if (battleScreen.FieldEffects.TurnCounts.Self > battleScreen.FieldEffects.TurnCounts.Opponent)
            {
                return false;
            }
        }
        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Flinch) == true)
        {
            return false;
        }
        if (p.Ability.Name.ToLower() == "inner focus" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " won't flinch because of its Inner Focus!"));
            return false;
        }
        else
        {
            int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
            if (substitute > 0)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("The substitute prevented flinching."));
                return false;
            }
            else
            {
                p.AddVolatileStatus(Pokemon.VolatileStatus.Flinch);
                ChangeCameraAngle(1, own, battleScreen);
                if (message == "-1") { }
                else if (message == String.Empty) { }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(message));
                }
                return true;
            }
        }
    }

    public bool InflictBurn(bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (p.Status == Pokemon.StatusProblems.Burn)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already burned!"));
            return false;
        }
        if (p.Status != Pokemon.StatusProblems.None)
        {
            return false;
        }
        if (battleScreen.FieldEffects.MistyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the burn."));
            return false;
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute took the burn."));
            return false;
        }
        else
        {
            if (p.Type1.Type == Element.Types.Fire || p.Type2.Type == Element.Types.Fire)
            {
                return false;
            }
            if (p.Ability.Name.ToLower() == "water veil" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("Water Veil prevented the burn."));
                return false;
            }
            if (p.Ability.Name.ToLower() == "leaf guard" && battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("Leaf Guard prevented the burn."));
                return false;
            }
            int safeGuard = own ? battleScreen.FieldEffects.Safeguard.Self : battleScreen.FieldEffects.Safeguard.Opponent;
            if (safeGuard > 0 && op.Ability.Name.ToLower() != "infiltrator")
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("Safeguard prevented the burn."));
                return false;
            }
            p.Status = Pokemon.StatusProblems.Burn;
            ChangeCameraAngle(1, own, battleScreen);
            AnimationQueryObject burnAnimation = new AnimationQueryObject(pNPC, own);
            burnAnimation.AnimationPlaySound(@"Battle\Effects\Burned", 0, 0);
            Entity flameEntity = burnAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f, 0.5f, 0.5f), 1.0f);
            burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 32, 32, 32), String.Empty), 0.75f, 0);
            burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 64, 32, 32), String.Empty), 1.5f, 0);
            burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 96, 32, 32), String.Empty), 2.25f, 0);
            burnAnimation.AnimationChangeTexture(flameEntity, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 128, 32, 32), String.Empty), 3, 0);
            battleScreen.BattleQuery.Add(burnAnimation);
            if (message == String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " got burned!"));
            }
            else if (message == "-1") { }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " got burned!"));
            }
            if (p.Ability.Name.ToLower() == "synchronize" && from != own)
            {
                InflictBurn(own == false, own == false, battleScreen, "Synchronize passed over the burn.", "synchronize");
            }
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "rawst" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                {
                    if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:rawst") == true)
                    {
                        battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                        CureStatusProblem(own, own, battleScreen, "The Rawst Berry cured the burn of " + p.GetDisplayName() + "!", "berry:rawst");
                    }
                }
            }
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "lum" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                {
                    if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lum") == true)
                    {
                        battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                        CureStatusProblem(own, own, battleScreen, "The Lum Berry cured the burn of " + p.GetDisplayName() + "!", "berry:lum");
                    }
                }
            }
            return true;
        }
    }

    public bool InflictFreeze(bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (p.Status == Pokemon.StatusProblems.Freeze)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already frozen!"));
            return false;
        }
        if (p.Status != Pokemon.StatusProblems.None)
        {
            return false;
        }
        if (battleScreen.FieldEffects.MistyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the freeze."));
            return false;
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute took the freeze effect."));
            return false;
        }
        else
        {
            if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("The sunny weather prevented " + p.GetDisplayName() + " from freezing."));
                return false;
            }
            if (p.Type1.Type == Element.Types.Ice || p.Type2.Type == Element.Types.Ice)
            {
                if (cause != "move:triattack" && cause != "move:secretpower")
                {
                    return false;
                }
            }
            if (p.Ability.Name.ToLower() == "magma armor" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
            {
                if (cause != "move:triattack" && cause != "move:secretpower")
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject("Magma Armor prevented the freeze."));
                    return false;
                }
            }
            if (p.Ability.Name.ToLower() == "leaf guard" && battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("Leaf Guard prevented the freeze."));
                return false;
            }
            int safeGuard = own ? battleScreen.FieldEffects.Safeguard.Self : battleScreen.FieldEffects.Safeguard.Opponent;
            if (safeGuard > 0 && op.Ability.Name.ToLower() != "infiltrator")
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("Safeguard prevented the freeze."));
                return false;
            }
            p.Status = Pokemon.StatusProblems.Freeze;
            ChangeCameraAngle(1, own, battleScreen);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                AnimationQueryObject frozenAnimation = new AnimationQueryObject(pNPC, own == false);
                frozenAnimation.AnimationPlaySound(@"Battle\Effects\Frozen", 0, 0);
                for (int currentAmount = 0; currentAmount <= 8; currentAmount++)
                {
                    Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Frozen", new Rectangle(0, 0, 32, 32), String.Empty);
                    float xPos;
                    float zPos;
                    if (own == false)
                    {
                        xPos = (float)Core.Random.Next(-2, 4) / 8;
                        zPos = (float)Core.Random.Next(-2, 4) / 8;
                    }
                    else
                    {
                        xPos = (float)Core.Random.Next(-4, 2) / 8;
                        zPos = (float)Core.Random.Next(-4, 2) / 8;
                    }
                    Vector3 position = new Vector3(xPos, -0.25f, zPos);
                    float startDelay = (float)(5.0 * Core.Random.NextDouble());
                    Entity snowflakeEntity = frozenAnimation.SpawnEntity(position, texture, new Vector3(0.25f), 1.0f, startDelay);
                    frozenAnimation.AnimationFade(snowflakeEntity, true, 0.02f, 0.0f, startDelay, 0.0f);
                }
                battleScreen.BattleQuery.Add(frozenAnimation);
            }
            else
            {
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Frozen", false));
            }
            if (message == String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " was frozen solid!"));
            }
            else if (message == "-1") { }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " was frozen solid!"));
            }
            if (p.Ability.Name.ToLower() == "synchronize" && from != own)
            {
                InflictFreeze(own == false, own == false, battleScreen, "Synchronize passed over the freeze.", "synchronize");
            }
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "aspear" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                {
                    if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:aspear") == true)
                    {
                        battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                        CureStatusProblem(own, own, battleScreen, "The Aspear Berry thraw out " + p.GetDisplayName() + "!", "berry:aspear");
                    }
                }
            }
            if (p.Item != null)
            {
                if (p.Item.OriginalName.ToLower() == "lum" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                {
                    if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lum") == true)
                    {
                        battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                        CureStatusProblem(own, own, battleScreen, "The Lum Berry thraw out " + p.GetDisplayName() + "!", "berry:lum");
                    }
                }
            }
            return true;
        }
    }

    public bool InflictParalysis(bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (p.Status == Pokemon.StatusProblems.Paralyzed)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already paralyzed!"));
            return false;
        }
        if (p.Type1.Type == Element.Types.Electric || p.Type2.Type == Element.Types.Electric)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is not affected by paralysis!"));
            return false;
        }
        if (p.Status != Pokemon.StatusProblems.None)
        {
            return false;
        }
        if (battleScreen.FieldEffects.MistyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the paralysis."));
            return false;
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute took the paralysis."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "limber" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Limber prevented the paralysis."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "leaf guard" && battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Leaf Guard prevented the paralysis."));
            return false;
        }
        int safeGuard = own ? battleScreen.FieldEffects.Safeguard.Self : battleScreen.FieldEffects.Safeguard.Opponent;
        if (safeGuard > 0 && op.Ability.Name.ToLower() != "infiltrator")
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Safeguard prevented the paralysis."));
            return false;
        }
        p.Status = Pokemon.StatusProblems.Paralyzed;
        ChangeCameraAngle(1, own, battleScreen);
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            AnimationQueryObject paralyzedAnimation = new AnimationQueryObject(pNPC, own == false);
            paralyzedAnimation.AnimationPlaySound(@"Battle\Effects\Paralyzed", 0, 0);
            for (int currentAmount = 0; currentAmount <= 4; currentAmount++)
            {
                Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(0, 0, 16, 16), String.Empty);
                float xPos = (float)Core.Random.Next(-4, 4) / 8;
                float zPos = (float)Core.Random.Next(-4, 4) / 8;
                Vector3 position = new Vector3(xPos, -0.25f, zPos);
                Vector3 destination = new Vector3(xPos - xPos * 2, 0, zPos - zPos * 2);
                float startDelay = (float)(5.0 * Core.Random.NextDouble());
                Entity shockEntity = paralyzedAnimation.SpawnEntity(position, texture, new Vector3(0.25f), 1.0f, startDelay);
                paralyzedAnimation.AnimationMove(shockEntity, false, destination.X, destination.Y, destination.Z, 0.025f, false, true, startDelay, 0.0f);
                paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(16, 0, 16, 16), String.Empty), startDelay + 1, 0);
                paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(32, 0, 16, 16), String.Empty), startDelay + 2, 0);
                paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(48, 0, 16, 16), String.Empty), startDelay + 3, 0);
                paralyzedAnimation.AnimationChangeTexture(shockEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(64, 0, 16, 16), String.Empty), startDelay + 4, 0);
                paralyzedAnimation.AnimationChangeTexture(shockEntity, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Paralyzed", new Rectangle(72, 0, 16, 16), String.Empty), startDelay + 5, 0);
            }
            battleScreen.BattleQuery.Add(paralyzedAnimation);
        }
        else
        {
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Paralyzed", false));
        }
        if (message == String.Empty)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is paralyzed!" + Environment.NewLine + "It can't move!"));
        }
        else if (message == "-1") { }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(message));
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is paralyzed!" + Environment.NewLine + "It can't move!"));
        }
        if (p.Ability.Name.ToLower() == "synchronize" && from != own)
        {
            InflictParalysis(own == false, own == false, battleScreen, "Synchronize passed over the paralysis.", "synchronize");
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "cheri" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:cheri") == true)
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    CureStatusProblem(own, own, battleScreen, "The Cheri Berry cured the paralysis of " + p.GetDisplayName() + "!", "berry:cheri");
                }
            }
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "lum" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lum") == true)
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    CureStatusProblem(own, own, battleScreen, "The Lum Berry cured the paralyzis of " + p.GetDisplayName() + "!", "berry:lum");
                }
            }
        }
        return true;
    }

    public bool InflictSleep(bool own, bool from, BattleScreen battleScreen, int turnsPreset, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (battleScreen.FieldEffects.ElectricTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The electricity prevented the sleep."));
            return false;
        }
        if (battleScreen.FieldEffects.MistyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the sleep."));
            return false;
        }
        int sleepTurns = turnsPreset;
        if (sleepTurns < 0)
        {
            sleepTurns = Core.Random.Next(1, 4);
        }
        if (p.Ability.Name.ToLower() == "early bird")
        {
            sleepTurns = (int)Math.Floor((double)sleepTurns / 2);
        }
        if (p.Status == Pokemon.StatusProblems.Sleep)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already asleep!"));
            return false;
        }
        if (p.Status != Pokemon.StatusProblems.None)
        {
            if (cause != "move:rest")
            {
                return false;
            }
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute took the sleep effect."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "vital spirit" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Vital Spirit prevented the sleep."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "insomnia" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Insomnia prevented the sleep."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "sweet veil" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Sweet Veil prevented the sleep."));
            return false;
        }
        int uproar = own ? battleScreen.FieldEffects.Uproar.Self : battleScreen.FieldEffects.Uproar.Opponent;
        if (uproar > 0)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The Uproar prevented the sleep."));
            return false;
        }
        int safeGuard = own ? battleScreen.FieldEffects.Safeguard.Self : battleScreen.FieldEffects.Safeguard.Opponent;
        if (safeGuard > 0 && op.Ability.Name.ToLower() != "infiltrator")
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Safeguard prevented the sleep."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "leaf guard" && battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Leaf Guard prevented the sleep."));
            return false;
        }
        ChangeCameraAngle(1, own, battleScreen);
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            AnimationQueryObject sleepAnimation = new AnimationQueryObject(pNPC, own == false);
            sleepAnimation.AnimationPlaySound(@"Battle\Effects\Asleep", 0, 0);
            Entity sleepEntity1 = sleepAnimation.SpawnEntity(new Vector3(0, 0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.5f), 1, 0, 1);
            sleepAnimation.AnimationChangeTexture(sleepEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 16, 16, 16), String.Empty), 1f, 1f);
            sleepAnimation.AnimationMove(sleepEntity1, true, 0, 0.5f, 0.25f, 0.01f, false, false, 0, 0);
            Entity sleepEntity2 = sleepAnimation.SpawnEntity(new Vector3(0, 0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.5f), 1, 1.5f, 1);
            sleepAnimation.AnimationChangeTexture(sleepEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Asleep", new Rectangle(0, 16, 16, 16), String.Empty), 2.5f, 1);
            sleepAnimation.AnimationMove(sleepEntity2, true, 0, 0.5f, 0.25f, 0.01f, false, false, 2, 0);
            battleScreen.BattleQuery.Add(sleepAnimation);
        }
        else
        {
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Asleep", false));
        }
        if (own == true)
        {
            battleScreen.FieldEffects.BideCounter.Self = 0;
            battleScreen.FieldEffects.BideDamage.Self = 0;
        }
        else
        {
            battleScreen.FieldEffects.BideCounter.Opponent = 0;
            battleScreen.FieldEffects.BideDamage.Opponent = 0;
        }
        ChangeCameraAngle(1, own, battleScreen);
        if (own == true)
        {
            battleScreen.FieldEffects.SleepTurns.Self = sleepTurns;
        }
        else
        {
            battleScreen.FieldEffects.SleepTurns.Opponent = sleepTurns;
        }
        p.Status = Pokemon.StatusProblems.Sleep;
        if (message == String.Empty)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " fell asleep!"));
        }
        else if (message == "-1") { }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(message));
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " fell asleep!"));
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "chesto" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:chesto") == true)
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    CureStatusProblem(own, own, battleScreen, "The Chesto Berry woke up " + p.GetDisplayName() + "!", "berry:chesto");
                }
            }
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "lum" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lum") == true)
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    CureStatusProblem(own, own, battleScreen, "The Lum Berry woke up " + p.GetDisplayName() + "!", "berry:lum");
                }
            }
        }
        return true;
    }

    public bool InflictPoison(bool own, bool from, BattleScreen battleScreen, bool bad, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (p.Status == Pokemon.StatusProblems.Poison || p.Status == Pokemon.StatusProblems.BadPoison)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already poisoned!"));
            return false;
        }
        if (p.Status != Pokemon.StatusProblems.None)
        {
            return false;
        }
        if (battleScreen.FieldEffects.MistyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the poison."));
            return false;
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute took the poison."));
            return false;
        }
        if (p.Type1.Type == Element.Types.Steel || p.Type1.Type == Element.Types.Poison || p.Type2.Type == Element.Types.Steel || p.Type2.Type == Element.Types.Poison)
        {
            return false;
        }
        if (p.Ability.Name.ToLower() == "immunity" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Immunity prevented the poison."));
            return false;
        }
        int safeGuard = own ? battleScreen.FieldEffects.Safeguard.Self : battleScreen.FieldEffects.Safeguard.Opponent;
        if (safeGuard > 0 && op.Ability.Name.ToLower() != "infiltrator")
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Safeguard prevented the poison."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "leaf guard" && battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Leaf Guard prevented the poison."));
            return false;
        }
        ChangeCameraAngle(1, own, battleScreen);
        if (bad == true)
        {
            p.Status = Pokemon.StatusProblems.BadPoison;
            if (message == String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is badly poisoned!"));
            }
            else if (message == "-1") { }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is badly poisoned!"));
            }
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                AnimationQueryObject poisonAnimation = new AnimationQueryObject(pNPC, own);
                poisonAnimation.AnimationPlaySound(@"Battle\Effects\Poisoned", 0, 0);
                Entity bubbleEntity1 = poisonAnimation.SpawnEntity(new Vector3(-0.25f, -0.25f, -0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 0, 1);
                poisonAnimation.AnimationChangeTexture(bubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 1f, 1f);
                Entity bubbleEntity2 = poisonAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 1, 1);
                poisonAnimation.AnimationChangeTexture(bubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 2, 1);
                poisonAnimation.AnimationChangeTexture(bubbleEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 2f, 1f);
                Entity bubbleEntity3 = poisonAnimation.SpawnEntity(new Vector3(0.25f, -0.25f, 0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 2, 1);
                poisonAnimation.AnimationChangeTexture(bubbleEntity2, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 3, 1);
                poisonAnimation.AnimationChangeTexture(bubbleEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 3f, 1f);
                poisonAnimation.AnimationChangeTexture(bubbleEntity3, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 4, 1);
                battleScreen.BattleQuery.Add(poisonAnimation);
            }
            else
            {
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Poisoned", false));
            }
        }
        else
        {
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                AnimationQueryObject poisonAnimation = new AnimationQueryObject(pNPC, own);
                poisonAnimation.AnimationPlaySound(@"Battle\Effects\Poisoned", 0, 0);
                Entity bubbleEntity1 = poisonAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 0, 1);
                poisonAnimation.AnimationChangeTexture(bubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 1f, 1f);
                poisonAnimation.AnimationChangeTexture(bubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 2, 1);
                battleScreen.BattleQuery.Add(poisonAnimation);
            }
            else
            {
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Poisoned", false));
            }
            p.Status = Pokemon.StatusProblems.Poison;
            if (message == String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is poisoned!"));
            }
            else if (message == "-1") { }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is poisoned!"));
            }
        }
        if (p.Ability.Name.ToLower() == "synchronize" && from != own)
        {
            InflictPoison(own == false, own == false, battleScreen, bad, "Synchronize passed over the" + bad.ToString() + " poison.", "synchronize");
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "pecha" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:pecha") == true)
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    CureStatusProblem(own, own, battleScreen, "The Pecha Berry cured the poison of " + p.GetDisplayName() + "!", "berry:pecha");
                }
            }
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "lum" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lum") == true)
                {
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    CureStatusProblem(own, own, battleScreen, "The Lum Berry cured the poison of " + p.GetDisplayName() + "!", "berry:lum");
                }
            }
        }
        return true;
    }

    public bool InflictConfusion(bool own, bool from, BattleScreen battleScreen, String message, String cause, int setConfusionTurns = -1)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true)
        {
            bool success = false;
            if (own == true)
            {
                if (battleScreen.FieldEffects.ConfusionTurns.Self < setConfusionTurns)
                {
                    success = true;
                }
            }
            else
            {
                if (battleScreen.FieldEffects.ConfusionTurns.Opponent < setConfusionTurns)
                {
                    success = true;
                }
            }
            if (success == false)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already confused!"));
                return false;
            }
        }
        if (battleScreen.FieldEffects.MistyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(own, battleScreen) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the confusion."));
            return false;
        }
        int confusionTurns = Core.Random.Next(1, 5);
        if (setConfusionTurns != -1)
        {
            confusionTurns = setConfusionTurns;
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute prevented the confusion."));
            return false;
        }
        else if (p.Ability.Name.ToLower() == "own tempo" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Own Tempo prevented the confusion."));
            return false;
        }
        ChangeCameraAngle(1, own, battleScreen);
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            AnimationQueryObject confusedAnimation = new AnimationQueryObject(pNPC, own);
            confusedAnimation.AnimationPlaySound(@"Battle\Effects\Confused", 0, 0);
            Entity duckEntity1 = confusedAnimation.SpawnEntity(new Vector3(-0.25f, 0.25f, -0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.25f), 1, 0, 0);
            Entity duckEntity2 = confusedAnimation.SpawnEntity(new Vector3(0, 0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.25f), 1, 0, 0);
            Entity duckEntity3 = confusedAnimation.SpawnEntity(new Vector3(0.25f, 0.25f, 0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), new Vector3(0.25f), 1, 0, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 0.75f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 0.75f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 0.75f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 1.5f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 1.5f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 1.5f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 2.25f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 2.25f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 2.25f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), 3.0f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), 3.0f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 0, 16, 16), String.Empty), 3.0f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 3.75f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 3.75f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 16, 16, 16), String.Empty), 3.75f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 4.5f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 4.5f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 32, 16, 16), String.Empty), 4.5f, 0);
            confusedAnimation.AnimationChangeTexture(duckEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 5.25f, 1);
            confusedAnimation.AnimationChangeTexture(duckEntity2, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 5.25f, 1);
            confusedAnimation.AnimationChangeTexture(duckEntity3, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Confused", new Rectangle(0, 48, 16, 16), String.Empty), 5.25f, 1);
            battleScreen.BattleQuery.Add(confusedAnimation);
        }
        else
        {
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Confused", false));
        }
        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == false)
        {
            p.AddVolatileStatus(Pokemon.VolatileStatus.Confusion);
        }
        if (message == String.Empty)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is confused!"));
        }
        else if (message == "-1") { }
        else
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(message));
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is confused!"));
        }
        if (own == true)
        {
            battleScreen.FieldEffects.ConfusionTurns.Self = confusionTurns;
        }
        else
        {
            battleScreen.FieldEffects.ConfusionTurns.Opponent = confusionTurns;
        }
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "persim" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:persim") == true)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    battleScreen.BattleQuery.Add(new TextQueryObject("The Persim Berry cured the confusion of " + p.GetDisplayName() + "!"));
                    if (own == true)
                    {
                        battleScreen.FieldEffects.ConfusionTurns.Self = 0;
                    }
                    else
                    {
                        battleScreen.FieldEffects.ConfusionTurns.Opponent = 0;
                    }
                    p.RemoveVolatileStatus(Pokemon.VolatileStatus.Confusion);
                }
            }
            else if (p.Item.OriginalName.ToLower() == "lum" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lum") == true)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
                    battleScreen.BattleQuery.Add(new TextQueryObject("The Lum Berry cured the confusion of " + p.GetDisplayName() + "!"));
                    if (own == true)
                    {
                        battleScreen.FieldEffects.ConfusionTurns.Self = 0;
                    }
                    else
                    {
                        battleScreen.FieldEffects.ConfusionTurns.Opponent = 0;
                    }
                    p.RemoveVolatileStatus(Pokemon.VolatileStatus.Confusion);
                }
            }
        }
        return true;
    }

    public bool RaiseStat(bool own, bool from, BattleScreen battleScreen, String stat, int val, String message, String cause, bool isGameModeMove = false)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (op.HP > 0 || op.Status != Pokemon.StatusProblems.Fainted)
        {
            if (from != own)
            {
                int mist = own ? battleScreen.FieldEffects.Mist.Self : battleScreen.FieldEffects.Mist.Opponent;
                if (mist > 0 && op.Ability.Name.ToLower() != "infiltrator")
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the status change!"));
                    return false;
                }
            }
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute prevented the stat change."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "contrary" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            if (cause != "contrary")
            {
                return LowerStat(own, own, battleScreen, stat, val, message + "Contrary reverted the stat change!", "contrary");
            }
        }
        if (p.Ability.Name.ToLower() == "simple" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            val *= 2;
        }
        String statString = stat.ToLower();
        switch (statString)
        {
            case "spdefense": statString = "special attack"; break;
            case "spattack": statString = "special defense"; break;
        }
        int statC = 0;
        switch (stat.ToLower())
        {
            case "attack": statC = p.StatAttack; break;
            case "defense": statC = p.StatDefense; break;
            case "special attack": statC = p.StatSpAttack; break;
            case "special defense": statC = p.StatSpDefense; break;
            case "speed": statC = p.StatSpeed; break;
            case "evasion": statC = p.Evasion; break;
            case "accuracy": statC = p.Accuracy; break;
        }
        if (statC >= 6)
        {
            ChangeCameraAngle(1, own, battleScreen);
            if (message == String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s " + statString + " cannot rise further."));
            }
            else if (message == "-1") { }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s " + statString + " cannot rise further."));
            }
            return false;
        }
        else
        {
            if (statC + val > 6)
            {
                val = 6 - statC;
            }
        }
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            AnimationQueryObject statAnimation = new AnimationQueryObject(null, false);
            for (int currentAmount = 0; currentAmount <= 20 * val; currentAmount++)
            {
                Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatChange\statUp");
                float xPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                float zPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                Vector3 position = new Vector3(xPos, -0.4f, zPos);
                float startDelay = (float)(5.0 * Core.Random.NextDouble());
                Entity statEntity = statAnimation.SpawnEntity(pNPC.Position + position, texture, new Vector3(0.2f), 1.0f, startDelay);
                statAnimation.AnimationMove(statEntity, true, 0, 1.2f, 0, 0.05f, false, true, startDelay, 0.0f);
            }
            statAnimation.AnimationPlaySound(@"Battle\Effects\Stat_Raise", 0, 0);
            battleScreen.BattleQuery.Add(statAnimation);
        }
        else
        {
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Stat_Raise", false));
        }
        String printMessage = p.GetDisplayName() + "'s " + statString;
        switch (val)
        {
            case 2: printMessage += " sharply rose!"; break;
            case 3:
            case 4:
            case 5: printMessage += " rose drastically!"; break;
            case 6: printMessage += " was maximized!"; break;
            default: printMessage += " slightly rose."; break;
        }
        switch (statString)
        {
            case "attack":
                p.StatAttack += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "defense":
                p.StatDefense += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "special attack":
                p.StatSpAttack += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "special defense":
                p.StatSpDefense += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "speed":
                p.StatSpeed += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "evasion":
                p.Evasion += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "accuracy":
                p.Accuracy += val;
                ChangeCameraAngle(1, own, battleScreen);
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
        }
        Logger.Log(Logger.LogTypes.Warning, "BattleV2.vb: Failed to indicate stat change: " + stat.ToUpper() + "!");
        return true;
    }

    public bool LowerStat(bool own, bool from, BattleScreen battleScreen, String stat, int val, String message, String cause, bool isGameModeMove = false)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (op.HP > 0 && op.Status != Pokemon.StatusProblems.Fainted)
        {
            if (from != own)
            {
                int mist = own ? battleScreen.FieldEffects.Mist.Self : battleScreen.FieldEffects.Mist.Opponent;
                if (mist > 0 && op.Ability.Name.ToLower() != "infiltrator")
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("The mist prevented the status change!"));
                    return false;
                }
                int guardSpec = own ? battleScreen.FieldEffects.GuardSpec.Self : battleScreen.FieldEffects.GuardSpec.Opponent;
                if (guardSpec > 0 && op.Ability.Name.ToLower() != "infiltrator")
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("Guard Spec. prevented the stat change!"));
                    return false;
                }
            }
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute prevented the stat change."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "contrary" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            if (cause != "contrary")
            {
                return RaiseStat(own, own, battleScreen, stat, val, message + "Contrary reverted the stat change!", "contrary");
            }
        }
        if (p.Ability.Name.ToLower() == "simple" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            val *= 2;
        }
        if (p.Ability.Name.ToLower() == "clear body" || p.Ability.Name.ToLower() == "white smoke")
        {
            if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
            {
                if (own != from)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject("The " + p.Ability.Name + " prevented the status change!"));
                    return false;
                }
            }
        }
        String statString = stat.ToLower();
        switch (statString)
        {
            case "attack":
                if (p.Ability.Name.ToLower() == "hyper cutter" && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject("Hyper Cutter prevented attack drop!"));
                    return false;
                }
                break;
            case "defense":
                if (p.Ability.Name.ToLower() == "big pecks" && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject("Big Pecks prevented defense drop!"));
                    return false;
                }
                break;
            case "accuracy":
                if (p.Ability.Name.ToLower() == "keen eye" && from != own && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    battleScreen.BattleQuery.Add(new TextQueryObject("Keen Eye prevented accuracy drop!"));
                    return false;
                }
                break;
        }
        switch (statString)
        {
            case "spdefense": statString = "special defense"; break;
            case "spattack": statString = "special attack"; break;
        }
        int statC = 0;
        switch (stat.ToLower())
        {
            case "attack": statC = p.StatAttack; break;
            case "defense": statC = p.StatDefense; break;
            case "special attack": statC = p.StatSpAttack; break;
            case "special defense": statC = p.StatSpDefense; break;
            case "speed": statC = p.StatSpeed; break;
            case "evasion": statC = p.Evasion; break;
            case "accuracy": statC = p.Accuracy; break;
        }
        if (statC <= -6)
        {
            ChangeCameraAngle(1, own, battleScreen);
            if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s " + statString + " cannot fall further.")); }
            else if (message == "-1") { }
            else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s " + statString + " cannot fall further.")); }
            return false;
        }
        else
        {
            if (statC - val < -6)
            {
                val = 6 + statC;
            }
        }
        ChangeCameraAngle(1, own, battleScreen);
        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
        {
            AnimationQueryObject statAnimation = new AnimationQueryObject(null, false);
            for (int currentAmount = 0; currentAmount <= 20 * val; currentAmount++)
            {
                Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatChange\statDown");
                float xPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                float zPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                Vector3 position = new Vector3(xPos, 0.8f, zPos);
                float startDelay = (float)(5.0 * Core.Random.NextDouble());
                Entity statEntity = statAnimation.SpawnEntity(pNPC.Position + position, texture, new Vector3(0.2f), 1.0f, startDelay);
                statAnimation.AnimationMove(statEntity, true, 0, -1.2f, 0, 0.05f, false, true, startDelay, 0.0f);
            }
            statAnimation.AnimationPlaySound(@"Battle\Effects\Stat_Lower", 0, 0);
            battleScreen.BattleQuery.Add(statAnimation);
        }
        else
        {
            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Stat_Lower", false));
        }
        String printMessage = p.GetDisplayName() + "'s " + statString;
        switch (val)
        {
            case 2: printMessage += " sharply fell!"; break;
            case 3:
            case 4:
            case 5: printMessage += " fell drastically!"; break;
            case 6: printMessage += " was minimized!"; break;
            default: printMessage += " slightly fell."; break;
        }
        switch (statString)
        {
            case "attack":
                p.StatAttack -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "defense":
                p.StatDefense -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "special attack":
                p.StatSpAttack -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "special defense":
                p.StatSpDefense -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "speed":
                p.StatSpeed -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "evasion":
                p.Evasion -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                return true;
            case "accuracy":
                p.Accuracy -= val;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); if (isGameModeMove == false) { battleScreen.BattleQuery.Add(new TextQueryObject(printMessage)); } }
                if (val > 0)
                {
                    if (p.Ability.Name.ToLower() == "defiant" && from != own)
                    {
                        RaiseStat(own, own, battleScreen, "Attack", 2, p.GetDisplayName() + "'s Defiant raised its attack!", "defiant");
                    }
                    if (p.Ability.Name.ToLower() == "competitive" && from != own)
                    {
                        RaiseStat(own, own, battleScreen, "Special Attack", 2, p.GetDisplayName() + "'s Competitive raised its Special Attack!", "competitive");
                    }
                }
                return true;
        }
        Logger.Log(Logger.LogTypes.Warning, "BattleV2.vb: Failed to indicate stat change: " + stat.ToUpper() + "!");
        return true;
    }

    public bool InflictInfatuate(bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return false;
        }
        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is already infatuated."));
            return false;
        }
        if (p.Ability.Name.ToLower() == "oblivious" && battleScreen.FieldEffects.CanUseAbility(own, battleScreen) == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Oblivious prevented the infatuation."));
            return false;
        }
        int substitute = own ? battleScreen.FieldEffects.Substitute.Self : battleScreen.FieldEffects.Substitute.Opponent;
        if (substitute > 0 && op.Ability.Name.ToLower() != "infiltrator" && from != own)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("The substitute prevented the infatuation."));
            return false;
        }
        ChangeCameraAngle(1, own, battleScreen);
        if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " fell in love.")); }
        else if (message == "-1") { }
        else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " fell in love.")); }
        p.AddVolatileStatus(Pokemon.VolatileStatus.Infatuation);
        if (p.Item != null)
        {
            if (p.Item.OriginalName.ToLower() == "destiny knot" && from != own && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                InflictInfatuate(own == false, own == false, battleScreen, "Destiny Knot reflects the infatuation.", "destinyknot");
            }
        }
        return true;
    }

    public void InflictRecoil(bool own, bool from, BattleScreen battleScreen, Attack moveUsed, int damage, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
        {
            return;
        }
        if (p.Ability.Name.ToLower() == "rock head" && cause.StartsWith("move:") == true)
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Rock Head prevented the recoil damage of " + p.GetDisplayName() + "!"));
        }
        else if (p.Ability.Name.ToLower() == "magic guard")
        {
            ChangeCameraAngle(1, own, battleScreen);
            battleScreen.BattleQuery.Add(new TextQueryObject("Magic Guard prevented the recoil damage of " + p.GetDisplayName() + "!"));
        }
        else
        {
            ChangeCameraAngle(1, own, battleScreen);
            if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is damaged by recoil!")); }
            else if (message == "-1") { }
            else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is damaged by recoil!")); }
            ReduceHP(damage, own, from, battleScreen, String.Empty, "recoildamage");
        }
    }

    public void GainHP(int hpAmount, bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        NPC pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP < p.MaxHP && p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted)
        {
            if (own == true) { ChangeCameraAngle(1, true, battleScreen); }
            else { ChangeCameraAngle(2, true, battleScreen); }
            if (hpAmount > p.MaxHP - p.HP)
            {
                hpAmount = p.MaxHP - p.HP;
            }
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                AnimationQueryObject healAnimation = new AnimationQueryObject(pNPC, own == false);
                for (int currentAmount = 0; currentAmount <= 20; currentAmount++)
                {
                    Texture2D texture = TextureManager.GetTexture(@"Textures\Battle\StatChange\Heal");
                    float xPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                    float zPos = (float)((Core.Random.NextDouble() - 0.5) * 1.2);
                    Vector3 position = new Vector3(xPos, -0.4f, zPos);
                    Vector3 destination = new Vector3(xPos, 0.8f, zPos);
                    float startDelay = (float)(5.0 * Core.Random.NextDouble());
                    Entity healEntity = healAnimation.SpawnEntity(position, texture, new Vector3(0.2f), 1.0f, startDelay);
                    healAnimation.AnimationMove(healEntity, true, destination.X, destination.Y, destination.Z, 0.05f, false, true, startDelay, 0.0f);
                }
                healAnimation.AnimationPlaySound(@"Battle\Effects\Heal", 0, 0);
                battleScreen.BattleQuery.Add(healAnimation);
            }
            else
            {
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Heal", false));
            }
            if (own == true)
            {
                battleScreen.BattleQuery.Add(new MathHPQueryObject(p.HP, p.MaxHP, -hpAmount, new Vector2(200, 256)));
            }
            else
            {
                battleScreen.BattleQuery.Add(new MathHPQueryObject(p.HP, p.MaxHP, -hpAmount, new Vector2(300, 256)));
            }
            if (message != String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
            }
            p.HP += hpAmount;
            p.HP = p.HP.Clamp(0, p.MaxHP);
        }
    }

    public void ReduceHP(int hpAmount, bool own, bool from, BattleScreen battleScreen, String message, String cause)
    {
        ReduceHP(hpAmount, own, from, battleScreen, message, cause, String.Empty);
    }

    public void ReduceHP(int hpAmount, bool own, bool from, BattleScreen battleScreen, String message, String cause, String sound)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Entity pNPC = own ? battleScreen.SelfPokemonNPC : battleScreen.OpponentPokemonNPC;
        if (p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted)
        {
            if (own == true) { ChangeCameraAngle(1, true, battleScreen); }
            else { ChangeCameraAngle(2, true, battleScreen); }
            if (sound != "NOSOUND")
            {
                if (sound == String.Empty)
                {
                    sound = @"Battle\Damage\Effective";
                }
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(sound, false, 0.0f));
            }
            int fly = own ? battleScreen.FieldEffects.FlyCounter.Self : battleScreen.FieldEffects.FlyCounter.Opponent;
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                if (fly == 0)
                {
                    AnimationQueryObject hitAnimation = new AnimationQueryObject(pNPC, own);
                    hitAnimation.AnimationFade(null, false, 1, 0, 0, 0);
                    hitAnimation.AnimationFade(null, false, 1, 1, 1, 0);
                    hitAnimation.AnimationFade(null, false, 1, 0, 2, 0);
                    hitAnimation.AnimationFade(null, false, 1, 1, 3, 0);
                    battleScreen.BattleQuery.Add(hitAnimation);
                }
            }
            if (own == true)
            {
                battleScreen.BattleQuery.Add(new MathHPQueryObject(p.HP, p.MaxHP, hpAmount, new Vector2(200, 256)));
            }
            else
            {
                battleScreen.BattleQuery.Add(new MathHPQueryObject(p.HP, p.MaxHP, hpAmount, new Vector2(300, 256)));
            }
            if (message != String.Empty)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
            }
            p.HP -= hpAmount;
            p.HP = p.HP.Clamp(0, p.MaxHP);
            if (battleScreen.IsTrainerBattle == true && cause.Contains("battledamage") == true && (float)hpAmount / p.MaxHP * 100 > 60 && p.HP > 0)
            {
                if (own == true)
                {
                    battleScreen.TrainerBigDamageOwn += 1;
                    if (battleScreen.Trainer.BigDamageOwnMessage.ContainsKey(battleScreen.TrainerBigDamageOwn))
                    {
                        QueryObject s1 = battleScreen.FocusOppPlayer();
                        TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.BigDamageOwnMessage[battleScreen.TrainerBigDamageOwn]).ToString());
                        battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                        ChangeCameraAngle(1, true, battleScreen);
                    }
                }
                else
                {
                    battleScreen.TrainerBigDamageOpp += 1;
                    if (battleScreen.Trainer.BigDamageOppMessage.ContainsKey(battleScreen.TrainerBigDamageOpp))
                    {
                        QueryObject s1 = battleScreen.FocusOppPlayer();
                        TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.BigDamageOppMessage[battleScreen.TrainerBigDamageOpp]).ToString());
                        battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                        ChangeCameraAngle(2, true, battleScreen);
                    }
                }
            }
            String itemID = "-1";
            if (p.Item != null)
            {
                itemID = p.Item.IsGameModeItem == true ? p.Item.gmID : p.Item.ID.ToString();
            }
            Attack lastMove = own ? battleScreen.FieldEffects.LastMove.Opponent : battleScreen.FieldEffects.LastMove.Self;
            if (lastMove != null)
            {
                float effectiveness = BattleCalculation.CalculateEffectiveness(own == false, lastMove, battleScreen);
                if (effectiveness > 1.0f)
                {
                    if (p.Item != null)
                    {
                        if (p.Item.OriginalName.ToLower() == "enigma" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                        {
                            if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:enigma") == true)
                            {
                                UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause);
                            }
                        }
                    }
                }
            }
            if (p.HP > 0 && p.HP < (int)Math.Ceiling((double)p.MaxHP / 3))
            {
                if (p.Item != null)
                {
                    if (p.Item.OriginalName.ToLower() == "oran" && battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                    {
                        if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:oran") == true)
                        {
                            UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause);
                        }
                    }
                }
            }
            if (p.HP > 0 && p.HP < (int)Math.Ceiling((double)p.MaxHP / 2))
            {
                if (p.Item != null)
                {
                    if (battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                    {
                        switch (p.Item.OriginalName.ToLower())
                        {
                            case "sitrus":
                                if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:sitrus") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); }
                                break;
                        }
                    }
                }
            }
            if (p.HP > 0 && p.HP < (int)Math.Ceiling((double)p.MaxHP / 4))
            {
                if (p.Item != null)
                {
                    if (battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
                    {
                        switch (p.Item.OriginalName.ToLower())
                        {
                            case "figy": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:figy") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "wiki": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:wiki") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "mago": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:mago") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "aguav": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:aguav") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "iapapa": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:iapapa") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "liechi": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:liechi") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "ganlon": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:ganlon") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "salac": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:salac") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "petaya": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:petaya") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "apicot": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:apicot") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "lansat": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:lansat") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "starf": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:starf") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "micle": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:micle") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                            case "custap": if (RemoveHeldItem(own, own, battleScreen, String.Empty, "berry:custap") == true) { UseBerry(own, from, Item.GetItemByID(itemID.ToString()), battleScreen, message, cause); } break;
                        }
                    }
                }
            }
        }
        if (p.HP <= 0 && cause.StartsWith("move:") == false && cause.Contains("battledamage") == false)
        {
            FaintPokemon(own, battleScreen, String.Empty);
        }
    }

    private void UseEffectBerry(bool own, bool from, Item berryItem, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        switch (berryItem.Name.ToLower())
        {
            case "lum":
                if (p.Status == Pokemon.StatusProblems.Burn) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Lum Berry cured the burn of " + p.GetDisplayName() + "!", "berry:lum"); }
                if (p.Status == Pokemon.StatusProblems.Freeze) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Lum Berry thraw out " + p.GetDisplayName() + "!", "berry:lum"); }
                if (p.Status == Pokemon.StatusProblems.Paralyzed) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Lum Berry cured the paralysis of " + p.GetDisplayName() + "!", "berry:lum"); }
                if (p.Status == Pokemon.StatusProblems.Sleep) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Lum Berry woke up " + p.GetDisplayName() + "!", "berry:lum"); }
                break;
            case "rawst":
                if (p.Status == Pokemon.StatusProblems.Burn) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Rawst Berry cured the burn of " + p.GetDisplayName() + "!", "berry:rawst"); }
                break;
            case "aspear":
                if (p.Status == Pokemon.StatusProblems.Freeze) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Aspear Berry thraw out " + p.GetDisplayName() + "!", "berry:aspear"); }
                break;
            case "cheri":
                if (p.Status == Pokemon.StatusProblems.Paralyzed) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Cheri Berry cured the paralysis of " + p.GetDisplayName() + "!", "berry:cheri"); }
                break;
            case "chesto":
                if (p.Status == Pokemon.StatusProblems.Sleep) { battleScreen.BattleQuery.Add(new PlaySoundQueryObject("single_heal", false)); CureStatusProblem(own, own, battleScreen, "The Chesto Berry woke up " + p.GetDisplayName() + "!", "berry:chesto"); }
                break;
        }
    }

    public void UseBerry(bool own, bool from, Item berryItem, BattleScreen battleScreen, String message, String cause)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Items.Berry berry = (Items.Berry)berryItem;
        battleScreen.BattleQuery.Add(new PlaySoundQueryObject("Use_Item", false));
        switch (berryItem.OriginalName.ToLower())
        {
            case "oran":
                GainHP(10, own, from, battleScreen, "The Oran Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:oran");
                break;
            case "sitrus":
                GainHP((int)(p.MaxHP / 4), own, own, battleScreen, "The Sitrus Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:sitrus");
                break;
            case "figy":
                GainHP((int)Math.Ceiling((double)p.MaxHP / 3), own, own, battleScreen, "The Figy Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:figy");
                if (berry.PokemonLikes(p) == false) { InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " disliked the Figy Berry!", "berry:figy"); }
                break;
            case "wiki":
                GainHP((int)Math.Ceiling((double)p.MaxHP / 3), own, own, battleScreen, "The Wiki Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:wiki");
                if (berry.PokemonLikes(p) == false) { InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " disliked the Wiki Berry!", "berry:wiki"); }
                break;
            case "mago":
                GainHP((int)Math.Ceiling((double)p.MaxHP / 3), own, own, battleScreen, "The Mago Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:mago");
                if (berry.PokemonLikes(p) == false) { InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " disliked the Mago Berry!", "mago"); }
                break;
            case "aguav":
                GainHP((int)Math.Ceiling((double)p.MaxHP / 3), own, own, battleScreen, "The Aguav Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:aguav");
                if (berry.PokemonLikes(p) == false) { InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " disliked the Aguav Berry!", "aguav"); }
                break;
            case "iapapa":
                GainHP((int)Math.Ceiling((double)p.MaxHP / 3), own, own, battleScreen, "The Iapapa Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:iapapa");
                if (berry.PokemonLikes(p) == false) { InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " disliked the Iapapa Berry!", "berry:iapapa"); }
                break;
            case "liechi":
                RaiseStat(own, own, battleScreen, "Attack", 2, "The Liechi Berry raised " + p.GetDisplayName() + "'s power!", "berry:liechi");
                break;
            case "ganlon":
                RaiseStat(own, own, battleScreen, "Defense", 2, "The Ganlon Berry raised " + p.GetDisplayName() + "'s power!", "berry:ganlon");
                break;
            case "salac":
                RaiseStat(own, own, battleScreen, "Speed", 2, "The Salac Berry raised " + p.GetDisplayName() + "'s power!", "berry:salac");
                break;
            case "petaya":
                RaiseStat(own, own, battleScreen, "Special Attack", 2, "The Petaya Berry raised " + p.GetDisplayName() + "'s power!", "berry:petaya");
                break;
            case "apicot":
                RaiseStat(own, own, battleScreen, "Special Defense", 2, "The Apicot Berry raised " + p.GetDisplayName() + "'s power!", "berry:apicot");
                break;
            case "lansat":
                if (own == true) { battleScreen.FieldEffects.LansatBerry.Self = 1; }
                else { battleScreen.FieldEffects.LansatBerry.Opponent = 1; }
                battleScreen.BattleQuery.Add(new TextQueryObject("The Lansat Berry raised " + p.GetDisplayName() + "'s power!"));
                break;
            case "starf":
                String statStar;
                switch (Core.Random.Next(0, 7))
                {
                    case 0: statStar = "Attack"; break;
                    case 1: statStar = "Defense"; break;
                    case 2: statStar = "Special Attack"; break;
                    case 3: statStar = "Special Defense"; break;
                    case 4: statStar = "Speed"; break;
                    case 5: statStar = "Accuracy"; break;
                    default: statStar = "Evasion"; break;
                }
                RaiseStat(own, own, battleScreen, statStar, 2, "The Starf Berry raised " + p.GetDisplayName() + "'s power!", "berry:starf");
                break;
            case "micle":
                RaiseStat(own, own, battleScreen, "Accuracy", 2, "The Micle Berry raised " + p.GetDisplayName() + "'s power!", "berry:micle");
                break;
            case "custap":
                ChangeCameraAngle(1, own, battleScreen);
                if (own == true) { battleScreen.FieldEffects.CustapBerry.Self = 1; }
                else { battleScreen.FieldEffects.CustapBerry.Opponent = 1; }
                battleScreen.BattleQuery.Add(new TextQueryObject("The Custap Berry gave " + p.GetDisplayName() + " a speed boost!"));
                break;
            case "enigma":
                GainHP((int)(p.MaxHP / 4), own, own, battleScreen, "The Enigma Berry filled up " + p.GetDisplayName() + "'s HP!", "berry:enigma");
                break;
        }
        if (p.Ability.Name.ToLower() == "cheek pouch")
        {
            GainHP((int)(p.MaxHP / 8), own, own, battleScreen, "Cheek Pouch healed some HP.", "cheekpouch");
        }
    }

    public bool RemoveHeldItem(bool own, bool from, BattleScreen battleScreen, String message, String cause, bool testFor = false, bool affectsFainted = false)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        if (p.Item == null)
        {
            return false;
        }
        if (affectsFainted == false)
        {
            if (p.HP <= 0 || p.Status == Pokemon.StatusProblems.Fainted)
            {
                return false;
            }
        }
        if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen) && p.Ability.Name.ToLower() == "sticky hold" && cause.StartsWith("berry:") == false)
        {
            if (testFor == false)
            {
                ChangeCameraAngle(1, own, battleScreen);
                battleScreen.BattleQuery.Add(new TextQueryObject("Sticky Hold prevented the item loss."));
            }
            return false;
        }
        if (testFor == false)
        {
            String itemID = p.Item.IsGameModeItem == true ? p.Item.gmID : p.Item.ID.ToString();
            Item lostItem = Item.GetItemByID(itemID);
            String[] cudChewBerries = { "oran", "sitrus", "figy", "wiki", "mago", "aguav", "iapapa", "liechi", "ganlon", "salac", "petaya", "apicot", "lansat", "starf", "lum", "rawst", "aspear", "cheri", "chesto" };
            if (cudChewBerries.Contains(lostItem.Name.ToLower()) && lostItem.IsBerry == true && p.Ability.Name.ToLower() == "cud chew")
            {
                if (own == true)
                {
                    battleScreen.FieldEffects.CudChewBerry.Self = lostItem;
                    battleScreen.FieldEffects.CudChewIndex.Self = battleScreen.SelfPokemonIndex;
                }
                else
                {
                    battleScreen.FieldEffects.CudChewBerry.Opponent = lostItem;
                    battleScreen.FieldEffects.CudChewIndex.Opponent = battleScreen.OpponentPokemonIndex;
                }
            }
            if (from == own)
            {
                if (own == true) { battleScreen.FieldEffects.ConsumedItem.Self = lostItem; }
                else { battleScreen.FieldEffects.ConsumedItem.Opponent = lostItem; }
            }
            p.Item = null;
            if (p.Ability.Name.ToLower() == "unburden")
            {
                RaiseStat(own, own, battleScreen, "Speed", 2, "Unburden raised the speed!", "unburden");
            }
            ChangeCameraAngle(1, own, battleScreen);
            if (message == String.Empty)
            {
                if (cause.StartsWith("berry:") == true) { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " ate the " + lostItem.OneLineName() + " Berry!")); }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " lost the " + lostItem.OneLineName() + "!")); }
            }
            else if (message == "-1") { }
            else
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                if (cause.StartsWith("berry:") == true) { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " ate the " + lostItem.OneLineName() + " Berry!")); }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " lost the " + lostItem.OneLineName() + "!")); }
            }
        }
        return true;
    }

    public void ChangeWeather(bool own, bool from, BattleWeather.WeatherTypes newWeather, int turns, BattleScreen battleScreen, String message, String cause)
    {
        if (battleScreen.FieldEffects.Weather != newWeather)
        {
            if (newWeather != BattleWeather.WeatherTypes.Clear)
            {
                int weatherRounds = turns == -1 ? 5 : turns;
                Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
                Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
                if (op.Ability.Name.ToLower() == "air lock" || op.Ability.Name.ToLower() == "cloud nine")
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s " + op.Ability.Name + " prevented the weather change!")); }
                    else if (message == "-1") { }
                    else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + "'s " + op.Ability.Name + " prevented the weather change!")); }
                    ApplyForecast(battleScreen);
                    return;
                }
                if (p.Ability.Name.ToLower() == "air lock" || p.Ability.Name.ToLower() == "cloud nine")
                {
                    ChangeCameraAngle(1, own, battleScreen);
                    if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s " + p.Ability.Name + " prevented the weather change!")); }
                    else if (message == "-1") { }
                    else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s " + p.Ability.Name + " prevented the weather change!")); }
                    ApplyForecast(battleScreen);
                    return;
                }
                battleScreen.FieldEffects.Weather = newWeather;
                battleScreen.FieldEffects.WeatherRounds = weatherRounds;
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject("The weather changed to " + newWeather.ToString() + "!")); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject("The weather changed to " + newWeather.ToString() + "!")); }
                ApplyForecast(battleScreen);
            }
            else
            {
                if (message == String.Empty) { battleScreen.BattleQuery.Add(new TextQueryObject("The effects of weather disappeared.")); }
                else if (message == "-1") { }
                else { battleScreen.BattleQuery.Add(new TextQueryObject(message)); battleScreen.BattleQuery.Add(new TextQueryObject("The effects of weather disappeared.")); }
                ApplyForecast(battleScreen);
            }
        }
    }

    public void TriggerAbilityEffect(BattleScreen battleScreen, bool own)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        Pokemon op = own ? battleScreen.OpponentPokemon : battleScreen.SelfPokemon;
        int turns = BattleCalculation.FieldEffectTurns(battleScreen, own);
        if (battleScreen.FieldEffects.CanUseAbility(own, battleScreen, 1) == true)
        {
            switch (p.Ability.Name.ToLower())
            {
                case "drizzle":
                    ChangeWeather(own, own, BattleWeather.WeatherTypes.Rain, turns, battleScreen, "Drizzle makes it rain!", "drizzle");
                    break;
                case "cloud nine":
                    ChangeWeather(own, own, BattleWeather.WeatherTypes.Clear, 0, battleScreen, String.Empty, "cloudnine");
                    break;
                case "intimidate":
                    if (op.Ability.Name.ToLower() == "oblivious" || op.Ability.Name.ToLower() == "inner focus" || op.Ability.Name.ToLower() == "own tempo" || op.Ability.Name.ToLower() == "scrappy")
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject("Intimidate's effect was prevented!"));
                    }
                    else
                    {
                        LowerStat(own == false, own, battleScreen, "Attack", 1, p.GetDisplayName() + "'s Intimidate cuts " + op.GetDisplayName() + "'s attack!", "intimidate");
                    }
                    if (op.Ability.Name.ToLower() == "rattled")
                    {
                        RaiseStat(own == false, own == false, battleScreen, "Speed", 1, op.GetDisplayName() + "'s Rattled affected it's clairaudience.", "rattled");
                    }
                    break;
                case "trace":
                    if (op.Ability.Name.ToLower() != "multitype" && op.Ability.Name.ToLower() != "illusion")
                    {
                        p.Ability = op.Ability;
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " copied the ability " + op.Ability.Name + " from " + op.GetDisplayName() + "!"));
                    }
                    break;
                case "sand stream":
                    ChangeWeather(own, own, BattleWeather.WeatherTypes.Sandstorm, turns, battleScreen, "Sand Stream creates a sandstorm!", "sandstream");
                    break;
                case "forecast":
                    ApplyForecast(battleScreen);
                    break;
                case "drought":
                    ChangeWeather(own, own, BattleWeather.WeatherTypes.Sunny, turns, battleScreen, "The sunlight turned harsh!", "drought");
                    break;
                case "air lock":
                    ChangeWeather(own, own, BattleWeather.WeatherTypes.Clear, 0, battleScreen, String.Empty, "airlock");
                    break;
                case "electric surge":
                    if (battleScreen.FieldEffects.ElectricTerrain <= 0)
                    {
                        battleScreen.FieldEffects.ElectricTerrain = turns;
                        battleScreen.FieldEffects.GrassyTerrain = 0;
                        battleScreen.FieldEffects.PsychicTerrain = 0;
                        battleScreen.FieldEffects.MistyTerrain = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject("An electric current runs across the battlefield!"));
                    }
                    break;
                case "grassy surge":
                    if (battleScreen.FieldEffects.GrassyTerrain <= 0)
                    {
                        battleScreen.FieldEffects.ElectricTerrain = 0;
                        battleScreen.FieldEffects.GrassyTerrain = turns;
                        battleScreen.FieldEffects.PsychicTerrain = 0;
                        battleScreen.FieldEffects.MistyTerrain = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject("Grass grew to cover the battlefield!"));
                    }
                    break;
                case "misty surge":
                    if (battleScreen.FieldEffects.MistyTerrain <= 0)
                    {
                        battleScreen.FieldEffects.ElectricTerrain = 0;
                        battleScreen.FieldEffects.GrassyTerrain = 0;
                        battleScreen.FieldEffects.PsychicTerrain = 0;
                        battleScreen.FieldEffects.MistyTerrain = turns;
                        battleScreen.BattleQuery.Add(new TextQueryObject("Mist swirls around the battlefield!"));
                    }
                    break;
                case "psychic surge":
                    if (battleScreen.FieldEffects.PsychicTerrain <= 0)
                    {
                        battleScreen.FieldEffects.ElectricTerrain = 0;
                        battleScreen.FieldEffects.GrassyTerrain = 0;
                        battleScreen.FieldEffects.PsychicTerrain = turns;
                        battleScreen.FieldEffects.MistyTerrain = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject("The battlefield got weird!"));
                    }
                    break;
                case "download":
                    if (op.Defense < op.SpDefense)
                    {
                        RaiseStat(own, own, battleScreen, "Attack", 1, "Download analyzed the foe!", "download");
                    }
                    else
                    {
                        RaiseStat(own, own, battleScreen, "Special Attack", 1, "Download analyzed the foe!", "download");
                    }
                    break;
                case "mold breaker":
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " breaks the mold!"));
                    break;
                case "turbo blaze":
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is radiating a blazing aura!"));
                    break;
                case "teravolt":
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " is radiating a bursting aura!"));
                    break;
                case "anticipation":
                    break;
                case "forewarn":
                    break;
                case "snow warning":
                    ChangeWeather(own, own, BattleWeather.WeatherTypes.Hailstorm, turns, battleScreen, "Snow Warning summoned a hailstorm!", "snowwarning");
                    break;
                case "frisk":
                    if (op.Item != null)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(op.GetDisplayName() + " is holding " + op.Item.OneLineName() + "."));
                    }
                    break;
                case "multitype":
                    p.OriginalType1 = p.Type1;
                    p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Normal);
                    p.Type2 = new Element(Element.Types.Blank);
                    if (p.Item != null)
                    {
                        bool changeType = false;
                        Element newType = null;
                        switch (p.Item.OriginalName.ToLower())
                        {
                            case "draco plate": changeType = true; newType = new Element(Element.Types.Dragon); break;
                            case "dread plate": changeType = true; newType = new Element(Element.Types.Dark); break;
                            case "earth plate": changeType = true; newType = new Element(Element.Types.Ground); break;
                            case "fist plate": changeType = true; newType = new Element(Element.Types.Fighting); break;
                            case "flame plate": changeType = true; newType = new Element(Element.Types.Fire); break;
                            case "icicle plate": changeType = true; newType = new Element(Element.Types.Ice); break;
                            case "insect plate": changeType = true; newType = new Element(Element.Types.Bug); break;
                            case "iron plate": changeType = true; newType = new Element(Element.Types.Steel); break;
                            case "meadow plate": changeType = true; newType = new Element(Element.Types.Grass); break;
                            case "mind plate": changeType = true; newType = new Element(Element.Types.Psychic); break;
                            case "sky plate": changeType = true; newType = new Element(Element.Types.Flying); break;
                            case "splash plate": changeType = true; newType = new Element(Element.Types.Water); break;
                            case "spooky plate": changeType = true; newType = new Element(Element.Types.Ghost); break;
                            case "stone plate": changeType = true; newType = new Element(Element.Types.Rock); break;
                            case "toxic plate": changeType = true; newType = new Element(Element.Types.Poison); break;
                            case "zap plate": changeType = true; newType = new Element(Element.Types.Electric); break;
                        }
                        if (changeType == true)
                        {
                            p.Type1 = newType;
                            p.Type2 = new Element(Element.Types.Blank);
                        }
                    }
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s type changed to " + p.Type1.ToString() + "!"));
                    break;
                case "imposter":
                    ChangeCameraAngle(1, own, battleScreen);
                    if (op.IsTransformed == false)
                    {
                        p.OriginalNumber = p.Number;
                        p.OriginalType1 = new Element(p.Type1.Type);
                        p.OriginalType2 = new Element(p.Type2.Type);
                        p.OriginalStats = new int[] { p.Attack, p.Defense, p.SpAttack, p.SpDefense, p.Speed };
                        p.OriginalShiny = p.IsShiny ? 1 : 0;
                        p.OriginalMoves = new System.Collections.Generic.List<Attack>();
                        p.OriginalMoves.AddRange(p.Attacks.ToArray());
                        p.Number = op.Number;
                        p.Type1 = new Element(op.Type1.Type);
                        p.Type2 = new Element(op.Type2.Type);
                        p.Attack = op.Attack;
                        p.Defense = op.Defense;
                        p.SpAttack = op.SpAttack;
                        p.SpDefense = op.SpDefense;
                        p.Speed = op.Speed;
                        p.StatAttack = op.StatAttack;
                        p.StatDefense = op.StatDefense;
                        p.StatSpAttack = op.StatSpAttack;
                        p.StatSpDefense = op.StatSpDefense;
                        p.StatSpeed = op.StatSpeed;
                        p.IsShiny = op.IsShiny;
                        p.Attacks.Clear();
                        for (int i = 0; i <= op.Attacks.Count - 1; i++)
                        {
                            p.Attacks.Add(Attack.GetAttackByID(op.Attacks[i].ID));
                            p.Attacks[i].CurrentPP = 5;
                        }
                        p.Ability = Ability.GetAbilityByID(op.Ability.ID);
                        p.IsTransformed = true;
                        battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(own, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(p, true), 0, 1, -1, -1));
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into " + op.GetName() + "!"));
                    }
                    else
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject("Imposter failed!"));
                    }
                    break;
            }
        }
    }

    public void TriggerItemEffect(BattleScreen battleScreen, bool own)
    {
        Pokemon p = own ? battleScreen.SelfPokemon : battleScreen.OpponentPokemon;
        if (p.Item != null)
        {
            if (battleScreen.FieldEffects.CanUseItem(own) == true && battleScreen.FieldEffects.CanUseOwnItem(own, battleScreen) == true)
            {
                switch (p.Item.OriginalName.ToLower())
                {
                    case "electric seed":
                        if (battleScreen.FieldEffects.ElectricTerrain > 0 && p.StatDefense < 6)
                        {
                            if (RemoveHeldItem(own, own, battleScreen, "-1", String.Empty) == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s Electric Seed activated!"));
                                RaiseStat(own, own, battleScreen, "Defense", 1, String.Empty, "item:electricseed");
                            }
                        }
                        break;
                    case "grassy seed":
                        if (battleScreen.FieldEffects.GrassyTerrain > 0 && p.StatDefense < 6)
                        {
                            if (RemoveHeldItem(own, own, battleScreen, "-1", String.Empty) == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s Grassy Seed activated!"));
                                RaiseStat(own, own, battleScreen, "Defense", 1, String.Empty, "item:grassyseed");
                            }
                        }
                        break;
                    case "misty seed":
                        if (battleScreen.FieldEffects.MistyTerrain > 0 && p.StatSpDefense < 6)
                        {
                            if (RemoveHeldItem(own, own, battleScreen, "-1", String.Empty) == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s Misty Seed activated!"));
                                RaiseStat(own, own, battleScreen, "Special Defense", 1, String.Empty, "item:mistyseed");
                            }
                        }
                        break;
                    case "psychic seed":
                        if (battleScreen.FieldEffects.PsychicTerrain > 0 && p.StatSpDefense < 6)
                        {
                            if (RemoveHeldItem(own, own, battleScreen, "-1", String.Empty) == true)
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s Psychic Seed activated!"));
                                RaiseStat(own, own, battleScreen, "Special Defense", 1, String.Empty, "item:psychicseed");
                            }
                        }
                        break;
                    case "berserk gene":
                        if (RemoveHeldItem(own, own, battleScreen, "-1", String.Empty) == true)
                        {
                            if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == false)
                            {
                                if (own == true)
                                {
                                    if (battleScreen.SelfPokemonIndex > 0 && battleScreen.FieldEffects.TempConfusionTurns.Self > 0)
                                    {
                                        InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " went berserk due to the Berserk Gene!", "item:berserkgene", battleScreen.FieldEffects.TempConfusionTurns.Self);
                                    }
                                    else
                                    {
                                        InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " went berserk due to the Berserk Gene!", "item:berserkgene", 256);
                                    }
                                }
                                else
                                {
                                    if (battleScreen.OpponentPokemonIndex > 0 && battleScreen.FieldEffects.TempConfusionTurns.Opponent > 0)
                                    {
                                        InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " went berserk due to the Berserk Gene!", "item:berserkgene", battleScreen.FieldEffects.TempConfusionTurns.Opponent);
                                    }
                                    else
                                    {
                                        InflictConfusion(own, own, battleScreen, p.GetDisplayName() + " went berserk due to the Berserk Gene!", "item:berserkgene", 256);
                                    }
                                }
                            }
                            else
                            {
                                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " went berserk due to the Berserk Gene!"));
                            }
                            battleScreen.FieldEffects.TempConfusionTurns.Opponent = 0;
                            RaiseStat(own, own, battleScreen, "Attack", 2, String.Empty, "item:berserkgene");
                            RaiseStat(own, own, battleScreen, "Special Attack", 2, String.Empty, "item:berserkgene");
                        }
                        break;
                }
            }
        }
    }

    private void ApplyForecast(BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;
        if (p.Ability.Name.ToLower() == "forecast")
        {
            switch (battleScreen.FieldEffects.Weather)
            {
                case BattleWeather.WeatherTypes.Rain:
                    p.OriginalType1 = p.Type1; p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Water); p.Type2 = new Element(Element.Types.Blank);
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into a water type!"));
                    break;
                case BattleWeather.WeatherTypes.Sunny:
                    p.OriginalType1 = p.Type1; p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Fire); p.Type2 = new Element(Element.Types.Blank);
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into a fire type!"));
                    break;
                case BattleWeather.WeatherTypes.Hailstorm:
                    p.OriginalType1 = p.Type1; p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Ice); p.Type2 = new Element(Element.Types.Blank);
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into an ice type!"));
                    break;
            }
        }
        p = battleScreen.OpponentPokemon;
        if (p.Ability.Name.ToLower() == "forecast")
        {
            switch (battleScreen.FieldEffects.Weather)
            {
                case BattleWeather.WeatherTypes.Rain:
                    p.OriginalType1 = p.Type1; p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Water); p.Type2 = new Element(Element.Types.Blank);
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into a water type!"));
                    break;
                case BattleWeather.WeatherTypes.Sunny:
                    p.OriginalType1 = p.Type1; p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Fire); p.Type2 = new Element(Element.Types.Blank);
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into a fire type!"));
                    break;
                case BattleWeather.WeatherTypes.Hailstorm:
                    p.OriginalType1 = p.Type1; p.OriginalType2 = p.Type2;
                    p.Type1 = new Element(Element.Types.Ice); p.Type2 = new Element(Element.Types.Blank);
                    battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " transformed into an ice type!"));
                    break;
            }
        }
    }

    public void ChangeCameraAngle(int direction, bool own, BattleScreen battleScreen, bool addPVP = false)
    {
        CameraQueryObject q = null;
        switch (direction)
        {
            case 0:
                q = (CameraQueryObject)battleScreen.FocusBattle();
                break;
            case 1:
                q = (CameraQueryObject)(own == true ? battleScreen.FocusSelfPokemon() : battleScreen.FocusOpponentPokemon());
                break;
            case 2:
                q = (CameraQueryObject)(own == false ? battleScreen.FocusSelfPokemon() : battleScreen.FocusOpponentPokemon());
                break;
        }
        if (q != null)
        {
            q.ApplyCurrentCamera = true;
            q.ReplacePVP = true;
            if (addPVP == true)
            {
                if (battleScreen.TempPVPBattleQuery.ContainsKey(battleScreen.BattleQuery.Count - 1) == false)
                {
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 1, q);
                }
            }
            else
            {
                battleScreen.BattleQuery.Add(q);
            }
        }
        if (battleScreen.IsRemoteBattle == true && addPVP == false)
        {
            switch (direction)
            {
                case 0: ChangeCameraAngle(0, own, battleScreen, true); break;
                case 1: ChangeCameraAngle(2, own, battleScreen, true); break;
                case 2: ChangeCameraAngle(1, own, battleScreen, true); break;
            }
        }
    }

    private void EndRound(BattleScreen battleScreen, int type)
    {
        switch (type)
        {
            case 0:
                if (BattleCalculation.MovesFirst(battleScreen) == true)
                {
                    EndRoundOwn(battleScreen);
                    EndRoundOpp(battleScreen);
                }
                else
                {
                    EndRoundOpp(battleScreen);
                    EndRoundOwn(battleScreen);
                }
                if (isAfterFaint == false)
                {
                    battleScreen.FieldEffects.Rounds += 1;
                    if (battleScreen.FieldEffects.WeatherRounds > 0)
                    {
                        battleScreen.FieldEffects.WeatherRounds -= 1;
                        if (battleScreen.FieldEffects.WeatherRounds == 0)
                        {
                            battleScreen.FieldEffects.Weather = BattleWeather.WeatherTypes.Clear;
                            battleScreen.BattleQuery.Add(new TextQueryObject("The weather became clear again!"));
                        }
                    }
                    if (battleScreen.FieldEffects.TrickRoom > 0)
                    {
                        battleScreen.FieldEffects.TrickRoom -= 1;
                        if (battleScreen.FieldEffects.TrickRoom == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("The dimensions have returned to normal."));
                        }
                    }
                    if (battleScreen.FieldEffects.Gravity > 0)
                    {
                        battleScreen.FieldEffects.Gravity -= 1;
                        if (battleScreen.FieldEffects.Gravity == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("Gravity became normal again!"));
                        }
                    }
                    if (battleScreen.FieldEffects.ElectricTerrain > 0)
                    {
                        battleScreen.FieldEffects.ElectricTerrain -= 1;
                        if (battleScreen.FieldEffects.ElectricTerrain == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("The electricity disappeared from the battlefield."));
                        }
                    }
                    if (battleScreen.FieldEffects.GrassyTerrain > 0)
                    {
                        battleScreen.FieldEffects.GrassyTerrain -= 1;
                        if (battleScreen.FieldEffects.GrassyTerrain == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("The grass disappeared from the battlefield."));
                        }
                    }
                    if (battleScreen.FieldEffects.MistyTerrain > 0)
                    {
                        battleScreen.FieldEffects.MistyTerrain -= 1;
                        if (battleScreen.FieldEffects.MistyTerrain == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("The mist disappeared from the battlefield."));
                        }
                    }
                    if (battleScreen.FieldEffects.PsychicTerrain > 0)
                    {
                        battleScreen.FieldEffects.PsychicTerrain -= 1;
                        if (battleScreen.FieldEffects.PsychicTerrain == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("The weirdness disappeared from the battlefield."));
                        }
                    }
                    if (battleScreen.FieldEffects.WaterSport > 0)
                    {
                        battleScreen.FieldEffects.WaterSport -= 1;
                        if (battleScreen.FieldEffects.WaterSport == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("Water Sport's effect ended."));
                        }
                    }
                    if (battleScreen.FieldEffects.MudSport > 0)
                    {
                        battleScreen.FieldEffects.MudSport -= 1;
                        if (battleScreen.FieldEffects.MudSport == 0)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("Mud Sport's effect ended."));
                        }
                    }
                    if (battleScreen.SelfPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Flinch) == true)
                    {
                        battleScreen.SelfPokemon.RemoveVolatileStatus(Pokemon.VolatileStatus.Flinch);
                    }
                    if (battleScreen.OpponentPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Flinch) == true)
                    {
                        battleScreen.OpponentPokemon.RemoveVolatileStatus(Pokemon.VolatileStatus.Flinch);
                    }
                    if (battleScreen.FieldEffects.RoostUsed.Self == true)
                    {
                        battleScreen.SelfPokemon.Type1 = battleScreen.SelfPokemon.OriginalType1;
                        battleScreen.SelfPokemon.Type2 = battleScreen.SelfPokemon.OriginalType2;
                        battleScreen.FieldEffects.RoostUsed.Self = false;
                    }
                    if (battleScreen.FieldEffects.RoostUsed.Opponent == true)
                    {
                        battleScreen.OpponentPokemon.Type1 = battleScreen.OpponentPokemon.OriginalType1;
                        battleScreen.OpponentPokemon.Type2 = battleScreen.OpponentPokemon.OriginalType2;
                        battleScreen.FieldEffects.RoostUsed.Opponent = false;
                    }
                    LearnMovesQueryObject.ClearCache();
                    battleScreen.FieldEffects.MagicCoat.Self = 0;
                    battleScreen.FieldEffects.MagicCoat.Opponent = 0;
                    battleScreen.FieldEffects.DetectCounter.Self = 0;
                    battleScreen.FieldEffects.ProtectCounter.Self = 0;
                    battleScreen.FieldEffects.KingsShieldCounter.Self = 0;
                    if (battleScreen.FieldEffects.Endure.Self > 0)
                    {
                        battleScreen.FieldEffects.Endure.Self = 0;
                    }
                    battleScreen.FieldEffects.DetectCounter.Opponent = 0;
                    battleScreen.FieldEffects.ProtectCounter.Opponent = 0;
                    battleScreen.FieldEffects.KingsShieldCounter.Opponent = 0;
                    if (battleScreen.FieldEffects.Endure.Opponent > 0)
                    {
                        battleScreen.FieldEffects.Endure.Opponent = 0;
                    }
                    if (battleScreen.FieldEffects.ProtectMovesCount.Self > 0 && battleScreen.FieldEffects.LastMove.Self != null && battleScreen.FieldEffects.LastMove.Self.isProtectMove == false)
                    {
                        battleScreen.FieldEffects.ProtectMovesCount.Self = 0;
                    }
                    if (battleScreen.FieldEffects.ProtectMovesCount.Opponent > 0 && battleScreen.FieldEffects.LastMove.Opponent != null && battleScreen.FieldEffects.LastMove.Opponent.isProtectMove == false)
                    {
                        battleScreen.FieldEffects.ProtectMovesCount.Opponent = 0;
                    }
                }
                isAfterFaint = false;
                if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Fainted || battleScreen.SelfPokemon.HP <= 0)
                {
                    battleScreen.SelfPokemon.Status = Pokemon.StatusProblems.Fainted;
                    battleScreen.OwnFaint = true;
                    if (battleScreen.IsRemoteBattle && battleScreen.IsHost)
                    {
                        Logger.Debug("[Battle]: The host's pokemon faints");
                        battleScreen.BattleQuery.Add(new AfterFaintQueryObject(true));
                    }
                    SwitchOutOwn(battleScreen, -1, -1);
                }
                if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Fainted || battleScreen.OpponentPokemon.HP <= 0)
                {
                    battleScreen.OpponentPokemon.Status = Pokemon.StatusProblems.Fainted;
                    battleScreen.OppFaint = true;
                    if (battleScreen.IsRemoteBattle && battleScreen.IsHost)
                    {
                        Logger.Debug("[Battle]: The client's pokemon faints");
                        battleScreen.BattleQuery.Add(new AfterFaintQueryObject(false));
                    }
                    if (battleScreen.IsTrainerBattle == true)
                    {
                        if (battleScreen.Trainer.HasBattlePokemon() == true)
                        {
                            if (Core.Player.BattleStyle == 0 && battleScreen.IsPVPBattle == false)
                            {
                                battleScreen.ShiftCanContinue = false;
                            }
                            battleScreen.FieldEffects.DefeatedTrainerPokemon = true;
                        }
                    }
                    SwitchOutOpp(battleScreen, -1);
                    if (battleScreen.IsTrainerBattle && battleScreen.IsRemoteBattle == false)
                    {
                        _hasSwitchedInOpp = false;
                    }
                }
                ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
                ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
                cq2.PassThis = true;
                battleScreen.BattleQuery.AddRange(new QueryObject[] { cq1, cq2 });
                BattleScreen.FirstRound = false;
                StartRound(battleScreen);
                battleScreen.ClearMainMenuTime = true;
                battleScreen.ClearMoveMenuTime = true;
                break;
            case 1:
                EndTurnOwn(battleScreen);
                break;
            case 2:
                EndTurnOpp(battleScreen);
                break;
        }
    }

    private bool PlayerWonBattle(BattleScreen battleScreen)
    {
        if (battleScreen.BattleMode == BattleScreen.BattleModes.Safari)
        {
            return false;
        }
        if (battleScreen.IsTrainerBattle == true)
        {
            return battleScreen.TrainerHasFightablePokemon() == false;
        }
        else
        {
            return battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Fainted || battleScreen.OpponentPokemon.HP <= 0;
        }
    }

    private void EndTurnOwn(BattleScreen battleScreen)
    {
        battleScreen.FieldEffects.TurnCounts.Self += 1;
        battleScreen.FieldEffects.PokemonTurns.Self += 1;
        if (_hasSwitchedInOwn)
        {
            battleScreen.FieldEffects.PokemonTurns.Self = 0;
            _hasSwitchedInOwn = false;
        }
        battleScreen.FieldEffects.LockOn.Self = 0;
        battleScreen.FieldEffects.Pursuit.Self = false;
        if (battleScreen.FieldEffects.SleepTurns.Self > 0)
        {
            battleScreen.FieldEffects.SleepTurns.Self -= 1;
        }
        if (battleScreen.FieldEffects.Charge.Self > 0)
        {
            battleScreen.FieldEffects.Charge.Self -= 1;
        }
        if (battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.SelfPokemon.Item != null)
            {
                if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "mental herb")
                {
                    bool usedMentalHerb = false;
                    if (battleScreen.SelfPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == true)
                    {
                        battleScreen.SelfPokemon.RemoveVolatileStatus(Pokemon.VolatileStatus.Infatuation);
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " got healed from the infatuation" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    if (battleScreen.FieldEffects.Taunt.Self > 0)
                    {
                        battleScreen.FieldEffects.Taunt.Self = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " got healed from the Taunt" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    if (battleScreen.FieldEffects.Encore.Self > 0)
                    {
                        battleScreen.FieldEffects.Encore.Self = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " got healed from the Encore" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    if (battleScreen.FieldEffects.Torment.Self > 0)
                    {
                        battleScreen.FieldEffects.Torment.Self = 0;
                        battleScreen.FieldEffects.TormentMove.Self = null;
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " got healed from the Torment" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    foreach (Attack a in battleScreen.SelfPokemon.Attacks)
                    {
                        if (a.Disabled > 0)
                        {
                            a.Disabled = 0;
                            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s" + " " + a.Name + " " + "is no longer disabled" + Environment.NewLine + "due to Mental Herb!"));
                        }
                    }
                    if (usedMentalHerb == true)
                    {
                        battleScreen.SelfPokemon.Item = null;
                    }
                }
                if (battleScreen.SelfPokemon.Item != null && battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "white herb")
                {
                    bool hasNegativeStats = false;
                    if (battleScreen.SelfPokemon.StatAttack < 0) { battleScreen.SelfPokemon.StatAttack = 0; hasNegativeStats = true; }
                    if (battleScreen.SelfPokemon.StatDefense < 0) { battleScreen.SelfPokemon.StatDefense = 0; hasNegativeStats = true; }
                    if (battleScreen.SelfPokemon.StatSpAttack < 0) { battleScreen.SelfPokemon.StatSpAttack = 0; hasNegativeStats = true; }
                    if (battleScreen.SelfPokemon.StatSpDefense < 0) { battleScreen.SelfPokemon.StatSpDefense = 0; hasNegativeStats = true; }
                    if (battleScreen.SelfPokemon.StatSpeed < 0) { battleScreen.SelfPokemon.StatSpeed = 0; hasNegativeStats = true; }
                    if (battleScreen.SelfPokemon.Accuracy < 0) { battleScreen.SelfPokemon.Accuracy = 0; hasNegativeStats = true; }
                    if (battleScreen.SelfPokemon.Evasion < 0) { battleScreen.SelfPokemon.Evasion = 0; hasNegativeStats = true; }
                    if (hasNegativeStats == true)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " negative stats got healed due to White Herb!"));
                        battleScreen.SelfPokemon.Item = null;
                    }
                }
            }
        }
        battleScreen.FieldEffects.PokemonDamagedLastTurn.Self = battleScreen.FieldEffects.PokemonDamagedThisTurn.Self;
        battleScreen.FieldEffects.PokemonDamagedThisTurn.Self = false;
    }

    private void EndRoundOwn(BattleScreen battleScreen)
    {
        if (PlayerWonBattle(battleScreen) == true)
        {
            return;
        }
        TriggerItemEffect(battleScreen, true);
        TriggerItemEffect(battleScreen, false);
        ChangeCameraAngle(0, true, battleScreen);
        if (isAfterFaint == true)
        {
            return;
        }
        if (battleScreen.FieldEffects.Reflect.Self > 0)
        {
            battleScreen.FieldEffects.Reflect.Self -= 1;
            if (battleScreen.FieldEffects.Reflect.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Reflect effect faded."));
            }
        }
        if (battleScreen.FieldEffects.LightScreen.Self > 0)
        {
            battleScreen.FieldEffects.LightScreen.Self -= 1;
            if (battleScreen.FieldEffects.LightScreen.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Light Screen effect faded."));
            }
        }
        if (battleScreen.FieldEffects.Mist.Self > 0)
        {
            battleScreen.FieldEffects.Mist.Self -= 1;
            if (battleScreen.FieldEffects.Mist.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The mist on your side of the field faded!"));
            }
        }
        if (battleScreen.FieldEffects.Safeguard.Self > 0)
        {
            battleScreen.FieldEffects.Safeguard.Self -= 1;
            if (battleScreen.FieldEffects.Safeguard.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Safeguard effect wore off!"));
            }
        }
        if (battleScreen.FieldEffects.GuardSpec.Self > 0)
        {
            battleScreen.FieldEffects.GuardSpec.Self -= 1;
            if (battleScreen.FieldEffects.GuardSpec.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Guard Spec. wore off."));
            }
        }
        if (battleScreen.FieldEffects.TailWind.Self > 0)
        {
            battleScreen.FieldEffects.TailWind.Self -= 1;
            if (battleScreen.FieldEffects.TailWind.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Tail Wind effect faded."));
            }
        }
        if (battleScreen.FieldEffects.LuckyChant.Self > 0)
        {
            battleScreen.FieldEffects.LuckyChant.Self -= 1;
            if (battleScreen.FieldEffects.LuckyChant.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Lucky Chant effect faded."));
            }
        }
        if (battleScreen.FieldEffects.Wish.Self > 0)
        {
            battleScreen.FieldEffects.Wish.Self -= 1;
            if (battleScreen.FieldEffects.Wish.Self == 0)
            {
                if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
                {
                    if (battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP && battleScreen.SelfPokemon.HP > 0)
                    {
                        GainHP((int)(battleScreen.SelfPokemon.MaxHP / 2), true, true, battleScreen, "A wish came true!", "wish");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sandstorm)
        {
            if (battleScreen.SelfPokemon.Type1.Type != Element.Types.Ground && battleScreen.SelfPokemon.Type2.Type != Element.Types.Ground && battleScreen.SelfPokemon.Type1.Type != Element.Types.Steel && battleScreen.SelfPokemon.Type2.Type != Element.Types.Steel && battleScreen.SelfPokemon.Type1.Type != Element.Types.Rock && battleScreen.SelfPokemon.Type2.Type != Element.Types.Rock)
            {
                String[] sandAbilities = { "sand veil", "sand rush", "sand force", "overcoat", "magic guard", "cloud nine" };
                if (sandAbilities.Contains(battleScreen.SelfPokemon.Ability.Name.ToLower()) == false)
                {
                    if (battleScreen.SelfPokemon.HP > 0)
                    {
                        int sandHP = (int)(battleScreen.SelfPokemon.MaxHP / 16);
                        ReduceHP(sandHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " took damage from the sandstorm!", "sandstorm");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
        {
            if (battleScreen.SelfPokemon.Type1.Type != Element.Types.Ice && battleScreen.SelfPokemon.Type2.Type != Element.Types.Ice)
            {
                String[] hailAbilities = { "ice body", "snow cloak", "overcoat", "magic guard", "cloud nine" };
                if (hailAbilities.Contains(battleScreen.SelfPokemon.Ability.Name.ToLower()) == false)
                {
                    if (battleScreen.SelfPokemon.HP > 0)
                    {
                        int hailHP = (int)(battleScreen.SelfPokemon.MaxHP / 16);
                        ReduceHP(hailHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " took damage from the hailstorm!", "hail");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.GrassyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(true, battleScreen) == true)
        {
            if (battleScreen.SelfPokemon.HP > 0)
            {
                GainHP((int)(battleScreen.SelfPokemon.MaxHP / 16), true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " restored some HP due to the Grassy Terrain!", "grassyterrain");
            }
        }
        if (battleScreen.SelfPokemon.HP > 0)
        {
            int hpChange = 0;
            String hpMessage = String.Empty;
            switch (battleScreen.SelfPokemon.Ability.Name.ToLower())
            {
                case "dry skin":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny) { hpChange = -(int)(battleScreen.SelfPokemon.MaxHP / 8); hpMessage = "Dry Skin"; }
                    else if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain) { hpChange = (int)(battleScreen.SelfPokemon.MaxHP / 8); hpMessage = "Dry Skin"; }
                    break;
                case "solar power":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny) { hpChange = -(int)(battleScreen.SelfPokemon.MaxHP / 8); hpMessage = "Solar Power"; }
                    break;
                case "rain dish":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain) { hpChange = (int)(battleScreen.SelfPokemon.MaxHP / 16); hpMessage = "Rain Dish"; }
                    break;
                case "hydration":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain)
                    {
                        if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.BadPoison || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Poison || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Paralyzed || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Freeze || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Burn || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Sleep)
                        {
                            CureStatusProblem(true, true, battleScreen, "Hydration cured " + battleScreen.SelfPokemon.GetDisplayName() + "'s status problem.", "hydration");
                        }
                    }
                    break;
                case "ice body":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm) { hpChange = (int)(battleScreen.SelfPokemon.MaxHP / 16); hpMessage = "Ice Body"; }
                    break;
            }
            if (hpChange > 0)
            {
                if (battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP)
                {
                    GainHP(hpChange, true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " restored some HP due to " + hpMessage + ".", hpMessage.Replace(" ", String.Empty).ToLower());
                }
            }
            else if (hpChange < 0)
            {
                ReduceHP(hpChange, true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " lost some HP due to " + hpMessage + ".", hpMessage.Replace(" ", String.Empty).ToLower());
            }
        }
        if (battleScreen.FieldEffects.Ingrain.Self > 0 && battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP && battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
            {
                int healHP = (int)(battleScreen.SelfPokemon.MaxHP / 16);
                if (battleScreen.SelfPokemon.Item != null)
                {
                    if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        healHP = (int)(healHP * 1.3f);
                    }
                }
                GainHP(healHP, true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " gained health from the Ingrain.", "ingrain");
            }
        }
        if (battleScreen.FieldEffects.AquaRing.Self > 0 && battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP && battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
            {
                int healHP = (int)(battleScreen.SelfPokemon.MaxHP / 16);
                if (battleScreen.SelfPokemon.Item != null)
                {
                    if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        healHP = (int)(healHP * 1.3f);
                    }
                }
                GainHP(healHP, true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " gained health from the Aqua Ring.", "aquaring");
            }
        }
        if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "shed skin" && battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.BadPoison || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Poison || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Paralyzed || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Freeze || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Burn || battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Sleep)
            {
                if (Core.Random.Next(0, 100) < 33)
                {
                    battleScreen.BattleQuery.Add(battleScreen.FocusSelfPokemon());
                    CureStatusProblem(true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + "'s Shed Skin cured its status problem.", "shedskin");
                }
            }
        }
        if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "speed boost" && battleScreen.SelfPokemon.HP > 0)
        {
            RaiseStat(true, true, battleScreen, "Speed", 1, battleScreen.SelfPokemon.GetDisplayName() + "'s Speed Boost raised its speed.", "speedboost");
        }
        if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "truant")
        {
            if (battleScreen.FieldEffects.TruantRound.Self == 1)
            {
                battleScreen.FieldEffects.TruantRound.Self = 0;
            }
            else
            {
                battleScreen.FieldEffects.TruantRound.Self = 1;
            }
        }
        if (battleScreen.SelfPokemon.Item != null)
        {
            if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "black sludge" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
            {
                if (battleScreen.SelfPokemon.Type1.Type == Element.Types.Poison || battleScreen.SelfPokemon.Type2.Type == Element.Types.Poison)
                {
                    if (battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP && battleScreen.SelfPokemon.HP > 0)
                    {
                        GainHP((int)(battleScreen.SelfPokemon.MaxHP / 16), true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " gained HP from Black Sludge!", "blacksludge");
                    }
                }
                else
                {
                    if (battleScreen.SelfPokemon.HP > 0)
                    {
                        ReduceHP((int)(battleScreen.SelfPokemon.MaxHP / 8), true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " lost HP due to Black Sludge!", "blacksludge");
                    }
                }
            }
        }
        if (battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP && battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
            {
                if (battleScreen.SelfPokemon.Item != null)
                {
                    if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "leftovers" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        GainHP((int)(battleScreen.SelfPokemon.MaxHP / 16), true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " restored some HP from Leftovers!", "leftovers");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.LeechSeed.Opponent > 0)
        {
            if (battleScreen.OpponentPokemon.HP > 0 && battleScreen.SelfPokemon.HP > 0)
            {
                int loseHP = (int)Math.Ceiling((double)battleScreen.OpponentPokemon.MaxHP / 8);
                int currHP = battleScreen.OpponentPokemon.HP;
                if (loseHP > currHP) { loseHP = currHP; }
                int addHP = loseHP;
                if (battleScreen.SelfPokemon.Item != null)
                {
                    if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        addHP += (int)Math.Ceiling(addHP * (30.0 / 100));
                    }
                }
                ReduceHP(loseHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " lost HP due to Leech Seed!", "leechseed");
                if (battleScreen.FieldEffects.HealBlock.Self == 0)
                {
                    if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "liquid ooze" && battleScreen.FieldEffects.CanUseAbility(false, battleScreen) == true)
                    {
                        battleScreen.Battle.ReduceHP(addHP, true, true, battleScreen, "Liquid Ooze damaged " + battleScreen.SelfPokemon.GetDisplayName() + "!", "liquidooze");
                    }
                    else
                    {
                        GainHP(addHP, true, true, battleScreen, String.Empty, "leechseed");
                    }
                }
            }
        }
        if (battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "poison heal")
            {
                if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
                {
                    if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Poison)
                    {
                        GainHP((int)(battleScreen.SelfPokemon.MaxHP / 8), true, true, battleScreen, "Poison Heal healed " + battleScreen.SelfPokemon.GetDisplayName() + ".", "poison");
                    }
                    if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.BadPoison)
                    {
                        battleScreen.FieldEffects.PoisonCounter.Self += 1;
                        GainHP((int)(battleScreen.SelfPokemon.MaxHP / 8), true, true, battleScreen, "Poison Heal healed " + battleScreen.SelfPokemon.GetDisplayName() + ".", "poison");
                    }
                }
            }
            else
            {
                if (battleScreen.SelfPokemon.Ability.Name.ToLower() != "magic guard")
                {
                    if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Poison)
                    {
                        ChangeCameraAngle(1, true, battleScreen);
                        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                        {
                            AnimationQueryObject poisonAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, true);
                            poisonAnimation.AnimationPlaySound(@"Battle\Effects\Poisoned", 0, 0);
                            Entity bubbleEntity1 = poisonAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 0, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 1f, 1f);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 2, 1);
                            battleScreen.BattleQuery.Add(poisonAnimation);
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Poisoned", false));
                        }
                        ReduceHP((int)(battleScreen.SelfPokemon.MaxHP / 8), true, true, battleScreen, "The poison hurt " + battleScreen.SelfPokemon.GetDisplayName() + ".", "poison");
                    }
                    if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.BadPoison)
                    {
                        battleScreen.FieldEffects.PoisonCounter.Self += 1;
                        double multiplier = (battleScreen.FieldEffects.PoisonCounter.Self / 16.0);
                        ChangeCameraAngle(1, true, battleScreen);
                        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                        {
                            AnimationQueryObject poisonAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, true);
                            poisonAnimation.AnimationPlaySound(@"Battle\Effects\Poisoned", 0, 0);
                            Entity bubbleEntity1 = poisonAnimation.SpawnEntity(new Vector3(-0.25f, -0.25f, -0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 0, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 1f, 1f);
                            Entity bubbleEntity2 = poisonAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 1, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 2, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 2f, 1f);
                            Entity bubbleEntity3 = poisonAnimation.SpawnEntity(new Vector3(0.25f, -0.25f, 0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 2, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity2, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 3, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 3f, 1f);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity3, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 4, 1);
                            battleScreen.BattleQuery.Add(poisonAnimation);
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Poisoned", false));
                        }
                        ReduceHP((int)(battleScreen.SelfPokemon.MaxHP * multiplier), true, true, battleScreen, "The toxic hurt " + battleScreen.SelfPokemon.GetDisplayName() + ".", "badpoison");
                    }
                }
            }
        }
        if (battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Burn)
            {
                if (battleScreen.SelfPokemon.Ability.Name.ToLower() != "water veil" && battleScreen.SelfPokemon.Ability.Name.ToLower() != "magic guard")
                {
                    int reduceAmount = (int)(battleScreen.SelfPokemon.MaxHP / 16);
                    if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "heatproof")
                    {
                        reduceAmount = (int)(battleScreen.SelfPokemon.MaxHP / 32);
                    }
                    ChangeCameraAngle(1, true, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject burnAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, true);
                        burnAnimation.AnimationPlaySound(@"Battle\Effects\Burned", 0, 0);
                        Entity flameEntity = burnAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f, 0.5f, 0.5f), 1.0f);
                        burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 32, 32, 32), String.Empty), 0.75f, 0);
                        burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 64, 32, 32), String.Empty), 1.5f, 0);
                        burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 96, 32, 32), String.Empty), 2.25f, 0);
                        burnAnimation.AnimationChangeTexture(flameEntity, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 128, 32, 32), String.Empty), 3, 0);
                        battleScreen.BattleQuery.Add(burnAnimation);
                    }
                    else
                    {
                        battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Burned", false));
                    }
                    ReduceHP(reduceAmount, true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by the burn.", "burn");
                }
            }
        }
        if (battleScreen.FieldEffects.Nightmare.Self > 0)
        {
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Sleep && battleScreen.SelfPokemon.HP > 0)
            {
                ReduceHP((int)(battleScreen.SelfPokemon.MaxHP / 4), true, false, battleScreen, "The nightmare haunted " + battleScreen.SelfPokemon.GetDisplayName() + "!", "nightmare");
            }
            else
            {
                battleScreen.FieldEffects.Nightmare.Self = 0;
            }
        }
        if (battleScreen.FieldEffects.Curse.Self > 0)
        {
            if (battleScreen.SelfPokemon.HP > 0)
            {
                ReduceHP((int)(battleScreen.SelfPokemon.MaxHP / 4), true, false, battleScreen, "The curse haunted " + battleScreen.SelfPokemon.GetDisplayName() + "!", "curse");
            }
        }
        if (battleScreen.FieldEffects.WaterPledge.Self > 0)
        {
            battleScreen.FieldEffects.WaterPledge.Self -= 1;
            if (battleScreen.FieldEffects.WaterPledge.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The rainbow faded!"));
            }
        }
        if (battleScreen.FieldEffects.GrassPledge.Self > 0)
        {
            battleScreen.FieldEffects.GrassPledge.Self -= 1;
            if (battleScreen.FieldEffects.GrassPledge.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The swamp faded!"));
            }
        }
        if (battleScreen.FieldEffects.FirePledge.Self > 0)
        {
            battleScreen.FieldEffects.FirePledge.Self -= 1;
            if (battleScreen.FieldEffects.FirePledge.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The fiery sea faded!"));
            }
            else
            {
                if (battleScreen.SelfPokemon.HP > 0)
                {
                    ReduceHP((int)(battleScreen.SelfPokemon.MaxHP / 8), true, false, battleScreen, "The firey sea hurt " + battleScreen.SelfPokemon.GetDisplayName() + "!", "firepledge");
                }
            }
        }
        if (battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.Wrap.Self > 0)
            {
                battleScreen.FieldEffects.Wrap.Self -= 1;
                if (battleScreen.FieldEffects.Wrap.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Wrap!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, true, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject wrapAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, false);
                        wrapAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Wrap", 5.0f, 0);
                        Entity wrapEntity = wrapAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 0, 80, 40), String.Empty), new Vector3(1.0f, 0.5f, 1.0f), 1, 0, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 40, 80, 40), String.Empty), 0.75f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 80, 80, 40), String.Empty), 1.5f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 120, 80, 40), String.Empty), 2.25f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 160, 80, 40), String.Empty), 3f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 200, 80, 40), String.Empty), 3.75f, 0.75f);
                        wrapAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 5, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 5, 0);
                        wrapAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 7, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 7, 0);
                        wrapAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 9, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 9, 0);
                        wrapAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 11, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 11, 0);
                        wrapAnimation.AnimationFade(wrapEntity, true, 0.03f, 0.0f, 11, 0);
                        battleScreen.BattleQuery.Add(wrapAnimation);
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Wrap!", "wrap");
                }
            }
            if (battleScreen.FieldEffects.Whirlpool.Self > 0)
            {
                battleScreen.FieldEffects.Whirlpool.Self -= 1;
                if (battleScreen.FieldEffects.Whirlpool.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Whirlpool!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, true, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject whirlpoolAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, false, true);
                        whirlpoolAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Whirlpool", 0.0f, 0);
                        Entity whirlpoolEntity = whirlpoolAnimation.SpawnEntity(new Vector3(0, -0.3f, 0), TextureManager.GetTexture(@"Textures\Battle\Water\Whirlpool"), new Vector3(0.0f), 1.0f, 0.0f, 0.0f);
                        whirlpoolAnimation.AnimationRotate(whirlpoolEntity, false, (float)(MathHelper.Pi * 1.5), 0, 0, (float)(MathHelper.Pi * 1.5), 0, 0, 0, 0, false);
                        whirlpoolAnimation.AnimationRotate(whirlpoolEntity, false, 0, 0, 0.2f, 0, 0, 10.0f, 0.0f, 0.0f, true);
                        whirlpoolAnimation.AnimationScale(whirlpoolEntity, false, 1.0f, 1.0f, 1.0f, 0.025f, 0.0f, 0.0f);
                        whirlpoolAnimation.AnimationScale(whirlpoolEntity, true, 0.0f, 0.0f, 0.0f, 0.025f, 5.0f, 0.0f);
                        battleScreen.BattleQuery.Add(whirlpoolAnimation);
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Whirlpool!", "whirlpool");
                }
            }
            if (battleScreen.FieldEffects.SandTomb.Self > 0)
            {
                battleScreen.FieldEffects.SandTomb.Self -= 1;
                if (battleScreen.FieldEffects.SandTomb.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Sand Tomb!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Sand Tomb!", "sandtomb");
                }
            }
            if (battleScreen.FieldEffects.Bind.Self > 0)
            {
                battleScreen.FieldEffects.Bind.Self -= 1;
                if (battleScreen.FieldEffects.Bind.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Bind!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, true, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject bindAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, false);
                        bindAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Bind", 5.0f, 0);
                        Entity bindEntity = bindAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 0, 80, 40), String.Empty), new Vector3(1.0f, 0.5f, 1.0f), 1, 0, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 40, 80, 40), String.Empty), 0.75f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 80, 80, 40), String.Empty), 1.5f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 120, 80, 40), String.Empty), 2.25f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 160, 80, 40), String.Empty), 3f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 200, 80, 40), String.Empty), 3.75f, 0.75f);
                        bindAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 5, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 5, 0);
                        bindAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 7, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 7, 0);
                        bindAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 9, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 9, 0);
                        bindAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 11, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 11, 0);
                        bindAnimation.AnimationFade(bindEntity, true, 0.03f, 0.0f, 11, 0);
                        battleScreen.BattleQuery.Add(bindAnimation);
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Bind!", "bind");
                }
            }
            if (battleScreen.FieldEffects.Clamp.Self > 0)
            {
                battleScreen.FieldEffects.Clamp.Self -= 1;
                if (battleScreen.FieldEffects.Clamp.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Clamp!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, true, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject clampAnimation = new AnimationQueryObject(battleScreen.SelfPokemonNPC, true);
                        float offsetLeft = 0.35f;
                        float offsetRight = -0.35f;
                        clampAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Clamp", 0, 0);
                        Entity clampEntityLeft = clampAnimation.SpawnEntity(new Vector3(offsetLeft, -0.1f, offsetLeft), TextureManager.GetTexture(@"Textures\Battle\Water\Clamp_Left", new Rectangle(0, 0, 24, 64), String.Empty), new Vector3(0.28f, 0.75f, 0.28f), 0.75f);
                        Entity clampEntityRight = clampAnimation.SpawnEntity(new Vector3(offsetRight, -0.1f, offsetRight), TextureManager.GetTexture(@"Textures\Battle\Water\Clamp_Right", new Rectangle(0, 0, 24, 64), String.Empty), new Vector3(0.28f, 0.75f, 0.28f), 0.75f);
                        clampAnimation.AnimationMove(clampEntityLeft, false, -0.1f, -0.1f, -0.1f, 0.02f, false, false, 0, 0);
                        clampAnimation.AnimationMove(clampEntityRight, false, 0.1f, -0.1f, 0.1f, 0.02f, false, false, 0, 0);
                        clampAnimation.AnimationMove(clampEntityLeft, true, -0.35f, -0.1f, -0.35f, 0.02f, false, false, 2, 0);
                        clampAnimation.AnimationMove(clampEntityRight, true, 0.35f, -0.1f, 0.35f, 0.02f, false, false, 2, 0);
                        Entity spawnEntity = clampAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Tackle"), new Vector3(0.5f), 1.0f, 2.5f, 2);
                        clampAnimation.AnimationFade(spawnEntity, true, 1.0f, 0.0f, 4.5f, 0);
                        battleScreen.BattleQuery.Add(clampAnimation);
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Clamp!", "clamp");
                }
            }
            if (battleScreen.FieldEffects.FireSpin.Self > 0)
            {
                battleScreen.FieldEffects.FireSpin.Self -= 1;
                if (battleScreen.FieldEffects.FireSpin.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Fire Spin!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Fire Spin!", "firespin");
                }
            }
            if (battleScreen.FieldEffects.MagmaStorm.Self > 0)
            {
                battleScreen.FieldEffects.MagmaStorm.Self -= 1;
                if (battleScreen.FieldEffects.MagmaStorm.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Magma Storm!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Magma Storm!", "magmastorm");
                }
            }
            if (battleScreen.FieldEffects.Infestation.Self > 0)
            {
                battleScreen.FieldEffects.Infestation.Self -= 1;
                if (battleScreen.FieldEffects.Infestation.Self == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " was freed from Infestation!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.SelfPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " is hurt by Infestation!", "infestation");
                }
            }
        }
        if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "bad dreams" && battleScreen.SelfPokemon.HP > 0 && battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Sleep)
        {
            ReduceHP((int)(battleScreen.SelfPokemon.MaxHP / 8), true, false, battleScreen, "The bad dreams haunted " + battleScreen.SelfPokemon.GetDisplayName() + "!", "baddreams");
        }
        if (battleScreen.FieldEffects.Outrage.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Outrage.Self -= 1;
            if (battleScreen.FieldEffects.Outrage.Self == 0)
            {
                InflictConfusion(true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + "'s Outrage stopped.", "outrage");
            }
        }
        if (battleScreen.FieldEffects.PetalDance.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.PetalDance.Self -= 1;
            if (battleScreen.FieldEffects.PetalDance.Self == 0)
            {
                InflictConfusion(true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + "'s Petal Dance stopped.", "petaldance");
            }
        }
        if (battleScreen.FieldEffects.Thrash.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Thrash.Self -= 1;
            if (battleScreen.FieldEffects.Thrash.Self == 0)
            {
                InflictConfusion(true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + "'s Thrash stopped.", "thrash");
            }
        }
        if (battleScreen.FieldEffects.Uproar.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Uproar.Self -= 1;
            if (battleScreen.FieldEffects.Uproar.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s uproar stopped."));
            }
        }
        foreach (Attack a in battleScreen.SelfPokemon.Attacks)
        {
            if (a.Disabled > 0)
            {
                a.Disabled -= 1;
                if (a.Disabled == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s" + " " + a.Name + " " + "is no longer disabled."));
                }
            }
        }
        if (battleScreen.FieldEffects.Encore.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Encore.Self -= 1;
            if (battleScreen.FieldEffects.Encore.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s encore stopped."));
            }
        }
        if (battleScreen.FieldEffects.Taunt.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Taunt.Self -= 1;
            if (battleScreen.FieldEffects.Taunt.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Taunt effect wore off."));
            }
        }
        if (battleScreen.FieldEffects.MagnetRise.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.MagnetRise.Self -= 1;
            if (battleScreen.FieldEffects.MagnetRise.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Own Magnet Rise effect faded."));
            }
        }
        if (battleScreen.FieldEffects.HealBlock.Self > 0)
        {
            battleScreen.FieldEffects.HealBlock.Self -= 1;
            if (battleScreen.FieldEffects.HealBlock.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The effects of Heal Block faded."));
            }
        }
        if (battleScreen.FieldEffects.Embargo.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Embargo.Self -= 1;
            if (battleScreen.FieldEffects.Embargo.Self == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " is not under the Embargo effect anymore."));
            }
        }
        if (battleScreen.FieldEffects.Yawn.Self > 0 && battleScreen.SelfPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Yawn.Self -= 1;
            if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.None && battleScreen.FieldEffects.Yawn.Self == 0)
            {
                InflictSleep(true, false, battleScreen, -1, String.Empty, "yawn");
            }
        }
        String futureSightOwn = "Future Sight";
        if (battleScreen.FieldEffects.FutureSightID.Self == 1)
        {
            futureSightOwn = "Doom Desire";
        }
        if (battleScreen.FieldEffects.FutureSightTurns.Self > 0)
        {
            battleScreen.FieldEffects.FutureSightTurns.Self -= 1;
            if (battleScreen.FieldEffects.FutureSightTurns.Self == 0)
            {
                if (battleScreen.OpponentPokemon.HP > 0)
                {
                    ReduceHP(battleScreen.FieldEffects.FutureSightDamage.Self, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " took the " + futureSightOwn + " attack!", futureSightOwn.Replace(" ", String.Empty).ToLower());
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("The " + futureSightOwn + " failed!"));
                }
            }
        }
        if (battleScreen.FieldEffects.PerishSongCount.Self > 0)
        {
            battleScreen.FieldEffects.PerishSongCount.Self -= 1;
            if (battleScreen.SelfPokemon.HP > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s Perish Count is at " + battleScreen.FieldEffects.PerishSongCount.Self.ToString() + "!"));
                if (battleScreen.FieldEffects.PerishSongCount.Self == 0)
                {
                    ReduceHP(battleScreen.SelfPokemon.HP, true, false, battleScreen, String.Empty, "move:perishsong");
                    FaintPokemon(true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " fainted due to Perish Song!");
                }
            }
        }
        if (battleScreen.SelfPokemon.HP > 0 && battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.None)
        {
            if (battleScreen.SelfPokemon.Item != null)
            {
                if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "flame orb" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                {
                    InflictBurn(true, true, battleScreen, "Flame Orb inflicts a burn!", "flameorb");
                }
            }
        }
        if (battleScreen.SelfPokemon.HP > 0 && battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.None)
        {
            if (battleScreen.SelfPokemon.Item != null)
            {
                if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "toxic orb" && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                {
                    InflictPoison(true, true, battleScreen, true, "Toxic Orb inflicts a poisoning!", "toxicorb");
                }
            }
        }
        if (battleScreen.FieldEffects.CudChewIndex.Self != -1 && battleScreen.FieldEffects.CudChewBerry.Self != null)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + " regurgitated the " + battleScreen.FieldEffects.CudChewBerry.Self.Name + " Berry due to Cud Chew!"));
            String[] regularBerries = { "oran", "sitrus", "figy", "wiki", "mago", "aguav", "iapapa", "liechi", "ganlon", "salac", "petaya", "apicot", "lansat", "starf" };
            if (regularBerries.Contains(battleScreen.FieldEffects.CudChewBerry.Self.Name.ToLower()))
            {
                UseBerry(true, true, battleScreen.FieldEffects.CudChewBerry.Self, battleScreen, String.Empty, "ability:cudchew");
            }
            else
            {
                UseEffectBerry(true, true, battleScreen.FieldEffects.CudChewBerry.Self, battleScreen, String.Empty, "ability:cudchew");
            }
            battleScreen.FieldEffects.CudChewBerry.Self = null;
            battleScreen.FieldEffects.CudChewIndex.Self = -1;
        }
        if (battleScreen.SelfPokemon.HP > 0)
        {
            if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "moody")
            {
                List<int> cannotRaise = [];
                List<int> cannotLower = [];
                if (battleScreen.SelfPokemon.StatAttack == 6) { cannotRaise.Add(0); } else if (battleScreen.SelfPokemon.StatAttack == -6) { cannotLower.Add(0); }
                if (battleScreen.SelfPokemon.StatDefense == 6) { cannotRaise.Add(1); } else if (battleScreen.SelfPokemon.StatDefense == -6) { cannotLower.Add(1); }
                if (battleScreen.SelfPokemon.StatSpAttack == 6) { cannotRaise.Add(2); } else if (battleScreen.SelfPokemon.StatSpAttack == -6) { cannotLower.Add(2); }
                if (battleScreen.SelfPokemon.StatSpDefense == 6) { cannotRaise.Add(3); } else if (battleScreen.SelfPokemon.StatSpDefense == -6) { cannotLower.Add(3); }
                if (battleScreen.SelfPokemon.StatSpeed == 6) { cannotRaise.Add(4); } else if (battleScreen.SelfPokemon.StatSpeed == -6) { cannotLower.Add(4); }
                if (cannotRaise.Count < 5)
                {
                    int statToRaise = Core.Random.Next(0, 5);
                    while (cannotRaise.Contains(statToRaise) == true)
                    {
                        statToRaise = Core.Random.Next(0, 5);
                    }
                    switch (statToRaise)
                    {
                        case 0: RaiseStat(true, true, battleScreen, "Attack", 2, "Moody raised a stat.", "moody"); break;
                        case 1: RaiseStat(true, true, battleScreen, "Defense", 2, "Moody raised a stat.", "moody"); break;
                        case 2: RaiseStat(true, true, battleScreen, "Special Attack", 2, "Moody raised a stat.", "moody"); break;
                        case 3: RaiseStat(true, true, battleScreen, "Special Defense", 2, "Moody raised a stat.", "moody"); break;
                        case 4: RaiseStat(true, true, battleScreen, "Speed", 2, "Moody raised a stat.", "moody"); break;
                    }
                    if (cannotLower.Contains(statToRaise) == false)
                    {
                        cannotLower.Add(statToRaise);
                    }
                }
                if (cannotLower.Count < 5)
                {
                    int statToLower = Core.Random.Next(0, 5);
                    while (cannotLower.Contains(statToLower) == true)
                    {
                        statToLower = Core.Random.Next(0, 5);
                    }
                    switch (statToLower)
                    {
                        case 0: LowerStat(true, true, battleScreen, "Attack", 1, "Moody lowered a stat.", "moody"); break;
                        case 1: LowerStat(true, true, battleScreen, "Defense", 1, "Moody lowered a stat.", "moody"); break;
                        case 2: LowerStat(true, true, battleScreen, "Special Attack", 1, "Moody lowered a stat.", "moody"); break;
                        case 3: LowerStat(true, true, battleScreen, "Special Defense", 1, "Moody lowered a stat.", "moody"); break;
                        case 4: LowerStat(true, true, battleScreen, "Speed", 1, "Moody lowered a stat.", "moody"); break;
                    }
                }
            }
        }
    }

    private void EndTurnOpp(BattleScreen battleScreen)
    {
        battleScreen.FieldEffects.TurnCounts.Opponent += 1;
        battleScreen.FieldEffects.PokemonTurns.Opponent += 1;
        if (_hasSwitchedInOpp)
        {
            battleScreen.FieldEffects.PokemonTurns.Opponent = 0;
            _hasSwitchedInOpp = false;
        }
        battleScreen.FieldEffects.LockOn.Opponent = 0;
        battleScreen.FieldEffects.Pursuit.Opponent = false;
        if (battleScreen.FieldEffects.SleepTurns.Opponent > 0)
        {
            battleScreen.FieldEffects.SleepTurns.Opponent -= 1;
        }
        if (battleScreen.FieldEffects.Charge.Opponent > 0)
        {
            battleScreen.FieldEffects.Charge.Opponent -= 1;
        }
        if (battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.OpponentPokemon.Item != null)
            {
                if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "mental herb")
                {
                    bool usedMentalHerb = false;
                    if (battleScreen.OpponentPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == true)
                    {
                        battleScreen.OpponentPokemon.RemoveVolatileStatus(Pokemon.VolatileStatus.Infatuation);
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " got healed from the infatuation" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    if (battleScreen.FieldEffects.Taunt.Opponent > 0)
                    {
                        battleScreen.FieldEffects.Taunt.Opponent = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " got healed from the Taunt" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    if (battleScreen.FieldEffects.Encore.Opponent > 0)
                    {
                        battleScreen.FieldEffects.Encore.Opponent = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " got healed from the Encore" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    if (battleScreen.FieldEffects.Torment.Opponent > 0)
                    {
                        battleScreen.FieldEffects.Torment.Opponent = 0;
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " got healed from the Torment" + Environment.NewLine + "due to Mental Herb!"));
                        usedMentalHerb = true;
                    }
                    foreach (Attack a in battleScreen.OpponentPokemon.Attacks)
                    {
                        if (a.Disabled > 0)
                        {
                            a.Disabled = 0;
                            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s" + " " + a.Name + " " + "is no longer disabled" + Environment.NewLine + "due to Mental Herb!"));
                        }
                    }
                    if (usedMentalHerb == true)
                    {
                        battleScreen.OpponentPokemon.Item = null;
                    }
                }
                if (battleScreen.OpponentPokemon.Item != null && battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "white herb")
                {
                    bool hasNegativeStats = false;
                    if (battleScreen.OpponentPokemon.StatAttack < 0) { battleScreen.OpponentPokemon.StatAttack = 0; hasNegativeStats = true; }
                    if (battleScreen.OpponentPokemon.StatDefense < 0) { battleScreen.OpponentPokemon.StatDefense = 0; hasNegativeStats = true; }
                    if (battleScreen.OpponentPokemon.StatSpAttack < 0) { battleScreen.OpponentPokemon.StatSpAttack = 0; hasNegativeStats = true; }
                    if (battleScreen.OpponentPokemon.StatSpDefense < 0) { battleScreen.OpponentPokemon.StatSpDefense = 0; hasNegativeStats = true; }
                    if (battleScreen.OpponentPokemon.StatSpeed < 0) { battleScreen.OpponentPokemon.StatSpeed = 0; hasNegativeStats = true; }
                    if (battleScreen.OpponentPokemon.Accuracy < 0) { battleScreen.OpponentPokemon.Accuracy = 0; hasNegativeStats = true; }
                    if (battleScreen.OpponentPokemon.Evasion < 0) { battleScreen.OpponentPokemon.Evasion = 0; hasNegativeStats = true; }
                    if (hasNegativeStats == true)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " negative stats got healed" + Environment.NewLine + "due to White Herb!"));
                        battleScreen.OpponentPokemon.Item = null;
                    }
                }
            }
        }
        battleScreen.FieldEffects.PokemonDamagedLastTurn.Opponent = battleScreen.FieldEffects.PokemonDamagedThisTurn.Opponent;
        battleScreen.FieldEffects.PokemonDamagedThisTurn.Opponent = false;
    }

    private void EndRoundOpp(BattleScreen battleScreen)
    {
        ChangeCameraAngle(0, true, battleScreen);
        if (isAfterFaint == true)
        {
            return;
        }
        TriggerItemEffect(battleScreen, true);
        TriggerItemEffect(battleScreen, false);
        if (battleScreen.FieldEffects.Reflect.Opponent > 0)
        {
            battleScreen.FieldEffects.Reflect.Opponent -= 1;
            if (battleScreen.FieldEffects.Reflect.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Reflect effect faded."));
            }
        }
        if (battleScreen.FieldEffects.LightScreen.Opponent > 0)
        {
            battleScreen.FieldEffects.LightScreen.Opponent -= 1;
            if (battleScreen.FieldEffects.LightScreen.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Light Screen effect faded."));
            }
        }
        if (battleScreen.FieldEffects.Mist.Opponent > 0)
        {
            battleScreen.FieldEffects.Mist.Opponent -= 1;
            if (battleScreen.FieldEffects.Mist.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The mist on the opponent's side of the field faded!"));
            }
        }
        if (battleScreen.FieldEffects.Safeguard.Opponent > 0)
        {
            battleScreen.FieldEffects.Safeguard.Opponent -= 1;
            if (battleScreen.FieldEffects.Safeguard.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Safeguard effect wore off!"));
            }
        }
        if (battleScreen.FieldEffects.GuardSpec.Opponent > 0)
        {
            battleScreen.FieldEffects.GuardSpec.Opponent -= 1;
            if (battleScreen.FieldEffects.GuardSpec.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Guard Spec. wore off."));
            }
        }
        if (battleScreen.FieldEffects.TailWind.Opponent > 0)
        {
            battleScreen.FieldEffects.TailWind.Opponent -= 1;
            if (battleScreen.FieldEffects.TailWind.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Tail Wind effect faded."));
            }
        }
        if (battleScreen.FieldEffects.LuckyChant.Opponent > 0)
        {
            battleScreen.FieldEffects.LuckyChant.Opponent -= 1;
            if (battleScreen.FieldEffects.LuckyChant.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Lucky Chant effect faded."));
            }
        }
        if (battleScreen.FieldEffects.Wish.Opponent > 0)
        {
            battleScreen.FieldEffects.Wish.Opponent -= 1;
            if (battleScreen.FieldEffects.Wish.Opponent == 0)
            {
                if (battleScreen.FieldEffects.HealBlock.Self == 0)
                {
                    if (battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP && battleScreen.OpponentPokemon.HP > 0)
                    {
                        GainHP((int)(battleScreen.OpponentPokemon.MaxHP / 2), false, false, battleScreen, "A wish came true!", "wish");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sandstorm)
        {
            if (battleScreen.OpponentPokemon.Type1.Type != Element.Types.Ground && battleScreen.OpponentPokemon.Type2.Type != Element.Types.Ground && battleScreen.OpponentPokemon.Type1.Type != Element.Types.Steel && battleScreen.OpponentPokemon.Type2.Type != Element.Types.Steel && battleScreen.OpponentPokemon.Type1.Type != Element.Types.Rock && battleScreen.OpponentPokemon.Type2.Type != Element.Types.Rock)
            {
                String[] sandAbilities = { "sand veil", "sand rush", "sand force", "overcoat", "magic guard", "cloud nine" };
                if (sandAbilities.Contains(battleScreen.OpponentPokemon.Ability.Name.ToLower()) == false)
                {
                    if (battleScreen.OpponentPokemon.HP > 0)
                    {
                        int sandHP = (int)(battleScreen.OpponentPokemon.MaxHP / 16);
                        ReduceHP(sandHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " took damage from the sandstorm!", "sandstorm");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm)
        {
            if (battleScreen.OpponentPokemon.Type1.Type != Element.Types.Ice && battleScreen.OpponentPokemon.Type2.Type != Element.Types.Ice)
            {
                String[] hailAbilities = { "ice body", "snow cloak", "overcoat", "magic guard", "cloud nine" };
                if (hailAbilities.Contains(battleScreen.OpponentPokemon.Ability.Name.ToLower()) == false)
                {
                    if (battleScreen.OpponentPokemon.HP > 0)
                    {
                        int hailHP = (int)(battleScreen.OpponentPokemon.MaxHP / 16);
                        ReduceHP(hailHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " took damage from the hailstorm!", "sandstorm");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.GrassyTerrain > 0 && battleScreen.FieldEffects.IsGrounded(false, battleScreen) == true)
        {
            if (battleScreen.OpponentPokemon.HP > 0)
            {
                GainHP((int)(battleScreen.OpponentPokemon.MaxHP / 16), false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " restored some HP due to the Grassy Terrain!", "grassyterrain");
            }
        }
        if (battleScreen.OpponentPokemon.HP > 0)
        {
            int hpChange = 0;
            String hpMessage = String.Empty;
            switch (battleScreen.OpponentPokemon.Ability.Name.ToLower())
            {
                case "dry skin":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny) { hpChange = -(int)(battleScreen.OpponentPokemon.MaxHP / 8); hpMessage = "Dry Skin"; }
                    else if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain) { hpChange = (int)(battleScreen.OpponentPokemon.MaxHP / 8); hpMessage = "Dry Skin"; }
                    break;
                case "solar power":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Sunny) { hpChange = -(int)(battleScreen.OpponentPokemon.MaxHP / 8); hpMessage = "Solar Power"; }
                    break;
                case "rain dish":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain) { hpChange = (int)(battleScreen.OpponentPokemon.MaxHP / 16); hpMessage = "Rain Dish"; }
                    break;
                case "hydration":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Rain)
                    {
                        if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.BadPoison || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Poison || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Paralyzed || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Freeze || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Burn || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Sleep)
                        {
                            CureStatusProblem(false, false, battleScreen, "Hydration cured " + battleScreen.OpponentPokemon.GetDisplayName() + "'s status problem.", "hydration");
                        }
                    }
                    break;
                case "ice body":
                    if (battleScreen.FieldEffects.Weather == BattleWeather.WeatherTypes.Hailstorm) { hpChange = (int)(battleScreen.OpponentPokemon.MaxHP / 16); hpMessage = "Ice Body"; }
                    break;
            }
            if (hpChange > 0)
            {
                if (battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP)
                {
                    GainHP(hpChange, false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " restored some HP due to " + hpMessage + ".", hpMessage.Replace(" ", String.Empty).ToLower());
                }
            }
            else if (hpChange < 0)
            {
                ReduceHP(hpChange, false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " lost some HP due to " + hpMessage + ".", hpMessage.Replace(" ", String.Empty).ToLower());
            }
        }
        if (battleScreen.FieldEffects.Ingrain.Opponent > 0 && battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP && battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.HealBlock.Self == 0)
            {
                int healHP = (int)(battleScreen.OpponentPokemon.MaxHP / 16);
                if (battleScreen.OpponentPokemon.Item != null)
                {
                    if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        healHP = (int)(healHP * 1.3f);
                    }
                }
                GainHP(healHP, false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " gained health from the Ingrain.", "ingrain");
            }
        }
        if (battleScreen.FieldEffects.AquaRing.Opponent > 0 && battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP && battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.HealBlock.Self == 0)
            {
                int healHP = (int)(battleScreen.OpponentPokemon.MaxHP / 16);
                if (battleScreen.OpponentPokemon.Item != null)
                {
                    if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        healHP = (int)(healHP * 1.3f);
                    }
                }
                GainHP(healHP, false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " gained health from the Aqua Ring.", "aquaring");
            }
        }
        if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "shed skin" && battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.BadPoison || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Poison || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Paralyzed || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Freeze || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Burn || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Sleep)
            {
                if (Core.Random.Next(0, 100) < 33)
                {
                    battleScreen.BattleQuery.Add(battleScreen.FocusOpponentPokemon());
                    CureStatusProblem(false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + "'s Shed Skin cured its status problem.", "shedskin");
                }
            }
        }
        if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "speed boost" && battleScreen.OpponentPokemon.HP > 0)
        {
            RaiseStat(false, false, battleScreen, "Speed", 1, battleScreen.OpponentPokemon.GetDisplayName() + "'s Speed Boost raised its speed.", "speedboost");
        }
        if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "truant")
        {
            if (battleScreen.FieldEffects.TruantRound.Opponent == 1)
            {
                battleScreen.FieldEffects.TruantRound.Opponent = 0;
            }
            else
            {
                battleScreen.FieldEffects.TruantRound.Opponent = 1;
            }
        }
        if (battleScreen.OpponentPokemon.Item != null)
        {
            if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "black sludge" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
            {
                if (battleScreen.OpponentPokemon.Type1.Type == Element.Types.Poison || battleScreen.OpponentPokemon.Type2.Type == Element.Types.Poison)
                {
                    if (battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP && battleScreen.OpponentPokemon.HP > 0)
                    {
                        GainHP((int)(battleScreen.OpponentPokemon.MaxHP / 16), false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " gained HP from Black Sludge!", "blacksludge");
                    }
                }
                else
                {
                    if (battleScreen.OpponentPokemon.HP > 0)
                    {
                        ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP / 8), false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " lost HP due to Black Sludge!", "blacksludge");
                    }
                }
            }
        }
        if (battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP && battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.HealBlock.Self == 0)
            {
                if (battleScreen.OpponentPokemon.Item != null)
                {
                    if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "leftovers" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        GainHP((int)(battleScreen.OpponentPokemon.MaxHP / 16), false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " restored some HP from Leftovers!", "leftovers");
                    }
                }
            }
        }
        if (battleScreen.FieldEffects.LeechSeed.Self > 0)
        {
            if (battleScreen.SelfPokemon.HP > 0 && battleScreen.OpponentPokemon.HP > 0)
            {
                int loseHP = (int)Math.Ceiling((double)battleScreen.SelfPokemon.MaxHP / 8);
                int currHP = battleScreen.SelfPokemon.HP;
                if (loseHP > currHP) { loseHP = currHP; }
                int addHP = loseHP;
                if (battleScreen.OpponentPokemon.Item != null)
                {
                    if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "big root" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                    {
                        addHP += (int)Math.Ceiling(addHP * (30.0 / 100));
                    }
                }
                ReduceHP(loseHP, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " lost HP due to Leech Seed!", "leechseed");
                if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
                {
                    if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "liquid ooze" && battleScreen.FieldEffects.CanUseAbility(true, battleScreen) == true)
                    {
                        battleScreen.Battle.ReduceHP(addHP, false, false, battleScreen, "Liquid Ooze damaged " + battleScreen.OpponentPokemon.GetDisplayName() + "!", "liquidooze");
                    }
                    else
                    {
                        GainHP(addHP, false, false, battleScreen, String.Empty, "leechseed");
                    }
                }
            }
        }
        if (battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "poison heal")
            {
                if (battleScreen.FieldEffects.HealBlock.Self == 0)
                {
                    if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Poison)
                    {
                        GainHP((int)(battleScreen.OpponentPokemon.MaxHP / 8), false, false, battleScreen, "Poison Heal healed " + battleScreen.OpponentPokemon.GetDisplayName() + ".", "poison");
                    }
                    if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.BadPoison)
                    {
                        battleScreen.FieldEffects.PoisonCounter.Opponent += 1;
                        GainHP((int)(battleScreen.OpponentPokemon.MaxHP / 8), false, false, battleScreen, "Poison Heal healed " + battleScreen.OpponentPokemon.GetDisplayName() + ".", "poison");
                    }
                }
            }
            else
            {
                if (battleScreen.OpponentPokemon.Ability.Name.ToLower() != "magic guard")
                {
                    if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Poison)
                    {
                        ChangeCameraAngle(1, false, battleScreen);
                        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                        {
                            AnimationQueryObject poisonAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, false);
                            poisonAnimation.AnimationPlaySound(@"Battle\Effects\Poisoned", 0, 0);
                            Entity bubbleEntity1 = poisonAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 0, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 1f, 1f);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 2, 1);
                            battleScreen.BattleQuery.Add(poisonAnimation);
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Poisoned", false));
                        }
                        ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP / 8), false, false, battleScreen, "The poison hurt " + battleScreen.OpponentPokemon.GetDisplayName() + ".", "poison");
                    }
                    if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.BadPoison)
                    {
                        battleScreen.FieldEffects.PoisonCounter.Opponent += 1;
                        double multiplier = (battleScreen.FieldEffects.PoisonCounter.Opponent / 16.0);
                        ChangeCameraAngle(1, false, battleScreen);
                        if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                        {
                            AnimationQueryObject poisonAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, false);
                            poisonAnimation.AnimationPlaySound(@"Battle\Effects\Poisoned", 0, 0);
                            Entity bubbleEntity1 = poisonAnimation.SpawnEntity(new Vector3(-0.25f, -0.25f, -0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 0, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 1f, 1f);
                            Entity bubbleEntity2 = poisonAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 1, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity1, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 2, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity2, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 2f, 1f);
                            Entity bubbleEntity3 = poisonAnimation.SpawnEntity(new Vector3(0.25f, -0.25f, 0.25f), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f), 1, 2, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity2, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 3, 1);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity3, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 32, 32, 32), String.Empty), 3f, 1f);
                            poisonAnimation.AnimationChangeTexture(bubbleEntity3, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Poisoned", new Rectangle(0, 64, 32, 32), String.Empty), 4, 1);
                            battleScreen.BattleQuery.Add(poisonAnimation);
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Poisoned", false));
                        }
                        ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP * multiplier), false, false, battleScreen, "The toxic hurt " + battleScreen.OpponentPokemon.GetDisplayName() + ".", "badpoison");
                    }
                }
            }
        }
        if (battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Burn)
            {
                if (battleScreen.OpponentPokemon.Ability.Name.ToLower() != "water veil" && battleScreen.OpponentPokemon.Ability.Name.ToLower() != "magic guard")
                {
                    int reduceAmount = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "heatproof")
                    {
                        reduceAmount = (int)(battleScreen.OpponentPokemon.MaxHP / 16);
                    }
                    ChangeCameraAngle(1, false, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject burnAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, false);
                        burnAnimation.AnimationPlaySound(@"Battle\Effects\Burned", 0, 0);
                        Entity flameEntity = burnAnimation.SpawnEntity(new Vector3(0, -0.25f, 0), TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 0, 32, 32), String.Empty), new Vector3(0.5f, 0.5f, 0.5f), 1.0f);
                        burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 32, 32, 32), String.Empty), 0.75f, 0);
                        burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 64, 32, 32), String.Empty), 1.5f, 0);
                        burnAnimation.AnimationChangeTexture(flameEntity, false, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 96, 32, 32), String.Empty), 2.25f, 0);
                        burnAnimation.AnimationChangeTexture(flameEntity, true, TextureManager.GetTexture(@"Textures\Battle\StatusEffect\Burned", new Rectangle(0, 128, 32, 32), String.Empty), 3, 0);
                        battleScreen.BattleQuery.Add(burnAnimation);
                    }
                    else
                    {
                        battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\Effects\Burned", false));
                    }
                    ReduceHP(reduceAmount, false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by the burn.", "burn");
                }
            }
        }
        if (battleScreen.FieldEffects.Nightmare.Opponent > 0)
        {
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Sleep && battleScreen.OpponentPokemon.HP > 0)
            {
                ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP / 4), false, true, battleScreen, "The nightmare haunted " + battleScreen.OpponentPokemon.GetDisplayName() + "!", "nightmare");
            }
            else
            {
                battleScreen.FieldEffects.Nightmare.Self = 0;
            }
        }
        if (battleScreen.FieldEffects.Curse.Opponent > 0)
        {
            if (battleScreen.OpponentPokemon.HP > 0)
            {
                ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP / 4), false, true, battleScreen, "The curse haunted " + battleScreen.OpponentPokemon.GetDisplayName() + "!", "curse");
            }
        }
        if (battleScreen.FieldEffects.WaterPledge.Opponent > 0)
        {
            battleScreen.FieldEffects.WaterPledge.Opponent -= 1;
            if (battleScreen.FieldEffects.WaterPledge.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The rainbow faded!"));
            }
        }
        if (battleScreen.FieldEffects.GrassPledge.Opponent > 0)
        {
            battleScreen.FieldEffects.GrassPledge.Opponent -= 1;
            if (battleScreen.FieldEffects.GrassPledge.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The swamp faded!"));
            }
        }
        if (battleScreen.FieldEffects.FirePledge.Opponent > 0)
        {
            battleScreen.FieldEffects.FirePledge.Opponent -= 1;
            if (battleScreen.FieldEffects.FirePledge.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The fiery sea faded!"));
            }
            else
            {
                if (battleScreen.OpponentPokemon.HP > 0)
                {
                    ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP / 8), false, true, battleScreen, "The firey sea hurt " + battleScreen.OpponentPokemon.GetDisplayName() + "!", "firepledge");
                }
            }
        }
        if (battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.FieldEffects.Wrap.Opponent > 0)
            {
                battleScreen.FieldEffects.Wrap.Opponent -= 1;
                if (battleScreen.FieldEffects.Wrap.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Wrap!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, false, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject wrapAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, true);
                        wrapAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Wrap", 5.0f, 0);
                        Entity wrapEntity = wrapAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 0, 80, 40), String.Empty), new Vector3(1.0f, 0.5f, 1.0f), 1, 0, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 40, 80, 40), String.Empty), 0.75f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 80, 80, 40), String.Empty), 1.5f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 120, 80, 40), String.Empty), 2.25f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 160, 80, 40), String.Empty), 3f, 0.75f);
                        wrapAnimation.AnimationChangeTexture(wrapEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Wrap", new Rectangle(0, 200, 80, 40), String.Empty), 3.75f, 0.75f);
                        wrapAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 5, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 5, 0);
                        wrapAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 7, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 7, 0);
                        wrapAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 9, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 9, 0);
                        wrapAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 11, 0);
                        wrapAnimation.AnimationScale(wrapEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 11, 0);
                        wrapAnimation.AnimationFade(wrapEntity, true, 0.03f, 0.0f, 11, 0);
                        battleScreen.BattleQuery.Add(wrapAnimation);
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Wrap!", "wrap");
                }
            }
            if (battleScreen.FieldEffects.Whirlpool.Opponent > 0)
            {
                battleScreen.FieldEffects.Whirlpool.Opponent -= 1;
                if (battleScreen.FieldEffects.Whirlpool.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Whirlpool!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, false, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject whirlpoolAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, true, true);
                        whirlpoolAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Whirlpool", 0.0f, 0);
                        Entity whirlpoolEntity = whirlpoolAnimation.SpawnEntity(new Vector3(0, -0.3f, 0), TextureManager.GetTexture(@"Textures\Battle\Water\Whirlpool"), new Vector3(0.0f), 1.0f, 0.0f, 0.0f);
                        whirlpoolAnimation.AnimationRotate(whirlpoolEntity, false, (float)(MathHelper.Pi * 1.5), 0, 0, (float)(MathHelper.Pi * 1.5), 0, 0, 0, 0, false);
                        whirlpoolAnimation.AnimationRotate(whirlpoolEntity, false, 0, 0, 0.2f, 0, 0, 10.0f, 0.0f, 0.0f, true);
                        whirlpoolAnimation.AnimationScale(whirlpoolEntity, false, 1.0f, 1.0f, 1.0f, 0.025f, 0.0f, 0.0f);
                        whirlpoolAnimation.AnimationScale(whirlpoolEntity, true, 0.0f, 0.0f, 0.0f, 0.025f, 5.0f, 0.0f);
                        battleScreen.BattleQuery.Add(whirlpoolAnimation);
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Whirlpool!", "whirlpool");
                }
            }
            if (battleScreen.FieldEffects.SandTomb.Opponent > 0)
            {
                battleScreen.FieldEffects.SandTomb.Opponent -= 1;
                if (battleScreen.FieldEffects.SandTomb.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Sand Tomb!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Sand Tomb!", "sandtomb");
                }
            }
            if (battleScreen.FieldEffects.Bind.Opponent > 0)
            {
                battleScreen.FieldEffects.Bind.Opponent -= 1;
                if (battleScreen.FieldEffects.Bind.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Bind!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, false, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject bindAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, true);
                        bindAnimation.AnimationPlaySound(@"Battle\Attacks\Normal\Bind", 5.0f, 0);
                        Entity bindEntity = bindAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 0, 80, 40), String.Empty), new Vector3(1.0f, 0.5f, 1.0f), 1, 0, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 40, 80, 40), String.Empty), 0.75f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 80, 80, 40), String.Empty), 1.5f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 120, 80, 40), String.Empty), 2.25f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 160, 80, 40), String.Empty), 3f, 0.75f);
                        bindAnimation.AnimationChangeTexture(bindEntity, false, TextureManager.GetTexture(@"Textures\Battle\Normal\Bind", new Rectangle(0, 200, 80, 40), String.Empty), 3.75f, 0.75f);
                        bindAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 5, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 5, 0);
                        bindAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 7, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 7, 0);
                        bindAnimation.AnimationScale(null, false, 0.75f, 1.0f, 0.75f, 0.02f, 9, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 0.75f, 0.5f, 0.75f, 0.02f, 9, 0);
                        bindAnimation.AnimationScale(null, false, 1.0f, 1.0f, 1.0f, 0.04f, 11, 0);
                        bindAnimation.AnimationScale(bindEntity, false, 1.0f, 0.5f, 1.0f, 0.04f, 11, 0);
                        bindAnimation.AnimationFade(bindEntity, true, 0.03f, 0.0f, 11, 0);
                        battleScreen.BattleQuery.Add(bindAnimation);
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Bind!", "bind");
                }
            }
            if (battleScreen.FieldEffects.Clamp.Opponent > 0)
            {
                battleScreen.FieldEffects.Clamp.Opponent -= 1;
                if (battleScreen.FieldEffects.Clamp.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Clamp!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ChangeCameraAngle(1, false, battleScreen);
                    if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                    {
                        AnimationQueryObject clampAnimation = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, false);
                        float offsetLeft = -0.35f;
                        float offsetRight = 0.35f;
                        clampAnimation.AnimationPlaySound(@"Battle\Attacks\Water\Clamp", 0, 0);
                        Entity clampEntityLeft = clampAnimation.SpawnEntity(new Vector3(offsetLeft, -0.1f, offsetLeft), TextureManager.GetTexture(@"Textures\Battle\Water\Clamp_Left", new Rectangle(0, 0, 24, 64), String.Empty), new Vector3(0.28f, 0.75f, 0.28f), 0.75f);
                        Entity clampEntityRight = clampAnimation.SpawnEntity(new Vector3(offsetRight, -0.1f, offsetRight), TextureManager.GetTexture(@"Textures\Battle\Water\Clamp_Right", new Rectangle(0, 0, 24, 64), String.Empty), new Vector3(0.28f, 0.75f, 0.28f), 0.75f);
                        clampAnimation.AnimationMove(clampEntityLeft, false, -0.1f, -0.1f, -0.1f, 0.02f, false, false, 0, 0);
                        clampAnimation.AnimationMove(clampEntityRight, false, 0.1f, -0.1f, 0.1f, 0.02f, false, false, 0, 0);
                        clampAnimation.AnimationMove(clampEntityLeft, true, -0.35f, -0.1f, -0.35f, 0.02f, false, false, 2, 0);
                        clampAnimation.AnimationMove(clampEntityRight, true, 0.35f, -0.1f, 0.35f, 0.02f, false, false, 2, 0);
                        Entity spawnEntity = clampAnimation.SpawnEntity(new Vector3(0, -0.2f, 0), TextureManager.GetTexture(@"Textures\Battle\Normal\Tackle"), new Vector3(0.5f), 1.0f, 2.5f, 2);
                        clampAnimation.AnimationFade(spawnEntity, true, 1.0f, 0.0f, 4.5f, 0);
                        battleScreen.BattleQuery.Add(clampAnimation);
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Clamp!", "clamp");
                }
            }
            if (battleScreen.FieldEffects.FireSpin.Opponent > 0)
            {
                battleScreen.FieldEffects.FireSpin.Opponent -= 1;
                if (battleScreen.FieldEffects.FireSpin.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Fire Spin!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Fire Spin!", "firespin");
                }
            }
            if (battleScreen.FieldEffects.MagmaStorm.Opponent > 0)
            {
                battleScreen.FieldEffects.MagmaStorm.Opponent -= 1;
                if (battleScreen.FieldEffects.MagmaStorm.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Magma Storm!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Magma Storm!", "magmastorm");
                }
            }
            if (battleScreen.FieldEffects.Infestation.Opponent > 0)
            {
                battleScreen.FieldEffects.Infestation.Opponent -= 1;
                if (battleScreen.FieldEffects.Infestation.Opponent == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " was freed from Infestation!"));
                }
                else
                {
                    int multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 8);
                    if (battleScreen.SelfPokemon.Item != null && battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                    {
                        if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "binding band") { multiHP = (int)(battleScreen.OpponentPokemon.MaxHP / 6); }
                    }
                    ReduceHP(multiHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " is hurt by Infestation!", "infestation");
                }
            }
        }
        if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "bad dreams" && battleScreen.OpponentPokemon.HP > 0 && battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Sleep)
        {
            ReduceHP((int)(battleScreen.OpponentPokemon.MaxHP / 8), false, true, battleScreen, "The bad dreams haunted " + battleScreen.OpponentPokemon.GetDisplayName() + "!", "baddreams");
        }
        if (battleScreen.FieldEffects.Outrage.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Outrage.Opponent -= 1;
            if (battleScreen.FieldEffects.Outrage.Opponent == 0)
            {
                InflictConfusion(false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + "'s Outrage stopped.", "outrage");
            }
        }
        if (battleScreen.FieldEffects.PetalDance.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.PetalDance.Opponent -= 1;
            if (battleScreen.FieldEffects.PetalDance.Opponent == 0)
            {
                InflictConfusion(false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + "'s Petal Dance stopped.", "petaldance");
            }
        }
        if (battleScreen.FieldEffects.Thrash.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Thrash.Opponent -= 1;
            if (battleScreen.FieldEffects.Thrash.Opponent == 0)
            {
                InflictConfusion(false, false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + "'s Thrash stopped.", "thrash");
            }
        }
        if (battleScreen.FieldEffects.Uproar.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Uproar.Opponent -= 1;
            if (battleScreen.FieldEffects.Uproar.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s uproar stopped."));
            }
        }
        foreach (Attack a in battleScreen.OpponentPokemon.Attacks)
        {
            if (a.Disabled > 0)
            {
                a.Disabled -= 1;
                if (a.Disabled == 0)
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s" + " " + a.Name + " " + "is no longer disabled."));
                }
            }
        }
        if (battleScreen.FieldEffects.Encore.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Encore.Opponent -= 1;
            if (battleScreen.FieldEffects.Encore.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s encore stopped."));
            }
        }
        if (battleScreen.FieldEffects.Taunt.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Taunt.Opponent -= 1;
            if (battleScreen.FieldEffects.Taunt.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Taunt effect wore off."));
            }
        }
        if (battleScreen.FieldEffects.MagnetRise.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.MagnetRise.Opponent -= 1;
            if (battleScreen.FieldEffects.MagnetRise.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("Opponent's Magnet Rise effect faded."));
            }
        }
        if (battleScreen.FieldEffects.HealBlock.Opponent > 0)
        {
            battleScreen.FieldEffects.HealBlock.Opponent -= 1;
            if (battleScreen.FieldEffects.HealBlock.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject("The effects of the opponent's Heal Block faded."));
            }
        }
        if (battleScreen.FieldEffects.Embargo.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Embargo.Opponent -= 1;
            if (battleScreen.FieldEffects.Embargo.Opponent == 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " is not under the Embargo effect anymore."));
            }
        }
        if (battleScreen.FieldEffects.Yawn.Opponent > 0 && battleScreen.OpponentPokemon.HP > 0)
        {
            battleScreen.FieldEffects.Yawn.Opponent -= 1;
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.None && battleScreen.FieldEffects.Yawn.Opponent == 0)
            {
                InflictSleep(false, true, battleScreen, -1, String.Empty, "yawn");
            }
        }
        String futureSightOpp = "Future Sight";
        if (battleScreen.FieldEffects.FutureSightID.Opponent == 1)
        {
            futureSightOpp = "Doom Desire";
        }
        if (battleScreen.FieldEffects.FutureSightTurns.Opponent > 0)
        {
            battleScreen.FieldEffects.FutureSightTurns.Opponent -= 1;
            if (battleScreen.FieldEffects.FutureSightTurns.Opponent == 0)
            {
                if (battleScreen.SelfPokemon.HP > 0)
                {
                    ReduceHP(battleScreen.FieldEffects.FutureSightDamage.Opponent, true, false, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + " took the " + futureSightOpp + " attack!", futureSightOpp.Replace(" ", String.Empty).ToLower());
                }
                else
                {
                    battleScreen.BattleQuery.Add(new TextQueryObject("The " + futureSightOpp + " failed!"));
                }
            }
        }
        if (battleScreen.FieldEffects.PerishSongCount.Opponent > 0)
        {
            battleScreen.FieldEffects.PerishSongCount.Opponent -= 1;
            if (battleScreen.OpponentPokemon.HP > 0)
            {
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s Perish Count is at " + battleScreen.FieldEffects.PerishSongCount.Opponent.ToString() + "!"));
                if (battleScreen.FieldEffects.PerishSongCount.Opponent == 0)
                {
                    ReduceHP(battleScreen.OpponentPokemon.HP, false, true, battleScreen, String.Empty, "move:perishsong");
                    FaintPokemon(false, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + " fainted due to Perish Song!");
                }
            }
        }
        if (battleScreen.OpponentPokemon.HP > 0 && battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.Burn)
        {
            if (battleScreen.OpponentPokemon.Item != null)
            {
                if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "flame orb" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                {
                    InflictBurn(false, false, battleScreen, "Flame Orb inflicts a burn!", "flameorb");
                }
            }
        }
        if (battleScreen.OpponentPokemon.HP > 0 && battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.Poison && battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.BadPoison)
        {
            if (battleScreen.OpponentPokemon.Item != null)
            {
                if (battleScreen.OpponentPokemon.Item.OriginalName.ToLower() == "toxic orb" && battleScreen.FieldEffects.CanUseItem(false) == true && battleScreen.FieldEffects.CanUseOwnItem(false, battleScreen) == true)
                {
                    InflictPoison(false, false, battleScreen, true, "Toxic Orb inflicts a poisoning!", "toxicorb");
                }
            }
        }
        if (battleScreen.FieldEffects.CudChewIndex.Opponent != -1 && battleScreen.FieldEffects.CudChewBerry.Opponent != null)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + " regurgitated the " + battleScreen.FieldEffects.CudChewBerry.Opponent.Name + " Berry due to Cud Chew!"));
            String[] regularBerries = { "oran", "sitrus", "figy", "wiki", "mago", "aguav", "iapapa", "liechi", "ganlon", "salac", "petaya", "apicot", "lansat", "starf" };
            if (regularBerries.Contains(battleScreen.FieldEffects.CudChewBerry.Opponent.Name.ToLower()))
            {
                UseBerry(false, false, battleScreen.FieldEffects.CudChewBerry.Opponent, battleScreen, String.Empty, "ability:cudchew");
            }
            else
            {
                UseEffectBerry(false, false, battleScreen.FieldEffects.CudChewBerry.Self, battleScreen, String.Empty, "ability:cudchew");
            }
            battleScreen.FieldEffects.CudChewBerry.Opponent = null;
            battleScreen.FieldEffects.CudChewIndex.Opponent = -1;
        }
        if (battleScreen.OpponentPokemon.HP > 0)
        {
            if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "moody")
            {
                List<int> cannotRaise = [];
                List<int> cannotLower = [];
                if (battleScreen.OpponentPokemon.StatAttack == 6) { cannotRaise.Add(0); } else if (battleScreen.OpponentPokemon.StatAttack == -6) { cannotLower.Add(0); }
                if (battleScreen.OpponentPokemon.StatDefense == 6) { cannotRaise.Add(1); } else if (battleScreen.OpponentPokemon.StatDefense == -6) { cannotLower.Add(1); }
                if (battleScreen.OpponentPokemon.StatSpAttack == 6) { cannotRaise.Add(2); } else if (battleScreen.OpponentPokemon.StatSpAttack == -6) { cannotLower.Add(2); }
                if (battleScreen.OpponentPokemon.StatSpDefense == 6) { cannotRaise.Add(3); } else if (battleScreen.OpponentPokemon.StatSpDefense == -6) { cannotLower.Add(3); }
                if (battleScreen.OpponentPokemon.StatSpeed == 6) { cannotRaise.Add(4); } else if (battleScreen.OpponentPokemon.StatSpeed == -6) { cannotLower.Add(4); }
                if (cannotRaise.Count < 5)
                {
                    int statToRaise = Core.Random.Next(0, 5);
                    while (cannotRaise.Contains(statToRaise) == true)
                    {
                        statToRaise = Core.Random.Next(0, 5);
                    }
                    switch (statToRaise)
                    {
                        case 0: RaiseStat(false, false, battleScreen, "Attack", 2, "Moody raised a stat.", "moody"); break;
                        case 1: RaiseStat(false, false, battleScreen, "Defense", 2, "Moody raised a stat.", "moody"); break;
                        case 2: RaiseStat(false, false, battleScreen, "Special Attack", 2, "Moody raised a stat.", "moody"); break;
                        case 3: RaiseStat(false, false, battleScreen, "Special Defense", 2, "Moody raised a stat.", "moody"); break;
                        case 4: RaiseStat(false, false, battleScreen, "Speed", 2, "Moody raised a stat.", "moody"); break;
                    }
                    if (cannotLower.Contains(statToRaise) == false)
                    {
                        cannotLower.Add(statToRaise);
                    }
                }
                if (cannotLower.Count < 5)
                {
                    int statToLower = Core.Random.Next(0, 5);
                    while (cannotLower.Contains(statToLower) == true)
                    {
                        statToLower = Core.Random.Next(0, 5);
                    }
                    switch (statToLower)
                    {
                        case 0: LowerStat(false, false, battleScreen, "Attack", 1, "Moody lowered a stat.", "moody"); break;
                        case 1: LowerStat(false, false, battleScreen, "Defense", 1, "Moody lowered a stat.", "moody"); break;
                        case 2: LowerStat(false, false, battleScreen, "Special Attack", 1, "Moody lowered a stat.", "moody"); break;
                        case 3: LowerStat(false, false, battleScreen, "Special Defense", 1, "Moody lowered a stat.", "moody"); break;
                        case 4: LowerStat(false, false, battleScreen, "Speed", 1, "Moody lowered a stat.", "moody"); break;
                    }
                }
            }
        }
    }

    public void SwitchOutOwn(BattleScreen battleScreen, int switchInIndex, int insertIndex, String message = "", bool hasSwitched = false)
    {
        if (battleScreen.FieldEffects.ConfusionTurns.Self > 0)
        {
            battleScreen.FieldEffects.TempConfusionTurns.Self = battleScreen.FieldEffects.ConfusionTurns.Self;
        }
        if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "natural cure")
        {
            ChangeCameraAngle(1, true, battleScreen);
            if (battleScreen.SelfPokemon.Status != Pokemon.StatusProblems.Fainted && battleScreen.SelfPokemon.Status != Pokemon.StatusProblems.None)
            {
                battleScreen.SelfPokemon.Status = Pokemon.StatusProblems.None;
                battleScreen.AddToQuery(insertIndex, new TextQueryObject(battleScreen.SelfPokemon.GetDisplayName() + "'s status problem got healed by Natural Cure"));
            }
        }
        if (battleScreen.SelfPokemon.Ability.Name.ToLower() == "regenerator")
        {
            ChangeCameraAngle(1, true, battleScreen);
            if ((battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Fainted || battleScreen.SelfPokemon.HP == 0) == false)
            {
                int restoreHP = (int)(battleScreen.SelfPokemon.MaxHP / 3);
                if (restoreHP > 0 && battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP && battleScreen.SelfPokemon.HP > 0)
                {
                    battleScreen.Battle.GainHP(restoreHP, true, true, battleScreen, battleScreen.SelfPokemon.GetDisplayName() + "'s HP was restored!", "ability:regenerator");
                }
            }
        }
        if (battleScreen.FieldEffects.UsedBatonPass.Self == true)
        {
            ChangeCameraAngle(1, true, battleScreen);
            battleScreen.FieldEffects.BatonPassStats.Self = [];
            battleScreen.FieldEffects.BatonPassStats.Self.AddRange(new int[] { battleScreen.SelfPokemon.StatAttack, battleScreen.SelfPokemon.StatDefense, battleScreen.SelfPokemon.StatSpAttack, battleScreen.SelfPokemon.StatSpDefense, battleScreen.SelfPokemon.StatSpeed, battleScreen.SelfPokemon.Evasion, battleScreen.SelfPokemon.Accuracy });
            battleScreen.FieldEffects.BatonPassConfusion.Self = battleScreen.SelfPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true;
        }
        battleScreen.SelfPokemon.ResetTemp();
        battleScreen.SelfPokemon.ClearAllVolatiles();
        battleScreen.FieldEffects.SleepTurns.Self = 0;
        battleScreen.FieldEffects.TruantRound.Self = 0;
        battleScreen.FieldEffects.Taunt.Self = 0;
        battleScreen.FieldEffects.Smacked.Self = 0;
        battleScreen.FieldEffects.RageCounter.Self = 0;
        battleScreen.FieldEffects.Uproar.Self = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.FocusEnergy.Self = 0; }
        battleScreen.FieldEffects.Endure.Self = 0;
        battleScreen.FieldEffects.ProtectCounter.Self = 0;
        battleScreen.FieldEffects.DetectCounter.Self = 0;
        battleScreen.FieldEffects.KingsShieldCounter.Self = 0;
        battleScreen.FieldEffects.ProtectMovesCount.Self = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.Ingrain.Self = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.Substitute.Self = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.MagnetRise.Self = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.AquaRing.Self = 0; }
        battleScreen.FieldEffects.PoisonCounter.Self = 0;
        battleScreen.FieldEffects.Nightmare.Self = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.Curse.Self = 0; }
        battleScreen.FieldEffects.Outrage.Self = 0;
        battleScreen.FieldEffects.Thrash.Self = 0;
        battleScreen.FieldEffects.PetalDance.Self = 0;
        battleScreen.FieldEffects.Encore.Self = 0;
        battleScreen.FieldEffects.EncoreMove.Self = null;
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.Embargo.Self = 0; }
        battleScreen.FieldEffects.Yawn.Self = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.PerishSongCount.Self = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.ConfusionTurns.Self = 0; }
        battleScreen.FieldEffects.Torment.Self = 0;
        battleScreen.FieldEffects.TormentMove.Self = null;
        battleScreen.FieldEffects.ChoiceMove.Self = null;
        battleScreen.FieldEffects.Recharge.Self = 0;
        battleScreen.FieldEffects.RolloutCounter.Self = 0;
        battleScreen.FieldEffects.IceBallCounter.Self = 0;
        battleScreen.FieldEffects.DefenseCurl.Self = 0;
        battleScreen.FieldEffects.Charge.Self = 0;
        battleScreen.FieldEffects.SolarBeam.Self = 0;
        battleScreen.FieldEffects.SolarBlade.Self = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.LeechSeed.Self = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Self == false) { battleScreen.FieldEffects.LockOn.Self = 0; }
        battleScreen.FieldEffects.LansatBerry.Self = 0;
        battleScreen.FieldEffects.CustapBerry.Self = 0;
        battleScreen.FieldEffects.TrappedCounter.Self = 0;
        battleScreen.FieldEffects.FuryCutter.Self = 0;
        battleScreen.FieldEffects.EchoedVoice.Self = 0;
        battleScreen.FieldEffects.PokemonTurns.Self = 0;
        battleScreen.FieldEffects.StockpileCount.Self = 0;
        battleScreen.FieldEffects.DestinyBond.Self = false;
        battleScreen.FieldEffects.GastroAcid.Self = false;
        battleScreen.FieldEffects.TarShot.Self = false;
        battleScreen.FieldEffects.Foresight.Self = 0;
        battleScreen.FieldEffects.OdorSleuth.Self = 0;
        battleScreen.FieldEffects.MiracleEye.Self = 0;
        battleScreen.FieldEffects.FlyCounter.Self = 0;
        battleScreen.FieldEffects.DigCounter.Self = 0;
        battleScreen.FieldEffects.BounceCounter.Self = 0;
        battleScreen.FieldEffects.DiveCounter.Self = 0;
        battleScreen.FieldEffects.ShadowForceCounter.Self = 0;
        battleScreen.FieldEffects.PhantomForceCounter.Self = 0;
        battleScreen.FieldEffects.SkyDropCounter.Self = 0;
        battleScreen.FieldEffects.GeomancyCounter.Self = 0;
        battleScreen.FieldEffects.SkyAttackCounter.Self = 0;
        battleScreen.FieldEffects.RazorWindCounter.Self = 0;
        battleScreen.FieldEffects.SkullBashCounter.Self = 0;
        battleScreen.FieldEffects.Wrap.Self = 0;
        battleScreen.FieldEffects.Whirlpool.Self = 0;
        battleScreen.FieldEffects.Bind.Self = 0;
        battleScreen.FieldEffects.Clamp.Self = 0;
        battleScreen.FieldEffects.FireSpin.Self = 0;
        battleScreen.FieldEffects.MagmaStorm.Self = 0;
        battleScreen.FieldEffects.SandTomb.Self = 0;
        battleScreen.FieldEffects.Infestation.Self = 0;
        battleScreen.FieldEffects.BideCounter.Self = 0;
        battleScreen.FieldEffects.BideDamage.Self = 0;
        battleScreen.FieldEffects.RoostUsed.Self = false;
        battleScreen.FieldEffects.TrappedCounter.Opponent = 0;
        battleScreen.FieldEffects.Wrap.Opponent = 0;
        battleScreen.FieldEffects.Whirlpool.Opponent = 0;
        battleScreen.FieldEffects.Bind.Opponent = 0;
        battleScreen.FieldEffects.Clamp.Opponent = 0;
        battleScreen.FieldEffects.FireSpin.Opponent = 0;
        battleScreen.FieldEffects.MagmaStorm.Opponent = 0;
        battleScreen.FieldEffects.SandTomb.Opponent = 0;
        battleScreen.FieldEffects.Infestation.Opponent = 0;
        if (battleScreen.OpponentPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation))
        {
            battleScreen.OpponentPokemon.RemoveVolatileStatus(Pokemon.VolatileStatus.Infatuation);
        }
        battleScreen.SelfPokemon.Ability.SwitchOut(battleScreen.SelfPokemon);
        if (Core.Player.ShowBattleAnimations == 0 || battleScreen.IsPVPBattle == true)
        {
            battleScreen.AddToQuery(insertIndex, new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.SelfPokemon, 2, -1, -1, -1, -1));
        }
        if (Core.Player.CountFightablePokemon > 0)
        {
            if (battleScreen.OwnFaint)
            {
                // Next pokemon sent by the player is decided via menu.
            }
            else
            {
                SwitchInOwn(battleScreen, switchInIndex, false, insertIndex, message, hasSwitched);
            }
        }
        else
        {
            if (battleScreen.IsTrainerBattle == true)
            {
                EndBattle(EndBattleReasons.LoseTrainer, battleScreen, false);
                if (battleScreen.IsRemoteBattle == true)
                {
                    EndBattle(EndBattleReasons.LoseTrainer, battleScreen, true);
                }
            }
            else
            {
                EndBattle(EndBattleReasons.LoseWild, battleScreen, false);
            }
        }
    }

    public void ApplyOwnBatonPass(BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.UsedBatonPass.Self == true)
        {
            battleScreen.FieldEffects.UsedBatonPass.Self = false;
            battleScreen.SelfPokemon.StatAttack = battleScreen.FieldEffects.BatonPassStats.Self[0];
            battleScreen.SelfPokemon.StatDefense = battleScreen.FieldEffects.BatonPassStats.Self[1];
            battleScreen.SelfPokemon.StatSpAttack = battleScreen.FieldEffects.BatonPassStats.Self[2];
            battleScreen.SelfPokemon.StatSpDefense = battleScreen.FieldEffects.BatonPassStats.Self[3];
            battleScreen.SelfPokemon.StatSpeed = battleScreen.FieldEffects.BatonPassStats.Self[4];
            battleScreen.SelfPokemon.Evasion = battleScreen.FieldEffects.BatonPassStats.Self[5];
            battleScreen.SelfPokemon.Accuracy = battleScreen.FieldEffects.BatonPassStats.Self[6];
            if (battleScreen.FieldEffects.BatonPassConfusion.Self == true)
            {
                battleScreen.FieldEffects.BatonPassConfusion.Self = false;
                battleScreen.SelfPokemon.AddVolatileStatus(Pokemon.VolatileStatus.Confusion);
            }
        }
    }

    public void SwitchInOwn(BattleScreen battleScreen, int newPokemonIndex, bool firstTime, int insertIndex, String message = "", bool hasSwitched = false)
    {
        _hasSwitchedInOwn = true;
        if (firstTime == false)
        {
            ChangeCameraAngle(1, true, battleScreen);
            String insertMessage = message;
            if (insertMessage == String.Empty)
            {
                insertMessage = "Come back, " + battleScreen.SelfPokemon.GetDisplayName() + "!";
            }
            battleScreen.AddToQuery(insertIndex, new TextQueryObject(insertMessage));
            float returnPositionOffsetY = 0.0f;
            if (battleScreen.SelfPokemonNPC.Model != null)
            {
                returnPositionOffsetY = 0.5f;
            }
            AnimationQueryObject ballReturn = new AnimationQueryObject(battleScreen.SelfPokemonNPC, false);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                ballReturn.AnimationPlaySound(@"Battle\Pokeball\Open", 0, 0);
                for (int smokeReturned = 0; smokeReturned <= 38; smokeReturned++)
                {
                    Vector3 smokePosition = new Vector3((float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0));
                    Vector3 smokeDestination = new Vector3(0, returnPositionOffsetY, 0);
                    Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                    Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                    float smokeSpeed = (float)(Core.Random.Next(1, 3) / 20.0f);
                    Entity smokeEntity = ballReturn.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1.0f);
                    ballReturn.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 0.0f, 0.0f);
                }
            }
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                ballReturn.AnimationFade(null, false, 1, 0, 1, 0);
                ballReturn.AnimationPlaySound(@"Battle\Pokeball\Throw", 1, 0);
                Entity ballReturnEntity = ballReturn.SpawnEntity(new Vector3(0, 0 + returnPositionOffsetY, 0), battleScreen.SelfPokemon.catchBall.Texture, new Vector3(0.3f), 1.0f);
                ballReturn.AnimationMove(ballReturnEntity, true, -2, 0 + returnPositionOffsetY, 0, 0.1f, false, true, 1, 0, spinZSpeed: 0.3f);
                battleScreen.AddToQuery(insertIndex, ballReturn);
            }
            if (hasSwitched == true && battleScreen.IsTrainerBattle == true)
            {
                battleScreen.TrainerRecallOwn += 1;
                if (battleScreen.Trainer.RecallOwnMessage.ContainsKey(battleScreen.TrainerRecallOwn))
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.RecallOwnMessage[battleScreen.TrainerRecallOwn]).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                    ChangeCameraAngle(1, true, battleScreen);
                }
            }
            int index = newPokemonIndex;
            if (index <= -1)
            {
                for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
                {
                    if (Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
                    {
                        index = i;
                        break;
                    }
                }
            }
            battleScreen.SelfPokemonIndex = index;
            if (battleScreen.ParticipatedPokemon.Contains(battleScreen.SelfPokemonIndex) == false)
            {
                battleScreen.ParticipatedPokemon.Add(battleScreen.SelfPokemonIndex);
            }
            battleScreen.SelfPokemon = Core.Player.Pokemons[index];
            ApplyOwnBatonPass(battleScreen);
            String ownShiny = "N";
            if (battleScreen.SelfPokemon.IsShiny == true) { ownShiny = "S"; }
            String ownModel = battleScreen.GetModelName(true);
            if (ownModel == String.Empty)
            {
                battleScreen.AddToQuery(insertIndex, new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.SelfPokemon, PokemonForms.GetOverworldSpriteName(battleScreen.SelfPokemon, true), 0, 1, -1, -1));
            }
            else
            {
                battleScreen.AddToQuery(insertIndex, new ToggleEntityQueryObject(true, ownModel, 1, 0, -1, -1));
            }
            float sendBallPositionOffsetY = 0.0f;
            float sendPokemonPositionOffsetY = 0.0f;
            if (battleScreen.SelfPokemonNPC.Model != null)
            {
                sendBallPositionOffsetY = 0.5f;
                sendPokemonPositionOffsetY = -0.5f;
            }
            if (Core.Player.ShowBattleAnimations == 0 || battleScreen.IsPVPBattle == true)
            {
                battleScreen.AddToQuery(insertIndex, new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.SelfPokemon, 1, -1, -1, -1, -1));
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(battleScreen.SelfPokemon.Number.ToString(), true));
            }
            battleScreen.AddToQuery(insertIndex, new TextQueryObject("Go, " + battleScreen.SelfPokemon.GetDisplayName() + "!"));
            AnimationQueryObject ballThrow = new AnimationQueryObject(battleScreen.SelfPokemonNPC, false);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                ballThrow.AnimationPlaySound(@"Battle\Pokeball\Throw", 0, 0);
                Entity ballThrowEntity = ballThrow.SpawnEntity(new Vector3(-2, -0.15f, 0), battleScreen.SelfPokemon.catchBall.Texture, new Vector3(0.3f), 1.0f);
                ballThrow.AnimationMove(ballThrowEntity, true, 0, (float)(0.35 + sendBallPositionOffsetY), 0, 0.1f, false, true, 0f, 0.5f, spinZSpeed: -0.3f, moveYSpeed: 0.025f);
                ballThrow.AnimationPlaySound(@"Battle\Pokeball\Open", 3, 0);
                for (int smokeSpawned = 0; smokeSpawned <= 38; smokeSpawned++)
                {
                    Vector3 smokePosition = new Vector3(0, 0.35f + sendBallPositionOffsetY, 0);
                    Vector3 smokeDestination = new Vector3((float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0) + sendBallPositionOffsetY, (float)(Core.Random.Next(-10, 10) / 10.0));
                    Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                    Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                    float smokeSpeed = (float)(Core.Random.Next(1, 3) / 20.0f);
                    Entity smokeEntity = ballThrow.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1.0f, 3);
                    ballThrow.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 3.0f, 0.0f);
                }
            }
            String crySuffixOwn = PokemonForms.GetCrySuffix(battleScreen.SelfPokemon);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                ballThrow.AnimationSetPosition(null, false, 12, 0.5f, 13, 0, 0);
                ballThrow.AnimationFade(null, false, 1, 1, 3, 0);
                ballThrow.AnimationPlaySound(battleScreen.SelfPokemon.Number.ToString(), 4, 0, isPokemon: true, crySuffix: crySuffixOwn);
                ballThrow.AnimationMove(null, false, 0, -0.5f + sendPokemonPositionOffsetY, 0, 0.05f, false, false, 5, 0, movementCurve: 3);
                battleScreen.AddToQuery(insertIndex, ballThrow);
            }
        }
        if (battleScreen.FieldEffects.UsedPokemon.Contains(newPokemonIndex) == false)
        {
            battleScreen.FieldEffects.UsedPokemon.Add(newPokemonIndex);
        }
        if (battleScreen.SelfPokemon.Item != null)
        {
            if (battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "amulet coin" || battleScreen.SelfPokemon.Item.OriginalName.ToLower() == "luck incense")
            {
                if (battleScreen.FieldEffects.CanUseItem(true) == true && battleScreen.FieldEffects.CanUseOwnItem(true, battleScreen) == true)
                {
                    battleScreen.FieldEffects.AmuletCoin += 1;
                }
            }
        }
        Pokemon p = battleScreen.SelfPokemon;
        bool spikeAffected = battleScreen.FieldEffects.IsGrounded(true, battleScreen);
        bool rockAffected = true;
        if (spikeAffected == true)
        {
            if (battleScreen.FieldEffects.Spikes.Opponent > 0 && (p.Ability.Name.ToLower() != "magic guard" || battleScreen.FieldEffects.CanUseAbility(true, battleScreen, 1) == false))
            {
                double spikeDamage = 1.0;
                switch (battleScreen.FieldEffects.Spikes.Opponent)
                {
                    case 1: spikeDamage = (p.MaxHP / 100.0) * 12.5; break;
                    case 2: spikeDamage = (p.MaxHP / 100.0) * 16.7; break;
                    case 3: spikeDamage = (p.MaxHP / 100.0) * 25.0; break;
                }
                ReduceHP((int)spikeDamage, true, false, battleScreen, "The Spikes hurt " + p.GetDisplayName() + "!", "spikes");
            }
        }
        if (spikeAffected == true)
        {
            if (battleScreen.FieldEffects.StickyWeb.Opponent > 0)
            {
                LowerStat(true, true, battleScreen, "Speed", 1, "Your Pokémon was caught in a Sticky Web!", "stickyweb");
            }
        }
        if (spikeAffected == true)
        {
            if (battleScreen.FieldEffects.ToxicSpikes.Opponent > 0 && p.Status == Pokemon.StatusProblems.None && p.Type1.Type != Element.Types.Poison && p.Type2.Type != Element.Types.Poison)
            {
                switch (battleScreen.FieldEffects.ToxicSpikes.Opponent)
                {
                    case 1: InflictPoison(true, false, battleScreen, false, "The Toxic Spikes hurt " + p.GetDisplayName() + "!", "toxicspikes"); break;
                    case 2: InflictPoison(true, false, battleScreen, true, "The Toxic Spikes hurt " + p.GetDisplayName() + "!", "toxicspikes"); break;
                }
            }
            if (battleScreen.FieldEffects.ToxicSpikes.Opponent > 0)
            {
                if (p.Type1.Type == Element.Types.Poison || p.Type2.Type == Element.Types.Poison)
                {
                    battleScreen.AddToQuery(insertIndex, new TextQueryObject(p.GetDisplayName() + " removed the Toxic Spikes!"));
                    battleScreen.FieldEffects.ToxicSpikes.Opponent = 0;
                }
            }
        }
        if (rockAffected == true)
        {
            if (battleScreen.FieldEffects.StealthRock.Opponent > 0 && (p.Ability.Name.ToLower() != "magic guard" || battleScreen.FieldEffects.CanUseAbility(true, battleScreen, 1) == false))
            {
                double rocksDamage = 1.0;
                float effectiveness = BattleCalculation.ReverseTypeEffectiveness(Element.GetElementMultiplier(new Element(Element.Types.Rock), p.Type1)) * BattleCalculation.ReverseTypeEffectiveness(Element.GetElementMultiplier(new Element(Element.Types.Rock), p.Type2));
                switch (effectiveness)
                {
                    case 0.25f: rocksDamage = (p.MaxHP / 100.0) * 3.125; break;
                    case 0.5f: rocksDamage = (p.MaxHP / 100.0) * 6.25; break;
                    case 1.0f: rocksDamage = (p.MaxHP / 100.0) * 12.5; break;
                    case 2.0f: rocksDamage = (p.MaxHP / 100.0) * 25.0; break;
                    case 4.0f: rocksDamage = (p.MaxHP / 100.0) * 50.0; break;
                }
                ReduceHP((int)rocksDamage, true, false, battleScreen, "The Stealth Rocks hurt " + p.GetDisplayName() + "!", "stealthrocks");
            }
        }
        TriggerAbilityEffect(battleScreen, true);
        TriggerItemEffect(battleScreen, true);
        if (battleScreen.SelfPokemon.Status == Pokemon.StatusProblems.Sleep)
        {
            battleScreen.FieldEffects.SleepTurns.Self = Core.Random.Next(1, 4);
        }
        if (battleScreen.FieldEffects.HealingWish.Self == true)
        {
            battleScreen.FieldEffects.HealingWish.Self = false;
            if (battleScreen.SelfPokemon.HP < battleScreen.SelfPokemon.MaxHP || battleScreen.SelfPokemon.Status != Pokemon.StatusProblems.None)
            {
                GainHP(battleScreen.SelfPokemon.MaxHP - battleScreen.SelfPokemon.HP, true, true, battleScreen, "The Healing Wish came true for " + battleScreen.SelfPokemon.GetDisplayName() + "!", "move:healingwish");
                CureStatusProblem(true, true, battleScreen, String.Empty, "move:healingwish");
            }
        }
        if (firstTime == false && battleScreen.IsTrainerBattle == true)
        {
            battleScreen.TrainerSendOutOwn += 1;
            if (Core.Player.CountFightablePokemon > 1)
            {
                if (battleScreen.Trainer.SendOutXOwnMessage.ContainsKey(battleScreen.TrainerSendOutOwn))
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.SendOutXOwnMessage[battleScreen.TrainerSendOutOwn]).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                }
            }
            else
            {
                if (battleScreen.Trainer.SendOutLastOwnMessage != String.Empty)
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.SendOutLastOwnMessage).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                }
            }
        }
    }

    public void SwitchOutOpp(BattleScreen battleScreen, int index, String message = "", bool hasSwitched = false, bool canAddSwitchPokemonQuery = true)
    {
        if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "natural cure")
        {
            if (battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.Fainted && battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.None)
            {
                battleScreen.OpponentPokemon.Status = Pokemon.StatusProblems.None;
                battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.OpponentPokemon.GetDisplayName() + "'s status problem got healed by Natural Cure"));
            }
        }
        if (battleScreen.OpponentPokemon.Ability.Name.ToLower() == "regenerator")
        {
            if ((battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Fainted || battleScreen.OpponentPokemon.HP == 0) == false)
            {
                int restoreHP = (int)(battleScreen.OpponentPokemon.MaxHP / 3);
                if (restoreHP > 0 && battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP && battleScreen.OpponentPokemon.HP > 0)
                {
                    battleScreen.Battle.GainHP(restoreHP, false, true, battleScreen, battleScreen.OpponentPokemon.GetDisplayName() + "'s HP was restored!", "ability:regenerator");
                }
            }
        }
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == true)
        {
            battleScreen.FieldEffects.BatonPassStats.Opponent = [];
            battleScreen.FieldEffects.BatonPassStats.Opponent.AddRange(new int[] { battleScreen.OpponentPokemon.StatAttack, battleScreen.OpponentPokemon.StatDefense, battleScreen.OpponentPokemon.StatSpAttack, battleScreen.OpponentPokemon.StatSpDefense, battleScreen.OpponentPokemon.StatSpeed, battleScreen.OpponentPokemon.Evasion, battleScreen.OpponentPokemon.Accuracy });
            battleScreen.FieldEffects.BatonPassConfusion.Opponent = battleScreen.OpponentPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true;
        }
        battleScreen.OpponentPokemon.ResetTemp();
        battleScreen.OpponentPokemon.ClearAllVolatiles();
        battleScreen.FieldEffects.SleepTurns.Opponent = 0;
        battleScreen.FieldEffects.TruantRound.Opponent = 0;
        battleScreen.FieldEffects.Taunt.Opponent = 0;
        battleScreen.FieldEffects.Smacked.Opponent = 0;
        battleScreen.FieldEffects.RageCounter.Opponent = 0;
        battleScreen.FieldEffects.Uproar.Opponent = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.FocusEnergy.Opponent = 0; }
        battleScreen.FieldEffects.Endure.Opponent = 0;
        battleScreen.FieldEffects.ProtectCounter.Opponent = 0;
        battleScreen.FieldEffects.DetectCounter.Opponent = 0;
        battleScreen.FieldEffects.KingsShieldCounter.Opponent = 0;
        battleScreen.FieldEffects.ProtectMovesCount.Opponent = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.Ingrain.Opponent = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.Substitute.Opponent = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.MagnetRise.Opponent = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.AquaRing.Opponent = 0; }
        battleScreen.FieldEffects.PoisonCounter.Opponent = 0;
        battleScreen.FieldEffects.Nightmare.Opponent = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.Curse.Opponent = 0; }
        battleScreen.FieldEffects.Outrage.Opponent = 0;
        battleScreen.FieldEffects.Thrash.Opponent = 0;
        battleScreen.FieldEffects.PetalDance.Opponent = 0;
        battleScreen.FieldEffects.Encore.Opponent = 0;
        battleScreen.FieldEffects.EncoreMove.Opponent = null;
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.Embargo.Opponent = 0; }
        battleScreen.FieldEffects.Yawn.Opponent = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.PerishSongCount.Opponent = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.ConfusionTurns.Opponent = 0; }
        battleScreen.FieldEffects.Torment.Opponent = 0;
        battleScreen.FieldEffects.TormentMove.Opponent = null;
        battleScreen.FieldEffects.ChoiceMove.Opponent = null;
        battleScreen.FieldEffects.Recharge.Opponent = 0;
        battleScreen.FieldEffects.RolloutCounter.Opponent = 0;
        battleScreen.FieldEffects.IceBallCounter.Opponent = 0;
        battleScreen.FieldEffects.DefenseCurl.Opponent = 0;
        battleScreen.FieldEffects.Charge.Opponent = 0;
        battleScreen.FieldEffects.SolarBeam.Opponent = 0;
        battleScreen.FieldEffects.SolarBlade.Opponent = 0;
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.LeechSeed.Opponent = 0; }
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == false) { battleScreen.FieldEffects.LockOn.Opponent = 0; }
        battleScreen.FieldEffects.LansatBerry.Opponent = 0;
        battleScreen.FieldEffects.CustapBerry.Opponent = 0;
        battleScreen.FieldEffects.TrappedCounter.Opponent = 0;
        battleScreen.FieldEffects.FuryCutter.Opponent = 0;
        battleScreen.FieldEffects.EchoedVoice.Opponent = 0;
        battleScreen.FieldEffects.PokemonTurns.Opponent = 0;
        battleScreen.FieldEffects.StockpileCount.Opponent = 0;
        battleScreen.FieldEffects.DestinyBond.Opponent = false;
        battleScreen.FieldEffects.GastroAcid.Opponent = false;
        battleScreen.FieldEffects.TarShot.Opponent = false;
        battleScreen.FieldEffects.FlyCounter.Opponent = 0;
        battleScreen.FieldEffects.DigCounter.Opponent = 0;
        battleScreen.FieldEffects.BounceCounter.Opponent = 0;
        battleScreen.FieldEffects.DiveCounter.Opponent = 0;
        battleScreen.FieldEffects.ShadowForceCounter.Opponent = 0;
        battleScreen.FieldEffects.PhantomForceCounter.Opponent = 0;
        battleScreen.FieldEffects.SkyDropCounter.Opponent = 0;
        battleScreen.FieldEffects.GeomancyCounter.Opponent = 0;
        battleScreen.FieldEffects.SkyAttackCounter.Opponent = 0;
        battleScreen.FieldEffects.RazorWindCounter.Opponent = 0;
        battleScreen.FieldEffects.SkullBashCounter.Opponent = 0;
        battleScreen.FieldEffects.Foresight.Opponent = 0;
        battleScreen.FieldEffects.OdorSleuth.Opponent = 0;
        battleScreen.FieldEffects.MiracleEye.Opponent = 0;
        battleScreen.FieldEffects.Wrap.Opponent = 0;
        battleScreen.FieldEffects.Whirlpool.Opponent = 0;
        battleScreen.FieldEffects.Bind.Opponent = 0;
        battleScreen.FieldEffects.Clamp.Opponent = 0;
        battleScreen.FieldEffects.FireSpin.Opponent = 0;
        battleScreen.FieldEffects.MagmaStorm.Opponent = 0;
        battleScreen.FieldEffects.SandTomb.Opponent = 0;
        battleScreen.FieldEffects.Infestation.Opponent = 0;
        battleScreen.FieldEffects.BideCounter.Opponent = 0;
        battleScreen.FieldEffects.BideDamage.Opponent = 0;
        battleScreen.FieldEffects.RoostUsed.Opponent = false;
        battleScreen.FieldEffects.TrappedCounter.Self = 0;
        battleScreen.FieldEffects.Wrap.Self = 0;
        battleScreen.FieldEffects.Whirlpool.Self = 0;
        battleScreen.FieldEffects.Bind.Self = 0;
        battleScreen.FieldEffects.Clamp.Self = 0;
        battleScreen.FieldEffects.FireSpin.Self = 0;
        battleScreen.FieldEffects.MagmaStorm.Self = 0;
        battleScreen.FieldEffects.SandTomb.Self = 0;
        battleScreen.FieldEffects.Infestation.Self = 0;
        if (battleScreen.SelfPokemon.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation))
        {
            battleScreen.SelfPokemon.RemoveVolatileStatus(Pokemon.VolatileStatus.Infatuation);
        }
        battleScreen.OpponentPokemon.Ability.SwitchOut(battleScreen.OpponentPokemon);
        if (battleScreen.IsTrainerBattle == false)
        {
            battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon, 2, -1, -1, -1, -1));
            EndBattle(EndBattleReasons.WinWild, battleScreen, false);
        }
        else
        {
            if (battleScreen.TrainerHasFightablePokemon() == true)
            {
                if (battleScreen.OpponentPokemon.HP <= 0 || battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Fainted)
                {
                    GainEXP(battleScreen);
                }
                if (battleScreen.IsRemoteBattle && battleScreen.OppFaint)
                {
                    // Next pokemon is selected by the opponent.
                }
                else
                {
                    SwitchInOpp(battleScreen, false, index, hasSwitched, canAddSwitchPokemonQuery);
                }
            }
            else
            {
                GainEXP(battleScreen);
                ChangeCameraAngle(1, false, battleScreen);
                if (message == String.Empty)
                {
                    message = battleScreen.Trainer.Name + ": \"Come back, " + battleScreen.OpponentPokemon.GetDisplayName() + "!\"";
                }
                battleScreen.BattleQuery.Add(new TextQueryObject(message));
                if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
                {
                    AnimationQueryObject ballReturn = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, true);
                    ballReturn.AnimationPlaySound(@"Battle\Pokeball\Open", 0, 0);
                    for (int smokeReturned = 0; smokeReturned <= 38; smokeReturned++)
                    {
                        Vector3 smokePosition = new Vector3((float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0));
                        Vector3 smokeDestination = new Vector3(0, 0, 0);
                        Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                        Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                        float smokeSpeed = (float)(Core.Random.Next(1, 3) / 20.0f);
                        Entity smokeEntity = ballReturn.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1);
                        ballReturn.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 0.0f, 0.0f);
                    }
                    ballReturn.AnimationFade(null, false, 1, 0, 1, 0);
                    ballReturn.AnimationMove(null, false, 0, 0.5f, 0, 0.5f, false, false, 2, 0);
                    ballReturn.AnimationPlaySound(@"Battle\Pokeball\Throw", 1, 0);
                    Entity ballReturnEntity = ballReturn.SpawnEntity(new Vector3(0, 0, 0), battleScreen.OpponentPokemon.catchBall.Texture, new Vector3(0.3f), 1.0f);
                    ballReturn.AnimationMove(ballReturnEntity, true, -2, 0, 0, 0.1f, false, true, 0f, 0f, spinZSpeed: 0.3f);
                    battleScreen.BattleQuery.Add(ballReturn);
                }
                else
                {
                    battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon, 2, -1, -1, -1, -1));
                }
                EndBattle(EndBattleReasons.WinTrainer, battleScreen, false);
                if (battleScreen.IsRemoteBattle == true)
                {
                    EndBattle(EndBattleReasons.WinTrainer, battleScreen, true);
                }
            }
        }
    }

    public void ApplyOppBatonPass(BattleScreen battleScreen)
    {
        if (battleScreen.FieldEffects.UsedBatonPass.Opponent == true)
        {
            battleScreen.FieldEffects.UsedBatonPass.Opponent = false;
            battleScreen.OpponentPokemon.StatAttack = battleScreen.FieldEffects.BatonPassStats.Opponent[0];
            battleScreen.OpponentPokemon.StatDefense = battleScreen.FieldEffects.BatonPassStats.Opponent[1];
            battleScreen.OpponentPokemon.StatSpAttack = battleScreen.FieldEffects.BatonPassStats.Opponent[2];
            battleScreen.OpponentPokemon.StatSpDefense = battleScreen.FieldEffects.BatonPassStats.Opponent[3];
            battleScreen.OpponentPokemon.StatSpeed = battleScreen.FieldEffects.BatonPassStats.Opponent[4];
            battleScreen.OpponentPokemon.Evasion = battleScreen.FieldEffects.BatonPassStats.Opponent[5];
            battleScreen.OpponentPokemon.Accuracy = battleScreen.FieldEffects.BatonPassStats.Opponent[6];
            if (battleScreen.FieldEffects.BatonPassConfusion.Opponent == true)
            {
                battleScreen.FieldEffects.BatonPassConfusion.Opponent = false;
                battleScreen.OpponentPokemon.AddVolatileStatus(Pokemon.VolatileStatus.Confusion);
            }
        }
    }

    public void SwitchInOpp(BattleScreen battleScreen, bool firstTime, int index, bool hasSwitched = false, bool canAddSwitchPokemonQuery = true)
    {
        bool addSwitch = false;
        if (hasSwitched == true && battleScreen.IsTrainerBattle == true)
        {
            battleScreen.TrainerRecallOpp += 1;
            if (battleScreen.Trainer.RecallOwnMessage.ContainsKey(battleScreen.TrainerRecallOpp))
            {
                QueryObject s1 = battleScreen.FocusOppPlayer();
                TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.RecallOppMessage[battleScreen.TrainerRecallOwn]).ToString());
                battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
            }
        }
        if (firstTime == false)
        {
            ChangeCameraAngle(1, false, battleScreen);
            _hasSwitchedInOpp = true;
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.Trainer.Name + ": \"Come back, " + battleScreen.OpponentPokemon.GetDisplayName() + "!\""));
            float returnPositionOffsetY = 0.0f;
            if (battleScreen.OpponentPokemonNPC.Model != null)
            {
                returnPositionOffsetY = 0.5f;
            }
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                AnimationQueryObject ballReturn = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, true);
                ballReturn.AnimationPlaySound(@"Battle\Pokeball\Open", 0, 0);
                for (int smokeReturned = 0; smokeReturned <= 38; smokeReturned++)
                {
                    Vector3 smokePosition = new Vector3((float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0));
                    Vector3 smokeDestination = new Vector3(0, returnPositionOffsetY, 0);
                    Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                    Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                    float smokeSpeed = (float)(Core.Random.Next(1, 3) / 20.0f);
                    Entity smokeEntity = ballReturn.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1);
                    ballReturn.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 0.0f, 0.0f);
                }
                ballReturn.AnimationFade(null, false, 1, 0, 1, 0);
                ballReturn.AnimationPlaySound(@"Battle\Pokeball\Throw", 1, 0);
                Entity ballReturnEntity = ballReturn.SpawnEntity(new Vector3(0, 0 + returnPositionOffsetY, 0), battleScreen.OpponentPokemon.catchBall.Texture, new Vector3(0.3f), 1.0f);
                ballReturn.AnimationMove(ballReturnEntity, true, -2, 0 + returnPositionOffsetY, 0, 0.1f, false, true, 1.0f, 0f, spinZSpeed: 0.3f);
                battleScreen.BattleQuery.Add(ballReturn);
            }
            else
            {
                battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon, 1, -1, -1, -1, -1));
            }
            battleScreen.SendInNewTrainerPokemon(index);
            ApplyOppBatonPass(battleScreen);
            if (battleScreen.ParticipatedPokemon.Contains(battleScreen.SelfPokemonIndex) == false)
            {
                battleScreen.ParticipatedPokemon.Add(battleScreen.SelfPokemonIndex);
            }
            if (battleScreen.FieldEffects.DigCounter.Self == 0 && battleScreen.FieldEffects.FlyCounter.Self == 0 && battleScreen.FieldEffects.DiveCounter.Self == 0)
            {
                if (Core.Player.BattleStyle != 1 && OppStep.StepType != BattleRoundConst.StepTypes.Switch && battleScreen.IsPVPBattle == false && canAddSwitchPokemonQuery == true)
                {
                    addSwitch = true;
                    battleScreen.BattleQuery.Add(new SwitchPokemonQueryObject(battleScreen, battleScreen.OpponentPokemon));
                    battleScreen.Battle.ChangeCameraAngle(1, false, battleScreen);
                }
            }
            String oppModel = battleScreen.GetModelName(false);
            if (oppModel == String.Empty)
            {
                battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon, PokemonForms.GetOverworldSpriteName(battleScreen.OpponentPokemon, true), -1, -1, 0, 1));
            }
            else
            {
                battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(false, oppModel, -1, -1, 1, 0));
            }
            float sendBallPositionOffsetY = 0.0f;
            float sendPokemonPositionOffsetY = 0.0f;
            if (battleScreen.OpponentPokemonNPC.Model != null)
            {
                sendBallPositionOffsetY = 0.5f;
                sendPokemonPositionOffsetY = -0.5f;
            }
            battleScreen.BattleQuery.Add(new ToggleEntityQueryObject(true, ToggleEntityQueryObject.BattleEntities.OpponentPokemon, 1, -1, -1, -1, -1));
            if (Core.Player.ShowBattleAnimations == 0 || battleScreen.IsPVPBattle == true)
            {
                battleScreen.BattleQuery.Add(new PlaySoundQueryObject(battleScreen.OpponentPokemon.Number.ToString(), true));
            }
            battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.Trainer.Name + ": \"Go, " + battleScreen.OpponentPokemon.GetDisplayName() + "!\""));
            AnimationQueryObject ballThrow = new AnimationQueryObject(battleScreen.OpponentPokemonNPC, false);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                ballThrow.AnimationPlaySound(@"Battle\Pokeball\Throw", 0, 0);
                Entity ballThrowEntity = ballThrow.SpawnEntity(new Vector3(2, -0.15f, 0), battleScreen.OpponentPokemon.catchBall.Texture, new Vector3(0.3f), 1.0f);
                ballThrow.AnimationMove(ballThrowEntity, true, 0, 0.35f + sendBallPositionOffsetY, 0, 0.1f, false, true, 0f, 0.5f, spinZSpeed: 0.3f, moveYSpeed: 0.025f);
                ballThrow.AnimationPlaySound(@"Battle\Pokeball\Open", 3, 0);
                for (int smokeSpawned = 0; smokeSpawned <= 38; smokeSpawned++)
                {
                    Vector3 smokePosition = new Vector3(0, 0.35f + sendBallPositionOffsetY, 0);
                    Vector3 smokeDestination = new Vector3((float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0) + sendBallPositionOffsetY, (float)(Core.Random.Next(-10, 10) / 10.0));
                    Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                    Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                    float smokeSpeed = (float)(Core.Random.Next(1, 3) / 20.0f);
                    Entity smokeEntity = ballThrow.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1, 3);
                    ballThrow.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 3.0f, 0.0f);
                }
            }
            else
            {
                battleScreen.Battle.ChangeCameraAngle(1, false, battleScreen);
            }
            String crySuffixOpp = PokemonForms.GetCrySuffix(battleScreen.OpponentPokemon);
            if (Core.Player.ShowBattleAnimations != 0 && battleScreen.IsPVPBattle == false)
            {
                ballThrow.AnimationSetPosition(null, false, 15, 0.5f, 13, 0, 0);
                ballThrow.AnimationFade(null, false, 1, 1, 3, 0);
                ballThrow.AnimationPlaySound(battleScreen.OpponentPokemon.Number.ToString(), 4, 0, isPokemon: true, crySuffix: crySuffixOpp);
                ballThrow.AnimationMove(null, false, 0, -0.5f + sendPokemonPositionOffsetY, 0, 0.05f, false, false, 5, 0, movementCurve: 3);
                battleScreen.BattleQuery.Add(ballThrow);
            }
        }
        if (addSwitch == false)
        {
            Pokemon p = battleScreen.OpponentPokemon;
            bool spikeAffected = battleScreen.FieldEffects.IsGrounded(false, battleScreen);
            bool rockAffected = true;
            if (spikeAffected == true)
            {
                if (battleScreen.FieldEffects.Spikes.Self > 0 && p.Ability.Name.ToLower() != "magic guard")
                {
                    double spikeDamage = 1.0;
                    switch (battleScreen.FieldEffects.Spikes.Self)
                    {
                        case 1: spikeDamage = (p.MaxHP / 100.0) * 12.5; break;
                        case 2: spikeDamage = (p.MaxHP / 100.0) * 16.7; break;
                        case 3: spikeDamage = (p.MaxHP / 100.0) * 25.0; break;
                    }
                    ReduceHP((int)spikeDamage, false, true, battleScreen, "The Spikes hurt " + p.GetDisplayName() + "!", "spikes");
                }
            }
            if (spikeAffected == true)
            {
                if (battleScreen.FieldEffects.StickyWeb.Self > 0)
                {
                    LowerStat(false, false, battleScreen, "Speed", 1, "The opposing Pokémon was caught in a Sticky Web!", "stickyweb");
                }
            }
            if (spikeAffected == true)
            {
                if (battleScreen.FieldEffects.ToxicSpikes.Self > 0 && p.Status == Pokemon.StatusProblems.None && p.Type1.Type != Element.Types.Poison && (p.Type2 != null && p.Type2.Type != Element.Types.Poison))
                {
                    switch (battleScreen.FieldEffects.ToxicSpikes.Self)
                    {
                        case 1: InflictPoison(false, true, battleScreen, false, "The Toxic Spikes hurt " + p.GetDisplayName() + "!", "toxicspikes"); break;
                        case 2: InflictPoison(false, true, battleScreen, true, "The Toxic Spikes hurt " + p.GetDisplayName() + "!", "toxicspikes"); break;
                    }
                }
                if (battleScreen.FieldEffects.ToxicSpikes.Self > 0)
                {
                    if (p.Type1.Type == Element.Types.Poison || p.Type2.Type == Element.Types.Poison)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " removed the Toxic Spikes!"));
                        battleScreen.FieldEffects.ToxicSpikes.Self = 0;
                    }
                }
            }
            if (rockAffected == true)
            {
                if (battleScreen.FieldEffects.StealthRock.Self > 0 && p.Ability.Name.ToLower() != "magic guard")
                {
                    double rocksDamage = 1.0;
                    float effectiveness = BattleCalculation.ReverseTypeEffectiveness(Element.GetElementMultiplier(new Element(Element.Types.Rock), p.Type1)) * BattleCalculation.ReverseTypeEffectiveness(Element.GetElementMultiplier(new Element(Element.Types.Rock), p.Type2));
                    switch (effectiveness)
                    {
                        case 0.25f: rocksDamage = (p.MaxHP / 100.0) * 3.125; break;
                        case 0.5f: rocksDamage = (p.MaxHP / 100.0) * 6.25; break;
                        case 1.0f: rocksDamage = (p.MaxHP / 100.0) * 12.5; break;
                        case 2.0f: rocksDamage = (p.MaxHP / 100.0) * 25.0; break;
                        case 4.0f: rocksDamage = (p.MaxHP / 100.0) * 50.0; break;
                    }
                    ReduceHP((int)rocksDamage, false, true, battleScreen, "The Stealth Rocks hurt " + p.GetDisplayName() + "!", "stealthrocks");
                }
            }
            TriggerAbilityEffect(battleScreen, false);
            TriggerItemEffect(battleScreen, false);
            if (battleScreen.OpponentPokemon.Status == Pokemon.StatusProblems.Sleep)
            {
                battleScreen.FieldEffects.SleepTurns.Opponent = Core.Random.Next(1, 4);
            }
            if (battleScreen.FieldEffects.HealingWish.Opponent == true)
            {
                battleScreen.FieldEffects.HealingWish.Opponent = false;
                if (battleScreen.OpponentPokemon.HP < battleScreen.OpponentPokemon.MaxHP || battleScreen.OpponentPokemon.Status != Pokemon.StatusProblems.None)
                {
                    GainHP(battleScreen.OpponentPokemon.MaxHP - battleScreen.OpponentPokemon.HP, false, false, battleScreen, "The Healing Wish came true for " + battleScreen.OpponentPokemon.GetDisplayName() + "!", "move:healingwish");
                    CureStatusProblem(false, false, battleScreen, String.Empty, "move:healingwish");
                }
            }
            if (Core.Player.BattleStyle == 1 || battleScreen.IsPVPBattle == true)
            {
                battleScreen.HasSwitchedOwn = false;
            }
        }
        if (firstTime == false && Core.Player.BattleStyle == 1)
        {
            battleScreen.TrainerSendOutOpp += 1;
            if (battleScreen.Trainer.CountUseablePokemon > 1)
            {
                if (battleScreen.Trainer.SendOutXOppMessage.ContainsKey(battleScreen.TrainerSendOutOpp))
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.SendOutXOppMessage[battleScreen.TrainerSendOutOpp]).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                }
            }
            else
            {
                if (battleScreen.Trainer.SendOutLastOppMessage != String.Empty)
                {
                    QueryObject s1 = battleScreen.FocusOppPlayer();
                    TextQueryObject s2 = new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.SendOutLastOppMessage).ToString());
                    battleScreen.BattleQuery.AddRange(new QueryObject[] { s1, s2 });
                }
            }
        }
    }

    private void EndBattle(EndBattleReasons reason, BattleScreen battleScreen, bool addPVP = false)
    {
        battleScreen.OwnFaint = false;
        battleScreen.OppFaint = false;
        isAfterFaint = false;
        if (addPVP == true)
        {
            switch (reason)
            {
                case EndBattleReasons.WinTrainer:
                {
                    CameraQueryObject q = new CameraQueryObject(new Vector3(12, 0, 13), Screen.Camera.Position, 0.03f, 0.03f, (MathHelper.Pi * 0.5f), Screen.Camera.Yaw, 0.0f, Screen.Camera.Pitch, 0.02f, 0.02f);
                    q.ApplyCurrentCamera = true;
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 5, q);
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 4, new TextQueryObject("You lost the battle!"));
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 3, new TextQueryObject(String.Empty));
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 2, new TextQueryObject(String.Empty));
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 1, new EndBattleQueryObject(true));
                    break;
                }
                case EndBattleReasons.LoseTrainer:
                {
                    CameraQueryObject q = new CameraQueryObject(new Vector3(15, 0, 13), Screen.Camera.Position, 0.03f, 0.03f, -(MathHelper.Pi * 0.5f), Screen.Camera.Yaw, 0.0f, Screen.Camera.Pitch, 0.02f, 0.02f);
                    q.ApplyCurrentCamera = true;
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 3, q);
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 2, new TextQueryObject("Pokémon Trainer " + Core.Player.Name + " was defeated!"));
                    battleScreen.TempPVPBattleQuery.Add(battleScreen.BattleQuery.Count - 1, new EndBattleQueryObject(true));
                    break;
                }
            }
        }
        else
        {
            switch (reason)
            {
                case EndBattleReasons.WinWild:
                {
                    Won = true;
                    Core.Player.AddPoints(1, "Won against wild Pokémon.");
                    String musicLoop = Screen.Level.CurrentRegion.Split(',')[0] + "_wild_defeat";
                    if (MusicManager.SongExists(musicLoop) == false)
                    {
                        musicLoop = "wild_defeat";
                    }
                    battleScreen.BattleQuery.Add(new PlayMusicQueryObject(musicLoop));
                    ChangeCameraAngle(1, true, battleScreen);
                    GainEXP(battleScreen);
                    if (battleScreen.FieldEffects.PayDayCounter.Self > 0)
                    {
                        Core.Player.Money += battleScreen.FieldEffects.PayDayCounter.Self;
                        battleScreen.BattleQuery.Add(new TextQueryObject(Core.Player.Name + " picked up $" + battleScreen.FieldEffects.PayDayCounter.Self + "!"));
                    }
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                    break;
                }
                case EndBattleReasons.WinTrainer:
                {
                    Won = true;
                    Core.Player.AddPoints(3, "Won against trainer.");
                    Core.Player.Money += battleScreen.GetTrainerMoney();
                    battleScreen.BattleQuery.Add(new PlayMusicQueryObject(battleScreen.Trainer.GetDefeatMusic()));
                    CameraQueryObject q = new CameraQueryObject(new Vector3(15, 0, 13), Screen.Camera.Position, 0.03f, 0.03f, -(MathHelper.Pi * 0.5f), Screen.Camera.Yaw, 0.0f, Screen.Camera.Pitch, 0.04f, 0.02f);
                    q.ApplyCurrentCamera = true;
                    battleScreen.BattleQuery.Add(q);
                    battleScreen.BattleQuery.Add(new TextQueryObject(battleScreen.Trainer.TrainerType + " " + battleScreen.Trainer.Name + " was defeated!"));
                    battleScreen.BattleQuery.Add(new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.OutroMessage).ToString()));
                    if (battleScreen.GetTrainerMoney() > 0)
                    {
                        battleScreen.BattleQuery.Add(new TextQueryObject(Core.Player.Name + " got $" + battleScreen.GetTrainerMoney() + "!"));
                    }
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                    break;
                }
                case EndBattleReasons.LoseTrainer:
                case EndBattleReasons.LoseWild:
                {
                    Won = false;
                    CameraQueryObject q = new CameraQueryObject(new Vector3(12, 0, 13), Screen.Camera.Position, 0.03f, 0.03f, (MathHelper.Pi * 0.5f), Screen.Camera.Yaw, 0.0f, Screen.Camera.Pitch, 0.02f, 0.02f);
                    q.ApplyCurrentCamera = true;
                    battleScreen.BattleQuery.Add(q);
                    battleScreen.BattleQuery.Add(new TextQueryObject("You lost the battle!"));
                    if (battleScreen.IsTrainerBattle == true && battleScreen.Trainer.PlayerLossMessage != String.Empty)
                    {
                        CameraQueryObject q1 = new CameraQueryObject(new Vector3(15, 0, 13), Screen.Camera.Position, 0.03f, 0.03f, -(MathHelper.Pi * 0.5f), Screen.Camera.Yaw, 0.0f, Screen.Camera.Pitch, 0.04f, 0.02f);
                        q1.ApplyCurrentCamera = true;
                        battleScreen.BattleQuery.Add(q);
                        battleScreen.BattleQuery.Add(new TextQueryObject(ScriptVersion2.ScriptCommander.Parse(battleScreen.Trainer.PlayerLossMessage).ToString()));
                    }
                    if (BattleScreen.CanGainLoseMoney == true)
                    {
                        int highestLevel = 1;
                        foreach (Pokemon pokemon in Core.Player.Pokemons)
                        {
                            if (pokemon.Level > highestLevel) { highestLevel = pokemon.Level; }
                        }
                        int basePayout = 8;
                        switch (Core.Player.Badges.Count)
                        {
                            case 1: basePayout = 16; break;
                            case 2: basePayout = 24; break;
                            case 3: basePayout = 36; break;
                            case 4: basePayout = 48; break;
                            case 5: basePayout = 64; break;
                            case 6: basePayout = 80; break;
                            case 7: basePayout = 100; break;
                            case 8: basePayout = 120; break;
                            default:
                                if (Core.Player.Badges.Count > 8) { basePayout = 120; }
                                break;
                        }
                        int lostMoney = (int)(highestLevel * basePayout);
                        if (Core.Player.Money <= lostMoney) { lostMoney = Core.Player.Money; }
                        Core.Player.Money -= lostMoney;
                        if (battleScreen.IsTrainerBattle == false)
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("You panicked and dropped $" + lostMoney.ToString() + "..."));
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject("You gave $" + lostMoney.ToString() + " to the winner..."));
                        }
                    }
                    battleScreen.BattleQuery.Add(new EndBattleQueryObject(true));
                    break;
                }
            }
        }
    }

    private void GainEXP(BattleScreen battleScreen)
    {
        if (battleScreen.IsPVPBattle == false && BattleScreen.CanReceiveEXP == true)
        {
            List<int> expPokemon = [];
            foreach (int i in battleScreen.ParticipatedPokemon)
            {
                if (Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
                {
                    expPokemon.Add(i);
                }
            }
            for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
            {
                if (Core.Player.Inventory.GetItemAmount(658.ToString()) > 0 && Core.Player.EnableExpAll == true)
                {
                    if (expPokemon.Contains(i) == false && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
                    {
                        expPokemon.Add(i);
                    }
                }
                else
                {
                    if (expPokemon.Contains(i) == false && Core.Player.Pokemons[i].Item != null && Core.Player.Pokemons[i].Item!.OriginalName.ToLower() == "exp. share" && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
                    {
                        expPokemon.Add(i);
                    }
                }
            }
            if (expPokemon.Count > 0)
            {
                ChangeCameraAngle(1, true, battleScreen);
            }
            for (int i = 0; i <= expPokemon.Count - 1; i++)
            {
                int pokeIndex = expPokemon[i];
                List<Attack> attackLearnList = [];
                int levelUpAmount = 0;
                int originalLevel = Core.Player.Pokemons[pokeIndex].Level;
                if (Core.Player.Pokemons[pokeIndex].Level < int.Parse(GameModeManager.GetGameRuleValue("MaxLevel", "100")))
                {
                    int exp = BattleCalculation.GainExp(Core.Player.Pokemons[pokeIndex], battleScreen, expPokemon, pokeIndex);
                    battleScreen.BattleQuery.Add(new TextQueryObject(Core.Player.Pokemons[pokeIndex].GetDisplayName() + " gained " + exp + " experience points."));
                    int moveLevel = originalLevel;
                    for (int e = 1; e <= exp; e++)
                    {
                        int[] oldStats;
                        if (Core.Player.Pokemons[pokeIndex].IsTransformed == true)
                        {
                            oldStats = new int[] { Core.Player.Pokemons[pokeIndex].MaxHP, Core.Player.Pokemons[pokeIndex].OriginalStats[0], Core.Player.Pokemons[pokeIndex].OriginalStats[1], Core.Player.Pokemons[pokeIndex].OriginalStats[2], Core.Player.Pokemons[pokeIndex].OriginalStats[3], Core.Player.Pokemons[pokeIndex].OriginalStats[4] };
                        }
                        else
                        {
                            oldStats = new int[] { Core.Player.Pokemons[pokeIndex].MaxHP, Core.Player.Pokemons[pokeIndex].Attack, Core.Player.Pokemons[pokeIndex].Defense, Core.Player.Pokemons[pokeIndex].SpAttack, Core.Player.Pokemons[pokeIndex].SpDefense, Core.Player.Pokemons[pokeIndex].Speed };
                        }
                        Core.Player.Pokemons[pokeIndex].GetExperience(1, false);
                        if (moveLevel < Core.Player.Pokemons[pokeIndex].Level)
                        {
                            moveLevel = Core.Player.Pokemons[pokeIndex].Level;
                            Core.Player.AddPoints(((int)Math.Sqrt(Core.Player.Pokemons[pokeIndex].Level)).Clamp(1, 10), "Leveled up a Pokémon to level " + moveLevel.ToString() + ".");
                            Core.Player.Pokemons[pokeIndex].ChangeFriendShip(Pokemon.FriendShipCauses.LevelUp);
                            Core.Player.Pokemons[pokeIndex].hasLeveledUp = true;
                            battleScreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\exp_max", false));
                            battleScreen.BattleQuery.Add(new TextQueryObject(Localization.GetString("level_up_PokemonReachedLevel", "[POKEMONNAME] reached~level [LEVELNUMBER]!").Replace("[POKEMONNAME]", Core.Player.Pokemons[pokeIndex].GetDisplayName()).Replace("[LEVELNUMBER]", moveLevel.ToString())));
                            battleScreen.BattleQuery.Add(new DisplayLevelUpQueryObject(Core.Player.Pokemons[pokeIndex], oldStats));
                        }
                    }
                    levelUpAmount = moveLevel - originalLevel;
                }
                if (levelUpAmount > 0)
                {
                    for (int l = 1; l <= levelUpAmount; l++)
                    {
                        if (Core.Player.Pokemons[pokeIndex].attackLearns.ContainsKey(originalLevel + l))
                        {
                            List<Attack> aList = Core.Player.Pokemons[pokeIndex].attackLearns[originalLevel + l];
                            for (int a = 0; a <= aList.Count - 1; a++)
                            {
                                if (attackLearnList.Contains(aList[a]) == false && Core.Player.Pokemons[pokeIndex].KnowsMove(aList[a]) == false)
                                {
                                    attackLearnList.Add(aList[a]);
                                }
                            }
                        }
                    }
                }
                if (attackLearnList.Count > 0)
                {
                    for (int a = 0; a <= attackLearnList.Count - 1; a++)
                    {
                        battleScreen.BattleQuery.Add(new LearnMovesQueryObject(Core.Player.Pokemons[pokeIndex], attackLearnList[a], battleScreen));
                    }
                }
                Core.Player.Pokemons[pokeIndex].GainEffort(battleScreen.OpponentPokemon);
            }
        }
        battleScreen.ParticipatedPokemon.Clear();
    }
}
