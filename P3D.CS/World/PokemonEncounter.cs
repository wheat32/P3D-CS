using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D;

public class PokemonEncounter
{
    private Level _levelReference;

    public PokemonEncounter(Level levelReference)
    {
        _levelReference = levelReference;
    }

    public void TryEncounterWildPokemon(Vector3 position, Spawner.EncounterMethods method, String pokeFile)
    {
        if (_levelReference.WalkedSteps > 3)
        {
            if (Screen.Level.CheckTrainerSights() == true)
            {
                return;
            }
            if (pokeFile.Equals("") == true)
            {
                pokeFile = _levelReference.LevelFile.Remove(_levelReference.LevelFile.Length - 4, 4) + ".poke";
            }

            if (System.IO.File.Exists(GameModeManager.GetPokeFilePath(pokeFile)) == true)
            {
                float encounterRate = 1.0f;
                int minTileValue = 0;
                switch (method)
                {
                    case Spawner.EncounterMethods.Land:
                        if (Screen.Level.WildPokemonFloor == true && Screen.Level.Surfing == false)
                        {
                            minTileValue = 15;
                        }
                        else
                        {
                            minTileValue = 25;
                        }
                        break;
                    case Spawner.EncounterMethods.Surfing:
                        minTileValue = 15;
                        break;
                }
                if (Core.Player.IsRunning() == true)
                {
                    encounterRate *= 1.5f;
                }
                if (Core.Player.Pokemons.Count > 0)
                {
                    Pokemon p = Core.Player.Pokemons[0];

                    if (p.Ability.Name.ToLower().Equals("arena trap") == true ||
                        p.Ability.Name.ToLower().Equals("illuminate") == true ||
                        p.Ability.Name.ToLower().Equals("no guard") == true ||
                        p.Ability.Name.ToLower().Equals("swarm") == true)
                    {
                        encounterRate *= 2.0f;
                    }

                    if (p.Ability.Name.ToLower().Equals("intimidate") == true ||
                        p.Ability.Name.ToLower().Equals("keen eye") == true ||
                        p.Ability.Name.ToLower().Equals("quick feet") == true ||
                        p.Ability.Name.ToLower().Equals("stench") == true ||
                        p.Ability.Name.ToLower().Equals("white smoke") == true)
                    {
                        encounterRate *= 0.5f;
                    }

                    if (_levelReference.WeatherType == 7 && p.Ability.Name.ToLower().Equals("sand veil") == true)
                    {
                        if (Core.Random.Next(0, 100) < 50)
                        {
                            return;
                        }
                    }

                    if (p.Ability.Name.ToLower().Equals("snow cloak") == true)
                    {
                        if (_levelReference.WeatherType == 2 || _levelReference.WeatherType == 9)
                        {
                            if (Core.Random.Next(0, 100) < 50)
                            {
                                return;
                            }
                        }
                    }
                }

                int minEncounterValue = (int)(encounterRate * minTileValue);
                int randomValue = Core.Random.Next(0, 255);

                if (randomValue <= minEncounterValue)
                {
                    if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
                    {
                        if (((OverworldCamera)Screen.Camera)._debugWalk == true)
                        {
                            return;
                        }
                    }

                    _levelReference.WalkedSteps = 0;

                    _levelReference.PokemonEncounterData.Position = position;
                    _levelReference.PokemonEncounterData.EncounteredPokemon = true;
                    _levelReference.PokemonEncounterData.Method = method;
                    _levelReference.PokemonEncounterData.PokeFile = pokeFile;
                }
            }
        }
    }

    public void TriggerBattle()
    {
        if (_levelReference.PokemonEncounterData.EncounteredPokemon == true &&
            Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
        {
            if (Core.Player.Pokemons.Count == 0)
            {
                if (Screen.Level.BlackOutScript.Equals("") == false)
                {
                    ((OverworldScreen)Core.CurrentScreen).ActionScript.StartScript(Screen.Level.BlackOutScript, 0, false);
                }
                else
                {
                    Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new BlackOutScreen(Core.CurrentScreen), Microsoft.Xna.Framework.Color.Black, false));
                }
                _levelReference.PokemonEncounterData.EncounteredPokemon = false;
                return;
            }
            else
            {
                if (Screen.Camera.Position.X == _levelReference.PokemonEncounterData.Position.X &&
                    Screen.Camera.Position.Z == _levelReference.PokemonEncounterData.Position.Z)
                {
                    _levelReference.PokemonEncounterData.EncounteredPokemon = false;
                    Screen.Camera.StopMovement();

                    Pokemon pokemon = Spawner.GetPokemon(Screen.Level.LevelFile, _levelReference.PokemonEncounterData.Method, true, _levelReference.PokemonEncounterData.PokeFile);

                    if (pokemon != null &&
                        ((OverworldScreen)Core.CurrentScreen).TrainerEncountered == false &&
                        ((OverworldScreen)Core.CurrentScreen).ActionScript.IsReady == true)
                    {
                        Screen.Level.RouteSign.Hide();

                        if (Core.Player.RepelSteps > 0)
                        {
                            Pokemon walkPokemon = Core.Player.GetWalkPokemon();
                            if (walkPokemon != null)
                            {
                                if (walkPokemon.Level >= pokemon.Level)
                                {
                                    if (BattleScreen.RoamingBattle == true)
                                    {
                                        BattleScreen.RoamingBattle = false;
                                        BattleScreen.RoamingPokemonStorage = null;
                                    }
                                    return;
                                }
                            }
                        }

                        if (Core.Player.Pokemons[0].Level >= pokemon.Level)
                        {
                            if (Core.Player.Pokemons[0].Item != null)
                            {
                                if (Core.Player.Pokemons[0].Item.IsGameModeItem == false &&
                                    Core.Player.Pokemons[0].Item.ID == 94)
                                {
                                    if (Core.Random.Next(0, 3) == 0)
                                    {
                                        if (BattleScreen.RoamingBattle == true)
                                        {
                                            BattleScreen.RoamingBattle = false;
                                            BattleScreen.RoamingPokemonStorage = null;
                                        }
                                        return;
                                    }
                                }
                            }
                        }

                        if (Core.Player.Pokemons[0].Level >= pokemon.Level)
                        {
                            if (Core.Player.Pokemons[0].Item != null)
                            {
                                if (Core.Player.Pokemons[0].Item.IsGameModeItem == false &&
                                    Core.Player.Pokemons[0].Item.ID == 291)
                                {
                                    if (Core.Random.Next(0, 3) == 0)
                                    {
                                        if (BattleScreen.RoamingBattle == true)
                                        {
                                            BattleScreen.RoamingBattle = false;
                                            BattleScreen.RoamingPokemonStorage = null;
                                        }
                                        return;
                                    }
                                }
                            }
                        }

                        String dexID = PokemonForms.GetPokemonDataFileName(pokemon.Number, pokemon.AdditionalData);
                        if (dexID.Contains("_") == false)
                        {
                            if (PokemonForms.GetAdditionalDataForms(pokemon.Number) != null &&
                                PokemonForms.GetAdditionalDataForms(pokemon.Number).Contains(pokemon.AdditionalData) == true)
                            {
                                dexID = pokemon.Number + ";" + pokemon.AdditionalData;
                            }
                            else
                            {
                                dexID = pokemon.Number.ToString();
                            }
                        }
                        Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 1);

                        int introType = Core.Random.Next(0, 10);
                        if (BattleScreen.RoamingBattle == true)
                        {
                            introType = 12;
                        }

                        BattleScreen b = new BattleScreen(pokemon, Core.CurrentScreen, _levelReference.PokemonEncounterData.Method);
                        Core.SetScreen(new BattleIntroScreen(Core.CurrentScreen, b, introType));
                    }
                }
            }
        }
    }
}
