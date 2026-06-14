using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Fighting;

public class CircleThrow : Attack
{
    public CircleThrow()
    {
        // #Definitions
        type = new Element(Element.Types.Fighting);
        ID = 509;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 60;
        Accuracy = 90;
        category = Categories.Physical;
        contestCategory = ContestCategories.Cool;
        Name = Localization.GetString($"move_name_{ID}", "Circle Throw");
        Description = "The target is thrown, and a different Pokémon is dragged out. In the wild, this ends a battle against a single Pokémon.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = -6;
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
        hasSecondaryEffect = false;
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
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        Pokemon p = battleScreen.SelfPokemon;  // p is the phazed pokemon
        if (own == true)
        {
            p = battleScreen.OpponentPokemon;
        }

        // Not fainted:
        if (p.HP > 0 && p.Status != Pokemon.StatusProblems.Fainted)
        {
            int substitude = battleScreen.FieldEffects.Substitute.Self;
            if (own == true)
            {
                substitude = battleScreen.FieldEffects.Substitute.Opponent;
            }

            // substitute:
            if (substitude <= 0)
            {

                // suction cups ability:
                if (p.Ability.Name.ToLower() != "suction cups")
                {
                    int ingrain = battleScreen.FieldEffects.Ingrain.Self;
                    if (own == true)
                    {
                        ingrain = battleScreen.FieldEffects.Ingrain.Opponent;
                    }

                    // check ingrain set up:
                    if (ingrain <= 0)
                    {

                        if (BattleCalculation.CanSwitch(battleScreen, own == false) == true)
                        {
                             battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " got thrown away!"));
                            if (battleScreen.IsPVPBattle == true || battleScreen.IsTrainerBattle == true || battleScreen.IsRemoteBattle == true)
                            {
                                // trainer battle
                                if (own == true)
                                {
                                    if (battleScreen.Trainer.CountUseablePokemon > 1)
                                    {
                                        int i = Core.Random.Next(0, battleScreen.Trainer.Pokemons.Count);
                                        while (battleScreen.Trainer.Pokemons[i].Status == Pokemon.StatusProblems.Fainted || battleScreen.OpponentPokemonIndex == i || battleScreen.Trainer.Pokemons[i].HP <= 0)
                                        {
                                            i = Core.Random.Next(0, battleScreen.Trainer.Pokemons.Count - 1);
                                        }
                                        battleScreen.Battle.SwitchOutOpp(battleScreen, i, "", false);
                                    }
                                    else
                                    {
                                        battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                                    }
                                }
                                else
                                {
                                    if (Core.Player.CountFightablePokemon > 1)
                                    {
                                         int i = Core.Random.Next(0, Core.Player.Pokemons.Count);
                                        while (Core.Player.Pokemons[i].Status == Pokemon.StatusProblems.Fainted || battleScreen.SelfPokemonIndex == i || Core.Player.Pokemons[i].HP <= 0)
                                        {
                                            i = Core.Random.Next(0, Core.Player.Pokemons.Count - 1);
                                        }
                                        battleScreen.Battle.SwitchOutOwn(battleScreen, i, -1);
                                    }
                                    else
                                    {
                                        battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                                    }
                                }
                            }
                            else
                            {
                                // wild battle
                                battleScreen.BattleQuery.Add(new EndBattleQueryObject(false));
                            }
                        }
                        else
                        {
                            battleScreen.BattleQuery.Add(new TextQueryObject(Name + " failed!"));
                        }
                    }
                }
            }
        }
    }

}
