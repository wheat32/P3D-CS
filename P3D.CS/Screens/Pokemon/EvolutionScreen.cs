using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class EvolutionScreen : Screen
{
    public List<int> PokeList = new List<int>();
    private Pokemon _currentPokemon;
    private Pokemon _evolvedPokemon;

    private bool _evolutionReady = false;
    private bool _evolutionStarted = false;
    private bool _evolved = false;
    private bool _brokeEvolution = false;
    private List<BattleSystem.Attack> _attackLearnList = new List<BattleSystem.Attack>();
    private bool _canEnd = false;

    private String _evolutionArg = String.Empty;
    private EvolutionCondition.EvolutionTrigger _evolutionTrigger;
    private bool _fromBattle = false;

    private List<Spark> _sparks = new List<Spark>();

    public EvolutionScreen(Screen currentScreen, List<int> evolvePokemonIndices, String evolutionArg,
                           EvolutionCondition.EvolutionTrigger evolutionTrigger, bool fromBattle = false)
    {
        Identification = Identifications.EvolutionScreen;
        PlayerStatistics.Track("Evolutions", 1);

        _attackLearnList.Clear();
        PreScreen = currentScreen;
        _fromBattle = fromBattle;
        _canEnd = false;
        foreach (int i in evolvePokemonIndices)
            PokeList.Add(i);

        _evolutionArg = evolutionArg;
        _evolutionTrigger = evolutionTrigger;

        _currentPokemon = Core.Player.Pokemons[PokeList[0]];

        EvolvePokemon();

        SavedMusic = MusicManager.CurrentSong.Name;
        MusicManager.PlayNoMusic();
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), Color.Black);
        Texture2D t = _currentPokemon.GetTexture(true);
        Vector2 size = new Vector2((int)MathHelper.Min(t.Width * 3, 288), (int)MathHelper.Min(t.Height * 3, 288));

        if (_evolved == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture(@"GUI\Evolution\Light"),
                new Rectangle((int)(Core.windowSize.Width / 2 - size.X), (int)(Core.windowSize.Height / 2 - size.Y),
                    (int)(size.X * 2), (int)(size.Y * 2)), Color.White);

        if (_evolved == true)
        {
            t = _evolvedPokemon.GetTexture(true);
            size = new Vector2((int)MathHelper.Min(t.Width * 3, 288), (int)MathHelper.Min(t.Height * 3, 288));
        }
        Core.SpriteBatch.Draw(t, new Rectangle(
            (int)(Core.windowSize.Width / 2 - size.X / 2),
            (int)(Core.windowSize.Height / 2 - size.Y / 2),
            (int)size.X, (int)size.Y), Color.White);

        foreach (Spark spark in _sparks)
            spark.Draw();

        TextBox.Draw();
    }

    public override void Update()
    {
        TextBox.Update();

        if (_evolutionStarted == false)
        {
            SoundManager.PlayPokemonCry(_currentPokemon.Number, PokemonForms.GetCrySuffix(_currentPokemon));
            TextBox.Show(Localization.GetString("evolution_PokemonIsEvolving", "What?*[POKEMONNAME] is evolving!").Replace("[POKEMONNAME]", _currentPokemon.GetDisplayName()), [], false, false);
            _evolutionStarted = true;
            for (int i = 0; i <= Core.Random.Next(200, 250); i++)
                _sparks.Add(new Spark());
        }
        else
        {
            if (_evolutionReady == false && TextBox.Showing == false)
            {
                MusicManager.Play("evolution", true);

                if (_evolved == false)
                {
                    bool allReady = true;

                    foreach (Spark spark in _sparks)
                    {
                        spark.Update();
                        if (spark.IsReady == false)
                            allReady = false;
                    }

                    if (allReady == true)
                    {
                        _evolved = true;
                        foreach (Spark spark in _sparks)
                            spark.doGrow = true;
                    }
                    else
                    {
                        if (Controls.Dismiss(true, true) == true)
                        {
                            _sparks.Clear();
                            _evolutionReady = true;
                            _brokeEvolution = true;
                            TextBox.Show(Localization.GetString("evolution_PokemonStoppedEvolving", "Huh? [POKEMONNAME]~stopped evolving!").Replace("[POKEMONNAME]", _currentPokemon.GetDisplayName()), [], false, false);
                        }
                    }
                }
                else
                {
                    bool allReady = true;

                    foreach (Spark spark in _sparks)
                    {
                        spark.Update();
                        if (spark.IsReady == false || spark.doGrow == true)
                            allReady = false;
                    }

                    if (allReady == true)
                    {
                        _sparks.Clear();
                        int type = 2;
                        if (_evolvedPokemon.IsShiny == true)
                            type = 3;

                        String dexID = PokemonForms.GetPokemonDataFileName(_evolvedPokemon.Number, _evolvedPokemon.AdditionalData);
                        if (dexID.Contains("_") == false)
                        {
                            if (PokemonForms.GetAdditionalDataForms(_evolvedPokemon.Number) != null &&
                                PokemonForms.GetAdditionalDataForms(_evolvedPokemon.Number)!.Contains(_evolvedPokemon.AdditionalData) == true)
                                dexID = _evolvedPokemon.Number + ";" + _evolvedPokemon.AdditionalData;
                            else
                                dexID = _evolvedPokemon.Number.ToString();
                        }

                        Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, type);

                        _evolvedPokemon.PlayCry();
                        SoundManager.PlaySound("success", true);

                        _evolutionReady = true;
                        String t = Localization.GetString("evolution_PokemonEvolvedInto", "Congratulations!*Your [POKEMONNAME]~evolved into [EVOLUTIONNAME]!").Replace("[POKEMONNAME]", _currentPokemon.GetDisplayName()).Replace("[EVOLUTIONNAME]", _evolvedPokemon.GetName());
                        if (_evolvedPokemon.attackLearns.ContainsKey(_evolvedPokemon.Level) == true)
                        {
                            List<BattleSystem.Attack> aList = _evolvedPokemon.attackLearns[_evolvedPokemon.Level];
                            for (int a = 0; a <= aList.Count - 1; a++)
                            {
                                if (_evolvedPokemon.KnowsMove(aList[a]) == false)
                                {
                                    if (_evolvedPokemon.attacks.Count == 4)
                                    {
                                        _attackLearnList.Add(aList[a]);
                                    }
                                    else
                                    {
                                        _evolvedPokemon.attacks.Add(aList[a]);
                                        t += "*" + Localization.GetString("learn_move_PokemonLearnedMove", "[POKEMONNAME] learned~[MOVENAME]!").Replace("[POKEMONNAME]", _evolvedPokemon.GetDisplayName()).Replace("[MOVENAME]", aList[a].Name);
                                        PlayerStatistics.Track("Moves learned", 1);
                                        _canEnd = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _canEnd = true;
                        }

                        if (_evolutionTrigger == EvolutionCondition.EvolutionTrigger.Trading)
                        {
                            EvolutionCondition econ = EvolutionCondition.GetEvolutionCondition(_currentPokemon, _evolutionTrigger, _evolutionArg);
                            bool removeItem = false;
                            if (econ.Trigger == EvolutionCondition.EvolutionTrigger.Trading)
                            {
                                for (int i = 0; i <= econ.Conditions.Count - 1; i++)
                                {
                                    if (econ.Conditions[i].ConditionType == EvolutionCondition.ConditionTypes.HoldItem)
                                        removeItem = true;
                                }
                            }
                            if (removeItem == true)
                                _evolvedPokemon.Item = null;
                        }

                        Core.Player.AddPoints(10, "Evolved Pokémon.");

                        if (ConnectScreen.Connected == true)
                            Core.ServersManager.ServerConnection.SendGameStateMessage("evolved their " + _currentPokemon.GetName() + " into a " + _evolvedPokemon.GetName() + "!");

                        TextBox.Show(t, [], false, false);
                    }
                }
            }
            else
            {
                if (TextBox.Showing == false)
                {
                    if (_canEnd == false)
                    {
                        if (_attackLearnList.Count > 0)
                            Core.SetScreen(new LearnAttackScreen(Core.CurrentScreen!, _evolvedPokemon, _attackLearnList));
                        _canEnd = true;
                    }
                    else
                    {
                        if (Core.CurrentScreen!.Identification == Identifications.EvolutionScreen)
                            Endscene();
                    }
                }
            }
        }
    }

    private void Endscene()
    {
        if (_brokeEvolution == false)
        {
            if (Shedinja.CanEvolveInto(_evolvedPokemon, _evolutionTrigger) == true)
            {
                Core.Player.Pokemons.Add(Shedinja.GenerateNew(_evolvedPokemon));
                Core.Player.Inventory.RemoveItem(5.ToString(), 1);
            }

            Core.Player.Pokemons[PokeList[0]] = _evolvedPokemon;
        }
        PokeList.RemoveAt(0);
        if (PokeList.Count == 0)
        {
            if (_fromBattle == false)
            {
                Screen s = Core.CurrentScreen!;
                while (s.PreScreen!.Identification == Identifications.EvolutionScreen)
                    s = s.PreScreen;
                Core.SetScreen(new TransitionScreen(s, s.PreScreen!, Color.Black, false));
                MusicManager.Play(SavedMusic, true, 0.01f);
            }
            else
            {
                Screen s = Core.CurrentScreen!;
                while (s.Identification != Identifications.BattleScreen)
                    s = s.PreScreen!;
                ChangeSavedScreen();
                Core.SetScreen(new TransitionScreen(this, ((BattleSystem.BattleScreen)s).SavedOverworld.OverworldScreen, Color.Black, false));
            }
        }
        else
        {
            Core.SetScreen(new TransitionScreen(this, new EvolutionScreen(this, PokeList, _evolutionArg, _evolutionTrigger, _fromBattle), Color.Black, false));
        }
    }

    private void EvolvePokemon()
    {
        int hpPercentage = (int)((float)_currentPokemon.HP / _currentPokemon.MaxHP * 100);
        String id = _currentPokemon.GetEvolutionID(_evolutionTrigger, _evolutionArg);
        if (id.Contains('_') == true)
            _evolvedPokemon = Pokemon.GetPokemonByID(int.Parse(id.Split('_')[0]), id.Split('_')[1], true);
        else
            _evolvedPokemon = Pokemon.GetPokemonByID(int.Parse(id), String.Empty, true);

        if (_evolvedPokemon.AdditionalData == String.Empty && _currentPokemon.AdditionalData != String.Empty)
            _evolvedPokemon.AdditionalData = _currentPokemon.AdditionalData;

        _evolvedPokemon.Status = _currentPokemon.Status;

        _evolvedPokemon.EVHP = _currentPokemon.EVHP;
        _evolvedPokemon.EVAttack = _currentPokemon.EVAttack;
        _evolvedPokemon.EVDefense = _currentPokemon.EVDefense;
        _evolvedPokemon.EVSpAttack = _currentPokemon.EVSpAttack;
        _evolvedPokemon.EVSpDefense = _currentPokemon.EVSpDefense;
        _evolvedPokemon.EVSpeed = _currentPokemon.EVSpeed;

        _evolvedPokemon.Friendship = _currentPokemon.Friendship;
        _evolvedPokemon.NickName = _currentPokemon.NickName;

        _evolvedPokemon.IVHP = _currentPokemon.IVHP;
        _evolvedPokemon.IVAttack = _currentPokemon.IVAttack;
        _evolvedPokemon.IVDefense = _currentPokemon.IVDefense;
        _evolvedPokemon.IVSpAttack = _currentPokemon.IVSpAttack;
        _evolvedPokemon.IVSpDefense = _currentPokemon.IVSpDefense;
        _evolvedPokemon.IVSpeed = _currentPokemon.IVSpeed;

        _evolvedPokemon.Generate(_currentPokemon.Level, false);

        _evolvedPokemon.attacks = _currentPokemon.attacks;
        _evolvedPokemon.Gender = _currentPokemon.Gender;
        _evolvedPokemon.Nature = _currentPokemon.Nature;

        _evolvedPokemon.ReloadDefinitions();
        _evolvedPokemon.CalculateStats();

        bool hasOldAbility = false;

        _evolvedPokemon.AbilitySlot = _currentPokemon.AbilitySlot;

        if (_currentPokemon.IsUsingHiddenAbility == true && _evolvedPokemon.HasHiddenAbility == true)
        {
            _evolvedPokemon.Ability = _evolvedPokemon.hiddenAbility;
        }
        else
        {
            for (int a = 0; a <= _evolvedPokemon.newAbilities.Count - 1; a++)
            {
                if (_evolvedPokemon.newAbilities[a].ID == _currentPokemon.Ability!.ID)
                {
                    hasOldAbility = true;
                    if (a == 0)
                        _evolvedPokemon.AbilitySlot = "A";
                    else if (a == 1)
                        _evolvedPokemon.AbilitySlot = "B";
                    else if (a == 2)
                        _evolvedPokemon.AbilitySlot = "C";
                    else
                        _evolvedPokemon.AbilitySlot = _evolvedPokemon.Ability!.ID.ToString();
                    _evolvedPokemon.Ability = _currentPokemon.Ability;
                    break;
                }
            }

            if (hasOldAbility == false)
            {
                if (_currentPokemon.AbilitySlot != null)
                {
                    _evolvedPokemon.Ability = _evolvedPokemon.Ability;
                }
                else
                {
                    int newAbilityIndex = Core.Random.Next(0, 2);
                    if (newAbilityIndex == 0)
                    {
                        _evolvedPokemon.Ability = Ability.GetAbilityByID(_evolvedPokemon.newAbilities[0].ID);
                        _evolvedPokemon.AbilitySlot = "A";
                    }
                    else if (newAbilityIndex == 1)
                    {
                        if (_evolvedPokemon.newAbilities.Count > 1 && _evolvedPokemon.newAbilities[1] != null)
                            _evolvedPokemon.Ability = Ability.GetAbilityByID(_evolvedPokemon.newAbilities[1].ID);
                        else
                            _evolvedPokemon.Ability = Ability.GetAbilityByID(_evolvedPokemon.newAbilities[0].ID);
                        _evolvedPokemon.AbilitySlot = "B";
                    }
                }
            }
        }
        _evolvedPokemon.SetOriginalAbility();
        _evolvedPokemon.IsShiny = _currentPokemon.IsShiny;
        _evolvedPokemon.Item = _currentPokemon.Item;

        _evolvedPokemon.catchBall = _currentPokemon.catchBall;
        if (_currentPokemon.CatchMethod == String.Empty)
            _currentPokemon.CatchMethod = Localization.GetString("CatchMethod_Empty", "Somehow obtained at");
        if (_currentPokemon.CatchLocation == String.Empty)
            _currentPokemon.CatchLocation = Localization.GetString("CatchLocation_Empty", "an unknown place");
        _evolvedPokemon.CatchLocation = _currentPokemon.CatchLocation;
        _evolvedPokemon.CatchMethod = _currentPokemon.CatchMethod;
        _evolvedPokemon.CatchTrainerName = _currentPokemon.CatchTrainerName;
        _evolvedPokemon.OT = _currentPokemon.OT;
        _evolvedPokemon.Experience = _currentPokemon.Experience;

        _evolvedPokemon.HP = (int)(_evolvedPokemon.MaxHP * (hpPercentage / 100.0));
    }

    public String SavedMusic = String.Empty;

    public void ChangeSavedScreen()
    {
        Screen s = Core.CurrentScreen!;
        while (s.Identification != Identifications.BattleScreen)
            s = s.PreScreen!;
        Screen.Level = ((BattleSystem.BattleScreen)s).SavedOverworld.Level;
        Screen.Camera = ((BattleSystem.BattleScreen)s).SavedOverworld.Camera;
        Screen.Effect = ((BattleSystem.BattleScreen)s).SavedOverworld.Effect;
        Screen.SkyDome = ((BattleSystem.BattleScreen)s).SavedOverworld.SkyDome;
        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    private class Spark
    {
        private Texture2D _t;
        private Vector2 _position = new Vector2(0, 0);
        private int _size = 0;
        private float _speed = 0.0f;
        private Vector2 _aim;
        private float _delay = 0.0f;
        private Color _c = new Color(255, 255, 255);

        private bool _xReady = false;
        private bool _yReady = false;

        public bool IsReady = false;
        public bool doGrow = false;
        private int _grown = 50;

        public Spark()
        {
            _t = TextureManager.GetTexture(@"GUI\Evolution\EvolutionSpark");
            _size = Core.Random.Next(12, 150);
            _speed = (float)(Core.Random.Next(5, 35) / 10.0);
            _aim = new Vector2(Core.windowSize.Width / 2.0f - _size / 2.0f, Core.windowSize.Height / 2.0f - _size / 2.0f);
            _aim = new Vector2(_aim.X + (Core.Random.Next(0, 320) - 160), _aim.Y + (Core.Random.Next(0, 320) - 160));
            _delay = (float)(Core.Random.Next(10, 250) / 10.0);
            _c = new Color(Core.Random.Next(200, 256), Core.Random.Next(200, 256), Core.Random.Next(200, 256));

            switch (Core.Random.Next(0, 4))
            {
                case 0:
                    _position.X = -_size;
                    _position.Y = Core.Random.Next(0, Core.windowSize.Height - _size);
                    break;
                case 1:
                    _position.X = Core.Random.Next(0, Core.windowSize.Height - _size);
                    _position.Y = -_size;
                    break;
                case 2:
                    _position.X = Core.windowSize.Width;
                    _position.Y = Core.Random.Next(0, Core.windowSize.Height - _size);
                    break;
                case 3:
                    _position.X = Core.Random.Next(0, Core.windowSize.Height - _size);
                    _position.Y = Core.windowSize.Height;
                    break;
            }
        }

        public void Draw()
        {
            Core.SpriteBatch.Draw(_t, new Rectangle((int)_position.X, (int)_position.Y, _size, _size), _c);
        }

        public void Update()
        {
            if (IsReady == false)
            {
                if (_delay > 0.0f)
                {
                    _delay -= 0.1f;
                    if (_delay <= 0.0f)
                        _delay = 0.0f;
                }
                else
                {
                    if (_xReady == false)
                    {
                        if (_position.X == _aim.X)
                            _xReady = true;
                        if (_position.X < _aim.X)
                        {
                            _position.X += _speed;
                            if (_position.X >= _aim.X)
                                _xReady = true;
                        }
                        if (_position.X > _aim.X)
                        {
                            _position.X -= _speed;
                            if (_position.X <= _aim.X)
                                _xReady = true;
                        }
                    }

                    if (_yReady == false)
                    {
                        if (_position.Y == _aim.Y)
                            _yReady = true;
                        if (_position.Y < _aim.Y)
                        {
                            _position.Y += _speed;
                            if (_position.Y >= _aim.Y)
                                _yReady = true;
                        }
                        if (_position.Y > _aim.Y)
                        {
                            _position.Y -= _speed;
                            if (_position.Y <= _aim.Y)
                                _yReady = true;
                        }
                    }

                    if (Core.Random.Next(0, 3) == 0)
                        _speed += 0.1f;

                    if (_xReady == true && _yReady == true)
                        IsReady = true;
                }
            }

            if (doGrow == true)
            {
                if (_grown > 0)
                {
                    _size += 2;
                    _position.X -= 1;
                    _position.Y -= 1;
                    _grown -= 1;

                    if (_grown == 0)
                    {
                        switch (Core.Random.Next(0, 4))
                        {
                            case 0:
                                _aim.X = -_size;
                                _aim.Y = Core.Random.Next(0, Core.windowSize.Height - _size);
                                break;
                            case 1:
                                _aim.X = Core.Random.Next(0, Core.windowSize.Height - _size);
                                _aim.Y = -_size;
                                break;
                            case 2:
                                _aim.X = Core.windowSize.Width;
                                _aim.Y = Core.Random.Next(0, Core.windowSize.Height - _size);
                                break;
                            case 3:
                                _aim.X = Core.Random.Next(0, Core.windowSize.Height - _size);
                                _aim.Y = Core.windowSize.Height;
                                break;
                        }

                        doGrow = false;
                        IsReady = false;
                        _xReady = false;
                        _yReady = false;
                        _speed *= 3;
                    }
                }
            }
        }
    }
}
