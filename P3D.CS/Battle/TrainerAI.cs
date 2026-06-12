using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using P3D.Items;

namespace P3D.BattleSystem;

// Trainer AI Levels:
// Normal Trainers: 0-1 (0 = weak trainers like Bug Catchers, 1 = Cool Trainers; higher on rematch)
// (Johto) Gym Leaders: 0-2 (0 = Falkner, 2 = Clair)
// (Kanto) Gym Leaders: All 2
// Elite 4: 2
// Lance: 2
// Elder Li: 0
// Rival: 2
// Team Rocket Grunts: 0
// Team Rocket Admins: 1
public class TrainerAI
{
    public static BattleRoundConst GetAIMove(BattleScreen battleScreen, BattleRoundConst ownStep)
    {
        Pokemon p = battleScreen.OpponentPokemon!;
        Pokemon op = battleScreen.SelfPokemon!;

        List<Attack> m = [..p.attacks];
        for (int i = m.Count - 1; i >= 0; i--)
        {
            if (m[i].currentPP <= 0)
            {
                m.RemoveAt(i);
            }
        }
        if (m.Count == 0)
        {
            return new BattleRoundConst
            {
                StepType = BattleRoundConst.StepTypes.Move,
                Argument = Attack.GetAttackByID(165)
            };
        }

        // ---------------- Encore: force encored move ----------------
        if (battleScreen.FieldEffects.Encore.Opponent > 0)
        {
            int attackIndex = -1;
            for (int a = 0; a < p.attacks.Count; a++)
            {
                if (battleScreen.FieldEffects.EncoreMove.Opponent != null &&
                    p.attacks[a].ID == battleScreen.FieldEffects.EncoreMove.Opponent.ID)
                {
                    attackIndex = a;
                }
            }
            if (attackIndex != -1 && p.attacks[attackIndex].currentPP > 0)
            {
                return new BattleRoundConst
                {
                    StepType = BattleRoundConst.StepTypes.Move,
                    Argument = battleScreen.FieldEffects.EncoreMove.Opponent
                };
            }
            else
            {
                battleScreen.FieldEffects.EncoreMove = (battleScreen.FieldEffects.EncoreMove.Self, null);
                battleScreen.FieldEffects.Encore = (battleScreen.FieldEffects.Encore.Self, 0);
                battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + "'s encore stopped."));
            }
        }

        // ---------------- Random move for AI level 0 ----------------
        if (battleScreen.Trainer!.AILevel <= 0)
        {
            List<int> availableAttacks = [];
            for (int i = 0; i < m.Count; i++)
            {
                availableAttacks.Add(i);
            }
            int oppAttackChoice = Core.Random.Next(0, availableAttacks.Count);
            bool ready = false;
            while (ready == false)
            {
                if (m[oppAttackChoice].Equals(battleScreen.FieldEffects.TormentMove.Opponent) ||
                    m[oppAttackChoice].Disabled > 0 ||
                    (battleScreen.FieldEffects.Taunt.Opponent > 0 &&
                     battleScreen.OpponentPokemon!.attacks[oppAttackChoice].category == Attack.Categories.Status))
                {
                    availableAttacks.Remove(oppAttackChoice);
                    if (availableAttacks.Count > 0)
                    {
                        oppAttackChoice = availableAttacks[Core.Random.Next(0, availableAttacks.Count)];
                    }
                    else
                    {
                        return new BattleRoundConst
                        {
                            StepType = BattleRoundConst.StepTypes.Move,
                            Argument = Attack.GetAttackByID(165)
                        };
                    }
                }
                else
                {
                    ready = true;
                }
            }
            // 35% chance to return random move outright at AI level 0
            if (Core.Random.Next(0, 100) < 35)
            {
                return ProduceOppStep(m, oppAttackChoice);
            }
        }

        // ---------------- Switching (AI level >= 2) ----------------
        if (battleScreen.Trainer!.AILevel >= 2)
        {
            if (BattleCalculation.CanSwitch(battleScreen, false) == true)
            {
                if (battleScreen.Trainer.Pokemons.Count > 0)
                {
                    int living = 0;
                    foreach (Pokemon cP in battleScreen.Trainer.Pokemons)
                    {
                        if (cP.HP > 0 && cP.Status != Pokemon.StatusProblems.Fainted)
                        {
                            living += 1;
                        }
                    }
                    if (living > 1)
                    {
                        // Switch if opponent has super-effective move
                        float maxOpponentEff = 0.0f;
                        foreach (Attack atk in op.attacks)
                        {
                            float effectiveness = BattleCalculation.CalculateEffectiveness(
                                Attack.GetAttackByID(atk.ID), battleScreen, op, p, true);
                            if (effectiveness > maxOpponentEff)
                            {
                                maxOpponentEff = effectiveness;
                            }
                        }
                        if (maxOpponentEff > 1.0f)
                        {
                            int chance = 0;
                            if (maxOpponentEff == 1.25f)
                            {
                                chance = 10;
                            }
                            else if (maxOpponentEff == 1.5f)
                            {
                                chance = 25;
                            }
                            else if (maxOpponentEff == 2.0f)
                            {
                                chance = 35;
                            }
                            else if (maxOpponentEff == 4.0f)
                            {
                                chance = 50;
                            }

                            if (RPercent(chance) == true)
                            {
                                List<int> lessTeamPs = [];
                                for (int i = 0; i < battleScreen.Trainer.Pokemons.Count; i++)
                                {
                                    if (i != battleScreen.OpponentPokemonIndex)
                                    {
                                        Pokemon teamP = battleScreen.Trainer.Pokemons[i];
                                        if (teamP.HP > 0 && teamP.Status != Pokemon.StatusProblems.Fainted)
                                        {
                                            bool alwaysLess = true;
                                            foreach (Attack atk in op.attacks)
                                            {
                                                float effectiveness = BattleCalculation.CalculateEffectiveness(
                                                    Attack.GetAttackByID(atk.ID), battleScreen, op, teamP, true);
                                                if (effectiveness >= maxOpponentEff)
                                                {
                                                    alwaysLess = false;
                                                    break;
                                                }
                                            }
                                            if (alwaysLess == true)
                                            {
                                                lessTeamPs.Add(i);
                                            }
                                        }
                                    }
                                }
                                if (lessTeamPs.Count > 0)
                                {
                                    return ProduceOppStep(lessTeamPs[Core.Random.Next(0, lessTeamPs.Count)]);
                                }
                            }
                        }

                        // Switch if all own moves are 0x effective
                        bool only0 = true;
                        foreach (Attack atk in p.attacks)
                        {
                            float effectiveness = BattleCalculation.CalculateEffectiveness(
                                Attack.GetAttackByID(atk.ID), battleScreen, p, op, false);
                            if (effectiveness != 0.0f)
                            {
                                only0 = false;
                                break;
                            }
                        }
                        if (only0 == true)
                        {
                            List<int> switchList = [];
                            for (int i = 0; i < battleScreen.Trainer.Pokemons.Count; i++)
                            {
                                if (i != battleScreen.OpponentPokemonIndex)
                                {
                                    Pokemon teamP = battleScreen.Trainer.Pokemons[i];
                                    if (teamP.HP > 0 && teamP.Status != Pokemon.StatusProblems.Fainted)
                                    {
                                        switchList.Add(i);
                                    }
                                }
                            }
                            if (switchList.Count > 0)
                            {
                                return ProduceOppStep(switchList[Core.Random.Next(0, switchList.Count)]);
                            }
                        }

                        // Switch if cursed (75%)
                        if (battleScreen.FieldEffects.Curse.Opponent > 0)
                        {
                            if (RPercent(75) == true)
                            {
                                List<int> canSwitchTo = [];
                                for (int i = 0; i < battleScreen.Trainer.Pokemons.Count; i++)
                                {
                                    Pokemon teamP = battleScreen.Trainer.Pokemons[i];
                                    if (teamP.HP > 0 && teamP.Status != Pokemon.StatusProblems.Fainted &&
                                        i != battleScreen.OpponentPokemonIndex)
                                    {
                                        canSwitchTo.Add(i);
                                    }
                                }
                                if (canSwitchTo.Count > 0)
                                {
                                    return ProduceOppStep(canSwitchTo[Core.Random.Next(0, canSwitchTo.Count)]);
                                }
                            }
                        }

                        // Switch if confused (50%)
                        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true)
                        {
                            if (RPercent(50) == true)
                            {
                                List<int> canSwitchTo = [];
                                for (int i = 0; i < battleScreen.Trainer.Pokemons.Count; i++)
                                {
                                    Pokemon teamP = battleScreen.Trainer.Pokemons[i];
                                    if (teamP.HP > 0 && teamP.Status != Pokemon.StatusProblems.Fainted &&
                                        i != battleScreen.OpponentPokemonIndex)
                                    {
                                        canSwitchTo.Add(i);
                                    }
                                }
                                if (canSwitchTo.Count > 0)
                                {
                                    return ProduceOppStep(canSwitchTo[Core.Random.Next(0, canSwitchTo.Count)]);
                                }
                            }
                        }
                    }
                }
            }
        }

        // ---------------- Items ----------------

        // Full Restore: ignore HP, cure status/confusion
        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true ||
            (p.Status != Pokemon.StatusProblems.None && p.Status != Pokemon.StatusProblems.Fainted))
        {
            if (TrainerHasItem(14, battleScreen) == true)
            {
                if ((TrainerHasItem(38, battleScreen) == true && p.HP == p.MaxHP) == false)
                {
                    return ProduceOppStep(14, -1);
                }
            }
        }

        // Potions: use when HP <= 40% (chance 60 + (40 - hp%))
        if (p.HP <= (int)((p.MaxHP / 100.0f) * 40))
        {
            int potion = GetBestPotion(battleScreen);
            if (potion > -1)
            {
                if (TrainerHasItem(potion, battleScreen) == true)
                {
                    int chance = GetPokemonValue(battleScreen, p) + (40 - (int)((p.HP / (float)p.MaxHP) * 100));
                    if (RPercent(chance) == true)
                    {
                        int hp = GetPotionHealHP(p, potion);
                        if (hp >= (int)Math.Ceiling(p.MaxHP / 2.0))
                        {
                            return ProduceOppStep(potion, -1);
                        }
                    }
                }
            }
        }

        // Full Heal: cure status/confusion when HP >= 25%
        if (p.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true ||
            (p.Status != Pokemon.StatusProblems.None && p.Status != Pokemon.StatusProblems.Fainted))
        {
            if (p.HP >= (int)((p.MaxHP / 100.0f) * 25))
            {
                if (TrainerHasItem(38, battleScreen) == true)
                {
                    return ProduceOppStep(38, -1);
                }
            }
        }

        // Status restore items at HP >= 25% (60% chance)
        if (p.HP >= (int)((p.MaxHP / 100.0f) * 25))
        {
            if (p.Status != Pokemon.StatusProblems.None && p.Status != Pokemon.StatusProblems.Fainted)
            {
                if (RPercent(60) == true)
                {
                    switch (p.Status)
                    {
                        case Pokemon.StatusProblems.Poison:
                        case Pokemon.StatusProblems.BadPoison:
                            if (TrainerHasItem(9, battleScreen) == true)
                            {
                                return ProduceOppStep(9, -1);
                            }
                            break;
                        case Pokemon.StatusProblems.Burn:
                            if (TrainerHasItem(10, battleScreen) == true)
                            {
                                return ProduceOppStep(10, -1);
                            }
                            break;
                        case Pokemon.StatusProblems.Freeze:
                            if (TrainerHasItem(11, battleScreen) == true)
                            {
                                return ProduceOppStep(11, -1);
                            }
                            break;
                        case Pokemon.StatusProblems.Sleep:
                            if (TrainerHasItem(12, battleScreen) == true)
                            {
                                return ProduceOppStep(12, -1);
                            }
                            break;
                        case Pokemon.StatusProblems.Paralyzed:
                            if (TrainerHasItem(13, battleScreen) == true)
                            {
                                return ProduceOppStep(13, -1);
                            }
                            break;
                    }
                }
            }
        }

        // Revive when >= 50% of team is beaten (50% chance)
        if (RPercent(50) == true &&
            (TrainerHasItem(39, battleScreen) == true || TrainerHasItem(40, battleScreen) == true))
        {
            int beaten = 0;
            foreach (Pokemon teamP in battleScreen.Trainer!.Pokemons)
            {
                if (teamP.HP <= 0 || teamP.Status == Pokemon.StatusProblems.Fainted || teamP.IsEgg == true)
                {
                    beaten += 1;
                }
            }
            if (beaten >= (int)Math.Floor(battleScreen.Trainer.Pokemons.Count / 2.0))
            {
                int highestLevel = -1;
                for (int i = 0; i < battleScreen.Trainer.Pokemons.Count; i++)
                {
                    Pokemon teamP = battleScreen.Trainer.Pokemons[i];
                    if (teamP.IsEgg == false)
                    {
                        if (teamP.HP <= 0 || teamP.Status == Pokemon.StatusProblems.Fainted)
                        {
                            if (highestLevel == -1 ||
                                battleScreen.Trainer.Pokemons[highestLevel].Level < teamP.Level)
                            {
                                highestLevel = i;
                            }
                        }
                    }
                }
                if (highestLevel > -1)
                {
                    int bestRevive = GetBestRevive(battleScreen);
                    return ProduceOppStep(bestRevive, highestLevel);
                }
            }
        }

        // ---------------- Moves ----------------

        // If asleep: try Sleep Talk (100%)
        if (p.Status == Pokemon.StatusProblems.Sleep &&
            battleScreen.FieldEffects.SleepTurns.Opponent > 1 &&
            HasMove(m, 214) == true &&
            m[IDtoMoveIndex(m, 214)].Disabled == 0 &&
            m[IDtoMoveIndex(m, 214)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
        {
            if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                m[IDtoMoveIndex(m, 214)].category != Attack.Categories.Status)
            {
                return ProduceOppStep(m, IDtoMoveIndex(m, 214));
            }
        }

        // If asleep: try Snore (100%)
        if (p.Status == Pokemon.StatusProblems.Sleep &&
            battleScreen.FieldEffects.SleepTurns.Opponent > 1 &&
            HasMove(m, 173) == true &&
            m[IDtoMoveIndex(m, 173)].Disabled == 0 &&
            m[IDtoMoveIndex(m, 173)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
        {
            if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                m[IDtoMoveIndex(m, 173)].category != Attack.Categories.Status)
            {
                return ProduceOppStep(m, IDtoMoveIndex(m, 173));
            }
        }

        // If frozen: use thaw-out move (100%)
        if (p.Status == Pokemon.StatusProblems.Freeze)
        {
            int chosenMove = MoveAI(m, Attack.AIField.ThrawOut);
            if (chosenMove > -1)
            {
                return ProduceOppStep(m, chosenMove);
            }
        }

        // Fake Out on turn 0 to inflict flinch (100%)
        if (HasMove(m, 252) == true &&
            m[IDtoMoveIndex(m, 252)].Disabled == 0 &&
            m[IDtoMoveIndex(m, 252)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
        {
            if (op.Ability?.Name.ToLower() != "inner focus")
            {
                if (battleScreen.FieldEffects.PokemonTurns.Opponent == 0)
                {
                    return ProduceOppStep(m, IDtoMoveIndex(m, 252));
                }
            }
        }

        // High-priority move when opponent is nearly fainted (<= 15%) (100%)
        if (op.HP <= (int)((op.MaxHP / 100.0f) * 15))
        {
            int chosenMove = MoveAI(m, Attack.AIField.HighPriority);
            if (chosenMove > -1)
            {
                if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                {
                    return ProduceOppStep(m, chosenMove);
                }
            }
        }

        // Attacking move when faster and opponent has low health (<= 30%) (75%)
        if (p.Speed > op.Speed && op.HP <= (int)((op.MaxHP / 100.0f) * 30))
        {
            int chosenMove = MoveAI(m, Attack.AIField.Damage);
            if (chosenMove > -1)
            {
                if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                {
                    return ProduceOppStep(m, chosenMove);
                }
            }
        }

        // Recover when HP < 50%
        if (p.HP <= (int)(p.MaxHP / 2.0f))
        {
            int chance = 50;
            if (op.Status == Pokemon.StatusProblems.Freeze ||
                op.Status == Pokemon.StatusProblems.Paralyzed ||
                op.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == true)
            {
                chance = 75;
            }
            if (op.Status == Pokemon.StatusProblems.Sleep)
            {
                chance = 100;
            }
            if (RPercent(chance) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.Healing);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Brick Break when opponent has Reflect or Light Screen up (80%)
        if (op.IsType(Element.Types.Ghost) == false)
        {
            if (battleScreen.FieldEffects.Reflect.Self > 0 ||
                battleScreen.FieldEffects.LightScreen.Self > 0)
            {
                if (RPercent(80) == true)
                {
                    int chosenMove = MoveAI(m, Attack.AIField.RemoveReflectLightscreen);
                    if (chosenMove > -1)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Status-inflicting moves (no substitute up, opponent has no status)
        if (battleScreen.FieldEffects.Substitute.Self == 0)
        {
            if (op.Status == Pokemon.StatusProblems.None)
            {
                // Paralyze (75%)
                if (op.Status != Pokemon.StatusProblems.Paralyzed &&
                    op.Type1.Type != Element.Types.Electric &&
                    op.Type2.Type != Element.Types.Electric &&
                    op.Ability?.Name.ToLower() != "limber")
                {
                    if (RPercent(75) == true)
                    {
                        int chosenMove = MoveAI(m, Attack.AIField.Paralysis);
                        if (chosenMove > -1)
                        {
                            if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                            {
                                return ProduceOppStep(m, chosenMove);
                            }
                        }
                    }
                }

                // Burn (75%)
                if (op.Status != Pokemon.StatusProblems.Burn &&
                    op.Type1.Type != Element.Types.Fire &&
                    op.Type2.Type != Element.Types.Fire &&
                    op.Ability?.Name.ToLower() != "water veil")
                {
                    if (RPercent(75) == true)
                    {
                        int chosenMove = MoveAI(m, Attack.AIField.Burn);
                        if (chosenMove > -1)
                        {
                            if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                            {
                                return ProduceOppStep(m, chosenMove);
                            }
                        }
                    }
                }

                // Sleep (75%)
                if (op.Status != Pokemon.StatusProblems.Sleep &&
                    op.Ability?.Name.ToLower() != "vital spirit" &&
                    op.Ability?.Name.ToLower() != "insomnia")
                {
                    if (RPercent(75) == true)
                    {
                        int chosenMove = MoveAI(m, Attack.AIField.Sleep);
                        if (chosenMove > -1)
                        {
                            if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                            {
                                return ProduceOppStep(m, chosenMove);
                            }
                        }
                    }
                }

                // Poison (50%)
                if (op.Status != Pokemon.StatusProblems.BadPoison &&
                    op.Status != Pokemon.StatusProblems.Poison &&
                    op.Type1.Type != Element.Types.Steel && op.Type1.Type != Element.Types.Poison &&
                    op.Type2.Type != Element.Types.Steel && op.Type2.Type != Element.Types.Poison &&
                    op.Ability?.Name.ToLower() != "immunity")
                {
                    if (RPercent(50) == true)
                    {
                        int chosenMove = MoveAI(m, Attack.AIField.Poison);
                        if (chosenMove > -1)
                        {
                            if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                            {
                                return ProduceOppStep(m, chosenMove);
                            }
                        }
                    }
                }
            }
        }

        // Confuse (75%)
        if (op.HasVolatileStatus(Pokemon.VolatileStatus.Confusion) == false &&
            op.Ability?.Name.ToLower() != "own tempo")
        {
            if (RPercent(75) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.Confusion);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Leech Seed (75%)
        if (op.IsType(Element.Types.Grass) == false &&
            battleScreen.FieldEffects.LeechSeed.Self == 0 &&
            HasMove(m, 73) == true &&
            m[IDtoMoveIndex(m, 73)].Disabled == 0 &&
            m[IDtoMoveIndex(m, 73)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
        {
            if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                m[IDtoMoveIndex(m, 73)].category != Attack.Categories.Status)
            {
                if (RPercent(75) == true)
                {
                    return ProduceOppStep(m, IDtoMoveIndex(m, 73));
                }
            }
        }

        // Focus Energy (50%)
        if (battleScreen.FieldEffects.FocusEnergy.Opponent == 0 &&
            HasMove(m, 116) == true &&
            m[IDtoMoveIndex(m, 116)].Disabled == 0 &&
            m[IDtoMoveIndex(m, 116)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
        {
            if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                m[IDtoMoveIndex(m, 116)].category != Attack.Categories.Status)
            {
                if (RPercent(50) == true)
                {
                    return ProduceOppStep(m, IDtoMoveIndex(m, 116));
                }
            }
        }

        // Sp. Atk boost when SpA > Atk, not boosted, SpA > SpD (50%)
        if (p.SpAttack > p.Attack && p.statSpAttack <= 0 && p.SpAttack > p.SpDefense)
        {
            if (RPercent(50) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.RaiseSpAttack);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Atk boost when Atk > SpA, not boosted, Atk > Def (50%)
        if (p.Attack > p.SpAttack && p.statAttack <= 0 && p.Attack > p.Defense)
        {
            if (RPercent(50) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.RaiseAttack);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Lower opp Def: Atk > SpA, Def > Atk, opp Def not lowered (50%)
        if (p.Attack > p.SpAttack && p.Defense > p.Attack && op.statDefense >= 0)
        {
            if (RPercent(50) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.LowerDefense);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Lower opp SpD: SpA > Atk, SpD > SpA, opp SpD not lowered (50%)
        if (p.SpAttack > p.Attack && p.SpDefense > p.SpAttack && op.statSpDefense >= 0)
        {
            if (RPercent(50) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.LowerSpDefense);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Stat-healing move when opp attacking stat < defensive stat (50%)
        if ((op.SpAttack > op.Attack && op.SpAttack < op.SpDefense) ||
            (op.Attack > op.SpAttack && op.Attack < op.Defense))
        {
            if (p.Status == Pokemon.StatusProblems.BadPoison || p.Status == Pokemon.StatusProblems.Burn ||
                p.Status == Pokemon.StatusProblems.Freeze || p.Status == Pokemon.StatusProblems.Paralyzed ||
                p.Status == Pokemon.StatusProblems.Poison || p.Status == Pokemon.StatusProblems.Sleep)
            {
                int chosenMove = MoveAI(m, Attack.AIField.CureStatus);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Boost evasion if not boosted (50%)
        if (p.evasion <= 0)
        {
            if (RPercent(50) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.RaiseEvasion);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Boost accuracy if not boosted (75%)
        if (p.accuracy <= 0)
        {
            if (RPercent(75) == true)
            {
                int chosenMove = MoveAI(m, Attack.AIField.RaiseAccuracy);
                if (chosenMove > -1)
                {
                    if (CheckForTypeIneffectiveness(battleScreen, m, chosenMove) == true)
                    {
                        return ProduceOppStep(m, chosenMove);
                    }
                }
            }
        }

        // Light Screen / Reflect (75%)
        if (RPercent(75) == true)
        {
            if (HasMove(m, 113) == true && op.SpAttack > op.Attack &&
                battleScreen.FieldEffects.LightScreen.Opponent == 0 &&
                m[IDtoMoveIndex(m, 113)].Disabled == 0 &&
                m[IDtoMoveIndex(m, 113)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
            {
                if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                    m[IDtoMoveIndex(m, 113)].category != Attack.Categories.Status)
                {
                    return ProduceOppStep(m, IDtoMoveIndex(m, 113));
                }
            }
            if (HasMove(m, 115) == true && op.Attack > op.SpAttack &&
                battleScreen.FieldEffects.Reflect.Opponent == 0 &&
                m[IDtoMoveIndex(m, 115)].Disabled == 0 &&
                m[IDtoMoveIndex(m, 115)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
            {
                if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                    m[IDtoMoveIndex(m, 115)].category != Attack.Categories.Status)
                {
                    return ProduceOppStep(m, IDtoMoveIndex(m, 115));
                }
            }
        }

        // Defense Curl + Rollout combo
        if (HasMove(m, 205) == true && HasMove(m, 111) == true)
        {
            if (battleScreen.FieldEffects.DefenseCurl.Opponent == 0 &&
                m[IDtoMoveIndex(m, 111)].Disabled == 0 &&
                m[IDtoMoveIndex(m, 111)].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
            {
                if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                    m[IDtoMoveIndex(m, 111)].category != Attack.Categories.Status)
                {
                    return ProduceOppStep(m, IDtoMoveIndex(m, 111));
                }
            }
            else
            {
                if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                    m[IDtoMoveIndex(m, 205)].category != Attack.Categories.Status)
                {
                    if (m[IDtoMoveIndex(m, 205)].Disabled == 0)
                    {
                        return ProduceOppStep(m, IDtoMoveIndex(m, 205));
                    }
                }
            }
        }

        // ---------------- Choose best attacking move ----------------
        Dictionary<int, int> attackDic = [];
        for (int i = 0; i < m.Count; i++)
        {
            if (MoveHasAIField(m[i], Attack.AIField.Damage) == true &&
                m[i].Disabled == 0 &&
                m[i].Equals(battleScreen.FieldEffects.TormentMove.Opponent) == false)
            {
                if (battleScreen.FieldEffects.Taunt.Opponent == 0 ||
                    m[i].category != Attack.Categories.Status)
                {
                    attackDic.Add(i, 0);
                }
            }
        }

        if (attackDic.Count > 0)
        {
            List<int> attackKeys = new List<int>(attackDic.Keys);
            for (int i = 0; i < attackKeys.Count; i++)
            {
                int key = attackKeys[i];
                Attack cMove = m[key];
                int value = 0;

                float effectiveness = BattleCalculation.CalculateEffectiveness(false, cMove, battleScreen);

                // Base power weighted by effectiveness
                if (effectiveness != 0.0f)
                {
                    value += (int)(cMove.GetBasePower(false, battleScreen) * effectiveness);
                }

                // Accuracy
                value += cMove.GetAccuracy(false, battleScreen);

                // STAB
                if (p.IsType(cMove.type.Type) == true)
                {
                    value += 35;
                }

                // Attack stat stages
                if (cMove.category == Attack.Categories.Physical)
                {
                    value += p.statAttack * 15;
                }
                else
                {
                    value += p.statSpAttack * 15;
                }
                if (cMove.useOpponentDefense == true)
                {
                    if (cMove.category == Attack.Categories.Physical)
                    {
                        value -= op.statDefense * 15;
                    }
                    else
                    {
                        value -= p.statSpDefense * 15;
                    }
                }

                // Dry Skin: fire moves preferred
                if (cMove.GetAttackType(false, battleScreen).Type == Element.Types.Fire &&
                    op.Ability?.Name.ToLower() == "dry skin")
                {
                    value += 25;
                }

                // Never-miss moves preferred when accuracy is low or evasion is high
                if (cMove.Accuracy == 0 ||
                    cMove.GetUseAccEvasion(false, battleScreen) == false)
                {
                    if (p.accuracy < 0)
                    {
                        value += (int)Math.Abs(p.accuracy * 15);
                    }
                    if (op.evasion > 0)
                    {
                        value += op.evasion * 15;
                    }
                }

                // Selfdestruct / Explosion: rarely choose if other options exist
                if (cMove.ID == 120 || cMove.ID == 153)
                {
                    if (Core.Random.Next(0, 8) != 0)
                    {
                        value = Core.Random.Next(50, 100);
                    }
                }

                // Add randomness
                value += Core.Random.Next(-35, 35);

                if (value < 0)
                {
                    value = 0;
                }

                // Zero out if immune
                if (effectiveness == 0.0f)
                {
                    value = 0;
                }

                // Ability absorptions: set value to 0
                Element.Types attackType = cMove.GetAttackType(false, battleScreen).Type;
                if (attackType == Element.Types.Water && op.Ability?.Name.ToLower() == "water absorb") { value = 0; }
                if (attackType == Element.Types.Electric && op.Ability?.Name.ToLower() == "volt absorb") { value = 0; }
                if (attackType == Element.Types.Electric && op.Ability?.Name.ToLower() == "motor drive") { value = 0; }
                if (attackType == Element.Types.Grass && op.Ability?.Name.ToLower() == "sap sipper") { value = 0; }
                if (attackType == Element.Types.Fire && op.Ability?.Name.ToLower() == "flash fire") { value = 0; }
                if (attackType == Element.Types.Water && op.Ability?.Name.ToLower() == "dry skin") { value = 0; }

                // Moves to never use in certain conditions
                if (cMove.ID == 150) { value = 0; } // Splash
                if (cMove.ID == 214 && p.Status != Pokemon.StatusProblems.Sleep) { value = 0; } // Sleep Talk
                if (cMove.ID == 171 && op.Status != Pokemon.StatusProblems.Sleep) { value = 0; } // Nightmare
                if (cMove.ID == 138 && op.Status != Pokemon.StatusProblems.Sleep) { value = 0; } // Dream Eater
                if (cMove.ID == 485 && op.IsType(p.Type1.Type) == false || op.IsType(p.Type2.Type) == false) // Synchronoise
                {
                    value = 0;
                }

                // Last Resort: require all other moves used first
                bool usedMoves = true;
                List<int> allUsedMoves = battleScreen.FieldEffects.UsedMoves.Opponent;
                foreach (int moveID in attackDic.Keys)
                {
                    if (allUsedMoves.Contains(m[moveID].ID) == false && m[moveID].ID != 387)
                    {
                        usedMoves = false;
                        break;
                    }
                }
                if (usedMoves == false)
                {
                    value = 0;
                }

                attackDic[key] = value;
            }

            // Pick highest-valued move (up to 4 candidates)
            List<int> finalKeys = new List<int>(attackDic.Keys);
            List<int> finalVals = new List<int>(attackDic.Values);
            int index = 0;
            if (finalVals.Count > 1 && finalVals[1] > finalVals[index]) { index = 1; }
            if (finalVals.Count > 2 && finalVals[2] > finalVals[index]) { index = 2; }
            if (finalVals.Count > 3 && finalVals[3] > finalVals[index]) { index = 3; }

            return ProduceOppStep(m, finalKeys[index]);
        }

        // Fallback: try any attacking move
        int chosenAttackMove = MoveAI(m, Attack.AIField.Damage);
        if (chosenAttackMove > -1)
        {
            if (battleScreen.Trainer!.AILevel >= 1)
            {
                if (CheckForTypeIneffectiveness(battleScreen, m, chosenAttackMove) == true)
                {
                    return ProduceOppStep(m, chosenAttackMove);
                }
            }
            else
            {
                return ProduceOppStep(m, chosenAttackMove);
            }
        }

        // Fallback: random valid move
        if (battleScreen.Trainer!.AILevel >= 0)
        {
            List<int> availableAttacks = [];
            for (int i = 0; i < m.Count; i++)
            {
                availableAttacks.Add(i);
            }
            int oppAttackChoice = Core.Random.Next(0, availableAttacks.Count);
            bool ready = false;
            while (ready == false)
            {
                // Skip infatuation moves when inappropriate (75% chance)
                if (availableAttacks.Count > 1)
                {
                    if (MoveHasAIField(battleScreen.OpponentPokemon!.attacks[oppAttackChoice], Attack.AIField.Infatuation) == true)
                    {
                        if (battleScreen.SelfPokemon!.HasVolatileStatus(Pokemon.VolatileStatus.Infatuation) == true ||
                            battleScreen.OpponentPokemon!.Gender == Pokemon.Genders.Genderless ||
                            battleScreen.OpponentPokemon!.Gender == battleScreen.SelfPokemon!.Gender)
                        {
                            if (RPercent(75) == true)
                            {
                                availableAttacks.Remove(oppAttackChoice);
                                oppAttackChoice = availableAttacks[Core.Random.Next(0, availableAttacks.Count)];
                            }
                        }
                    }
                }

                if (m[oppAttackChoice].Equals(battleScreen.FieldEffects.TormentMove.Opponent) ||
                    m[oppAttackChoice].Disabled > 0 ||
                    (battleScreen.FieldEffects.Taunt.Opponent > 0 &&
                     battleScreen.OpponentPokemon!.attacks[oppAttackChoice].category == Attack.Categories.Status))
                {
                    availableAttacks.Remove(oppAttackChoice);
                    if (availableAttacks.Count > 0)
                    {
                        oppAttackChoice = availableAttacks[Core.Random.Next(0, availableAttacks.Count)];
                    }
                    else
                    {
                        return new BattleRoundConst
                        {
                            StepType = BattleRoundConst.StepTypes.Move,
                            Argument = Attack.GetAttackByID(165)
                        };
                    }
                }
                else
                {
                    ready = true;
                }
            }
            return ProduceOppStep(m, oppAttackChoice);
        }

        // Catch-all: return Struggle
        return new BattleRoundConst
        {
            StepType = BattleRoundConst.StepTypes.Move,
            Argument = Attack.GetAttackByID(165)
        };
    }

    private static bool HasOtherAttackingMoveThanExplosion(List<Attack> m, int leaveOutIndex)
    {
        for (int i = 0; i < m.Count; i++)
        {
            if (i != leaveOutIndex)
            {
                if (m[i].ID != 121 && m[i].ID != 153)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool CheckForTypeIneffectiveness(BattleScreen battleScreen, List<Attack> m, int i)
    {
        Attack move = m[i];
        if (move.immunityAffected == true)
        {
            float effectiveness = BattleCalculation.CalculateEffectiveness(false, move, battleScreen);
            if (effectiveness == 0.0f)
            {
                return false;
            }
        }
        return true;
    }

    private static bool RPercent(int p)
    {
        if (p >= 100)
        {
            return true;
        }
        if (Core.Random.Next(0, 100) < p)
        {
            return true;
        }
        return false;
    }

    private static int MoveAI(List<Attack> m, Attack.AIField aiType)
    {
        List<int> validMoves = [];
        Screen battleScreen = Core.CurrentScreen!;
        while (battleScreen.Identification != Screen.Identifications.BattleScreen)
        {
            battleScreen = battleScreen.PreScreen!;
        }
        BattleScreen bs = (BattleScreen)battleScreen;
        for (int i = 0; i < m.Count; i++)
        {
            if (m[i].Disabled == 0 &&
                m[i].Equals(bs.FieldEffects.TormentMove.Opponent) == false)
            {
                if (bs.FieldEffects.Taunt.Opponent == 0 ||
                    m[i].category != Attack.Categories.Status)
                {
                    if (m[i].aiField1 == aiType || m[i].aiField2 == aiType || m[i].aiField3 == aiType)
                    {
                        validMoves.Add(i);
                    }
                }
            }
        }
        if (validMoves.Count > 0)
        {
            return validMoves[Core.Random.Next(0, validMoves.Count)];
        }
        return -1;
    }

    private static bool MoveHasAIField(Attack m, Attack.AIField aiType)
    {
        return m.aiField1 == aiType || m.aiField2 == aiType || m.aiField3 == aiType;
    }

    private static bool HasMove(List<Attack> m, int id)
    {
        foreach (Attack move in m)
        {
            if (move.ID == id)
            {
                return true;
            }
        }
        return false;
    }

    private static int IDtoMoveIndex(List<Attack> m, int id)
    {
        for (int i = 0; i < m.Count; i++)
        {
            if (m[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }

    // ---- ProduceSteps ----

    private static BattleRoundConst ProduceOppStep(List<Attack> m, int i)
    {
        while (i > m.Count - 1)
        {
            i -= 1;
        }
        if (m.Count == 0)
        {
            throw new Exception("An empty move array was passed in!");
        }
        return new BattleRoundConst
        {
            StepType = BattleRoundConst.StepTypes.Move,
            Argument = m[i]
        };
    }

    private static BattleRoundConst ProduceOppStep(int itemID, int target)
    {
        return new BattleRoundConst
        {
            StepType = BattleRoundConst.StepTypes.Item,
            Argument = itemID.ToString() + "," + target.ToString()
        };
    }

    private static BattleRoundConst ProduceOppStep(int switchID)
    {
        return new BattleRoundConst
        {
            StepType = BattleRoundConst.StepTypes.Switch,
            Argument = switchID.ToString()
        };
    }

    // ---- Item helpers ----

    private static bool TrainerHasItem(int itemID, BattleScreen battleScreen)
    {
        foreach (Item item in battleScreen.Trainer!.Items)
        {
            if (item.ID == itemID)
            {
                return true;
            }
        }
        return false;
    }

    private static int GetBestPotion(BattleScreen battleScreen)
    {
        List<int> potionRange = [18, 17, 16, 15, 14];
        int bestPotion = -1;
        foreach (Item item in battleScreen.Trainer!.Items)
        {
            if (potionRange.Contains(item.ID) == true)
            {
                if (potionRange.IndexOf(item.ID) > bestPotion)
                {
                    bestPotion = potionRange.IndexOf(item.ID);
                }
            }
        }
        if (bestPotion == -1)
        {
            return -1;
        }
        else
        {
            return potionRange[bestPotion];
        }
    }

    private static int GetBestRevive(BattleScreen battleScreen)
    {
        foreach (Item item in battleScreen.Trainer!.Items)
        {
            if (item.ID == 40)
            {
                return 40;
            }
        }
        return 39;
    }

    private static int GetPotionHealHP(Pokemon p, int itemID)
    {
        switch (itemID)
        {
            case 18:
                return 20;
            case 17:
                return 50;
            case 16:
                return 200;
            case 15:
                return p.MaxHP;
            case 14:
                return p.MaxHP;
        }
        return 0;
    }

    private static int GetPokemonValue(BattleScreen battleScreen, Pokemon p)
    {
        int total = 0;
        foreach (Pokemon teamP in battleScreen.Trainer!.Pokemons)
        {
            total += teamP.baseHP + teamP.baseAttack + teamP.baseDefense +
                     teamP.baseSpAttack + teamP.baseSpDefense + teamP.baseSpeed;
        }
        int pTotal = p.baseHP + p.baseAttack + p.baseDefense +
                     p.baseSpAttack + p.baseSpDefense + p.baseSpeed;
        int percent = (int)((pTotal / (float)total) * 100);
        return percent;
    }
}
