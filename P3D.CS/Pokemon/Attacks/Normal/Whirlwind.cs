using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Normal;

public class Whirlwind : Attack
{
    public Whirlwind()
    {
        // #Definitions
        type = new Element(Element.Types.Normal);
        ID = 18;
        originalPP = 20;
        currentPP = 20;
        maxPP = 20;
        Power = 0;
        Accuracy = 0;
        category = Categories.Status;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Whirlwind");
        Description = "The target is blown away, and a different Pokémon is dragged out. In the wild, this ends a battle against a single Pokémon.";
        criticalChance = 0;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = -6;
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
        isWindMove = true;
        isDamagingMove = false;
        isProtectMove = false;

        isAffectedBySubstitute = true;
        isOneHitKOMove = false;
        isWonderGuardAffected = false;
        canHitInMidAir = true;
        useAccEvasion = false;
        // #End

        aiField1 = AIField.Support;
        aiField2 = AIField.Nothing;
    }

    public override void MoveHits(bool own, BattleScreen battleScreen)
    {
        // end wild battles when you use it (if your level is higher)
        // when used against trainer (or you), the move will drag out another Pokémon.
        // check for canswitch

        Pokemon p = battleScreen.SelfPokemon;
        if (own == true)
        {
            p = battleScreen.OpponentPokemon;
        }

        if (BattleCalculation.CanSwitch(battleScreen, own == false) == true)
        {
            battleScreen.BattleQuery.Add(new TextQueryObject(p.GetDisplayName() + " got blown away!"));
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
                            i = Core.Random.Next(0, battleScreen.Trainer.Pokemons.Count);
                        }
                        battleScreen.Battle.SwitchOutOpponent(battleScreen, i, "", false);
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
                            i = Core.Random.Next(0, Core.Player.Pokemons.Count);
                        }
                        battleScreen.Battle.SwitchOutSelf(battleScreen, i, -1);
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
