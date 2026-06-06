using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.BattleSystem.Moves.Dark;

public class BeatUp : Attack
{
    public BeatUp()
    {
        // #Definitions
        type = new Element(Element.Types.Dark);
        ID = 251;
        originalPP = 10;
        currentPP = 10;
        maxPP = 10;
        Power = 0;
        Accuracy = 100;
        category = Categories.Physical;
        contestCategory = ContestCategories.Smart;
        Name = Localization.GetString($"move_name_{ID}", "Beat Up");
        Description = "The user gets all party Pokémon to attack the target. The more party Pokémon, the greater the number of attacks.";
        criticalChance = 1;
        isHMMove = false;
        target = Targets.OneAdjacentTarget;
        priority = 0;
        timesToAttack = 1;
        // #End

        // #SpecialDefinitions
        makesContact = false;
        protectAffected = true;
        magicCoatAffected = false;
        snatchAffected = false;
        mirrorMoveAffected = true;
        kingsrockAffected = true;
        counterAffected = true;

        disabledWhileGravity = false;
        useEffectiveness = true;
        immunityAffected = false;
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

    public override int GetTimesToAttack(bool own, BattleScreen battleScreen)
    {
        int i = 0;
        if (own == true)
        {
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                if (p.Status != Pokemon.StatusProblems.Fainted)
                {
                    i += 1;
                }
            }
        }
        else if (battleScreen.IsTrainerBattle == true)
        {
            foreach (Pokemon p in battleScreen.Trainer.Pokemons)
            {
                if (p.Status != Pokemon.StatusProblems.Fainted)
                {
                    i += 1;
                }
            }
        }
        else
        {
            i += 1;
        }
        return i;
    }

    public override int GetBasePower(bool own, BattleScreen battleScreen)
    {
        double avgTeamBaseAttack = 0.0D;
        int pokemonCounter = 0;
        if (own == true)
        {
            foreach (Pokemon pokemon in Core.Player.Pokemons)
            {
                if ((pokemon.IsEgg == false) && pokemon.Status != Pokemon.StatusProblems.Fainted && pokemon.HP > 0)
                {
                    avgTeamBaseAttack += (pokemon.BaseAttack / 10);
                    pokemonCounter += 1;
                }
            }
        }
        else if (battleScreen.IsTrainerBattle == true)
        {
            foreach (Pokemon pokemon in battleScreen.Trainer.Pokemons)
            {
                if ((pokemon.IsEgg == false) && pokemon.Status != Pokemon.StatusProblems.Fainted && pokemon.HP > 0)
                {
                    avgTeamBaseAttack += (pokemon.BaseAttack / 10);
                    pokemonCounter += 1;
                }
            }
        }
        else
        {
            avgTeamBaseAttack += (battleScreen.OpponentPokemon.BaseAttack / 10);
            pokemonCounter += 1;
        }
        if (pokemonCounter != 0)
        {
            avgTeamBaseAttack /= pokemonCounter;
        }
        else
        {
            avgTeamBaseAttack = 10;  // should never meet this case.
        }
        return (int)(avgTeamBaseAttack) + 5;
    }

}
