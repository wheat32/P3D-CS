using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.BattleSystem;
using P3D.Items;

namespace P3D;

public class BattleCatchScreen : Screen
{
    private Item _ball;

    private Vector3 _pokemonScale;

    private int _animationIndex = 0;
    private bool _inBall = false;
    private bool _criticalCapture = false;

    private bool _textboxStart = false;
    private bool _showPokedexEntry = false;

    private Pokemon _p;

    private bool _spriteVisible = false;

    private BattleSystem.BattleScreen _battleScreen;
    private bool _animationHasStarted = false;
    private List<BattleSystem.AnimationQueryObject> _animationList = [];

    public static bool sentToBox = false;

    public BattleCatchScreen(BattleSystem.BattleScreen battleScreen, Item ball)
    {
        Identification = Identifications.BattleCatchScreen;

        _ball = ball;
        PreScreen = battleScreen;
        UpdateFadeIn = true;
        BattleCatchScreen.sentToBox = false;

        _battleScreen = battleScreen;
        _p = battleScreen.OpponentPokemon!;

        _spriteVisible = battleScreen.OpponentPokemonNPC!.Visible;

        SetCamera();
    }

    public override void Draw()
    {
        SkyDome.Draw(45.0f);

        Level.Draw();

        List<Entity> renderObjects = [_battleScreen.OpponentPokemonNPC!];
        renderObjects = (renderObjects.OrderByDescending(r => r.CameraDistance)).ToList();

        foreach (Entity obj in renderObjects)
        {
            obj.Render();
        }

        if (_animationList.Count > 0)
        {
            int cIndex = 0;
            List<BattleSystem.AnimationQueryObject> cQuery = [];
            if (_animationList.Count > cIndex)
            {
                BattleSystem.AnimationQueryObject cQueryObject = _animationList[cIndex];
                cQuery.Add(cQueryObject);
                while (cQueryObject.PassThis == true && _animationList.Count > cIndex + 1)
                {
                    cIndex++;
                    cQueryObject = _animationList[cIndex];
                    cQuery.Add(cQueryObject);
                }
            }

            cQuery.Reverse();

            foreach (BattleSystem.AnimationQueryObject cQueryObject in cQuery)
            {
                cQueryObject.Draw(_battleScreen);
            }
        }

        World.DrawWeather(Screen.Level.World.CurrentMapWeather);

        TextBox.Draw();
    }

    public void UpdateAnimations()
    {
        int cIndex = 0;
        while (_animationList.Count > cIndex)
        {
            BattleSystem.AnimationQueryObject cQueryObject = _animationList[cIndex];
            cQueryObject.Update(_battleScreen);

            if (cQueryObject.IsReady == true)
            {
                _animationList.RemoveAt(cIndex);
                if (cQueryObject.PassThis == true)
                {
                    continue;
                }
            }
            else
            {
                if (cQueryObject.PassThis == true)
                {
                    cIndex++;
                    continue;
                }
            }
            break;
        }
    }

    private void SetCamera()
    {
        float modelOffsetY = 0.0f;
        if (_battleScreen.OpponentPokemonNPC!.ModelPath != String.Empty)
        {
            modelOffsetY = 0.5f;
        }
        Camera.Position = new Vector3(_battleScreen.OpponentPokemonNPC.Position.X - 2.5f, _battleScreen.OpponentPokemonNPC.Position.Y + 0.25f + modelOffsetY, _battleScreen.OpponentPokemonNPC.Position.Z + 0.5f) - BattleSystem.BattleScreen.BattleMapOffset;
        Camera.Pitch = -0.25f;
        Camera.Yaw = MathHelper.Pi * 1.5f + 0.25f;
    }

    private bool _playIntroSound = false;

    public override void Update()
    {
        if (Screen.Effect != null)
        {
            BasicEffectWithAlphaTest eff = Screen.Effect;
            Lighting.UpdateLighting(ref eff);
            Screen.Effect = eff;
        }
        if (_textboxStart == false)
        {
            _textboxStart = true;
            String text = Localization.GetString("battle_catch_PlayerUsedBall", "<Player.Name> used a~[BALLNAME]!");
            String oneLineLower = _ball.OneLineName().ToLower();
            if (oneLineLower.StartsWith("a") || oneLineLower.StartsWith("o") || oneLineLower.StartsWith("e") ||
                oneLineLower.StartsWith("i") || oneLineLower.StartsWith("u"))
            {
                text = Localization.GetString("battle_catch_PlayerUsedAnBall", "<Player.Name> used an~[BALLNAME]!");
            }
            TextBox.Show(text.Replace("[BALLNAME]", _ball.OneLineName()), [], false, false);
        }
        TextBox.Update();

        SkyDome.Update();

        Level.Update();

        _battleScreen.OpponentPokemonNPC!.UpdateEntity();
        float modelOffsetY = 0.0f;
        if (_battleScreen.OpponentPokemonNPC.ModelPath != String.Empty)
        {
            modelOffsetY = 0.5f;
        }
        ((BattleSystem.BattleCamera)Camera).UpdateMatrices();
        ((BattleSystem.BattleCamera)Camera).UpdateFrustum();
        if (TextBox.Showing == false)
        {
            if (IsCurrentScreen() == true)
            {
                UpdateAnimations();
                switch (_animationIndex)
                {
                    case 0:
                        if (_animationHasStarted == false)
                        {
                            List<bool> shakes = [];
                            for (int i = 0; i <= 3; i++)
                            {
                                if (StayInBall() == true)
                                {
                                    if (_criticalCapture == true)
                                    {
                                        shakes.Clear();
                                        shakes.Add(false);
                                        _inBall = true;
                                        break;
                                    }
                                    else
                                    {
                                        switch (i)
                                        {
                                            case 0: shakes.Add(false); break;
                                            case 1: shakes.Add(true); break;
                                            case 2: shakes.Add(false); break;
                                            case 3: _inBall = true; break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_criticalCapture == true)
                                    {
                                        shakes.Clear();
                                        shakes.Add(false);
                                        _inBall = false;
                                        break;
                                    }
                                    else
                                    {
                                        _inBall = false;
                                        break;
                                    }
                                }
                            }

                            if (Core.Player.ShowBattleAnimations != 0 && _battleScreen.IsPVPBattle == false)
                            {
                                Vector3 pokemonPosition = _battleScreen.OpponentPokemonNPC.Position - BattleSystem.BattleScreen.BattleMapOffset + new Vector3(0, modelOffsetY, 0);
                                _pokemonScale = _battleScreen.OpponentPokemonNPC.Scale;

                                BattleSystem.AnimationQueryObject catchAnimation = new BattleSystem.AnimationQueryObject(null, false);
                                if (_criticalCapture == true)
                                {
                                    catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Throw_Critical", 0, 0);
                                }
                                else
                                {
                                    catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Throw", 0, 0);
                                }

                                Vector3 ballPosition = new Vector3(pokemonPosition.X - 3, pokemonPosition.Y + 0.15f, pokemonPosition.Z) + BattleSystem.BattleScreen.BattleMapOffset;
                                Entity ballEntity = catchAnimation.SpawnEntity(ballPosition, _ball.Texture, new Vector3(0.3f), 1.0f, 0, 0);

                                catchAnimation.AnimationMove(ballEntity, false, 3, 0.1f, 0, 0.075f, false, false, 0f, 0f);
                                catchAnimation.AnimationRotate(ballEntity, false, 0, 0, -0.5f, 0, 0, -6 * MathHelper.Pi, 0, 0, false);
                                catchAnimation.AnimationRotate(ballEntity, false, 0, 0, 6 * MathHelper.Pi, 0, 0, 0, 4, 0, false);

                                catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Open", 3, 0);
                                int smokeParticlesClose = 0;
                                while (smokeParticlesClose <= 38)
                                {
                                    Vector3 smokePosition = new Vector3(pokemonPosition.X + (float)(Core.Random.Next(-10, 10) / 10.0), pokemonPosition.Y - 0.35f, pokemonPosition.Z + (float)(Core.Random.Next(-10, 10) / 10.0)) + BattleSystem.BattleScreen.BattleMapOffset;
                                    Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                                    Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                                    float smokeSpeed = (float)(Core.Random.Next(1, 3) / 25.0f);
                                    Entity smokeEntity = catchAnimation.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1, 3, 0);
                                    Vector3 smokeDestination = new Vector3(ballEntity.Position.X - smokePosition.X + 3, ballEntity.Position.Y - smokePosition.Y, ballEntity.Position.Z - smokePosition.Z - 0.05f);
                                    catchAnimation.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 3, 0);
                                    smokeParticlesClose++;
                                }

                                catchAnimation.AnimationScale(_battleScreen.OpponentPokemonNPC, false, 0.0f, 0.0f, 0.0f, 0.035f, 3, 0, "t");

                                float ballOffsetY = 0;
                                if (modelOffsetY != 0)
                                {
                                    ballOffsetY = 0.25f;
                                }
                                catchAnimation.AnimationMove(ballEntity, false, 3, -0.35f - ballOffsetY, 0, 0.1f, false, false, 8, 0);
                                catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Land", 9, 0);

                                for (int i = 0; i <= shakes.Count - 1; i++)
                                {
                                    catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Shake", 12 + i * 10, 0);
                                    if (shakes[i] == false)
                                    {
                                        catchAnimation.AnimationRotate(ballEntity, false, 0, 0, 0.15f, 0, 0, MathHelper.PiOver4, 12 + i * 10, 0, true);
                                    }
                                    else
                                    {
                                        catchAnimation.AnimationRotate(ballEntity, false, 0, 0, -0.15f, 0, 0, -MathHelper.PiOver4, 12 + i * 10, 0, true);
                                    }
                                }

                                if (_inBall == true)
                                {
                                    for (int i = 0; i <= 2; i++)
                                    {
                                        Vector3 starPosition = new Vector3(pokemonPosition.X + 0.05f, pokemonPosition.Y, pokemonPosition.Z) + BattleSystem.BattleScreen.BattleMapOffset;
                                        Vector3 starDestination = new Vector3(0.05f, 0.65f, 0 - ((1 - i) * 0.4f));
                                        Entity starEntity = catchAnimation.SpawnEntity(starPosition, TextureManager.GetTexture(@"Textures\Battle\BallCatchStar"), new Vector3(0.35f), 1.0f, 12 + shakes.Count * 10);
                                        catchAnimation.AnimationMove(starEntity, true, starDestination.X, starDestination.Y, starDestination.Z, 0.01f, false, false, 12 + shakes.Count * 10, 0.0f);
                                        catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Catch", 12 + shakes.Count * 10, 4);
                                        catchAnimation.AnimationFade(ballEntity, true, 0.01f, 0.0f, 12 + shakes.Count * 10 + 3, 2);
                                    }
                                }
                                else
                                {
                                    catchAnimation.AnimationFade(ballEntity, true, 1.0f, 0.0f, 12 + shakes.Count * 10, 0);
                                    catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Break", 12 + shakes.Count * 10, 0);

                                    int smokeParticlesOpen = 0;
                                    while (smokeParticlesOpen <= 38)
                                    {
                                        Vector3 smokePosition = pokemonPosition + BattleSystem.BattleScreen.BattleMapOffset;
                                        Vector3 smokeDestination = new Vector3((float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0), (float)(Core.Random.Next(-10, 10) / 10.0));
                                        Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                                        Vector3 smokeScale = new Vector3((float)(Core.Random.Next(2, 6) / 10.0));
                                        float smokeSpeed = (float)(Core.Random.Next(1, 3) / 25.0f);
                                        Entity smokeEntity = catchAnimation.SpawnEntity(smokePosition, smokeTexture, smokeScale, 1.0f, 12 + shakes.Count * 10, 0);
                                        catchAnimation.AnimationMove(smokeEntity, true, smokeDestination.X, smokeDestination.Y, smokeDestination.Z, smokeSpeed, false, false, 12 + shakes.Count * 10, 0);
                                        smokeParticlesOpen++;
                                    }

                                    float positionOffsetY = 0;
                                    if (modelOffsetY != 0)
                                    {
                                        positionOffsetY = -0.25f;
                                    }
                                    catchAnimation.AnimationSetPosition(_battleScreen.OpponentPokemonNPC, false, pokemonPosition.X, pokemonPosition.Y - 0.25f - positionOffsetY, pokemonPosition.Z, 11 + shakes.Count * 10, 0);
                                    catchAnimation.AnimationScale(_battleScreen.OpponentPokemonNPC, false, _pokemonScale.X, _pokemonScale.Y, _pokemonScale.Z, 0.035f, 12 + shakes.Count * 10, 0, "b");
                                }

                                _animationList.Add(catchAnimation);
                            }
                            else
                            {
                                BattleSystem.AnimationQueryObject catchAnimation = new BattleSystem.AnimationQueryObject(null, false);
                                if (_inBall == true)
                                {
                                    float ballOffsetY = 0;
                                    if (modelOffsetY != 0)
                                    {
                                        ballOffsetY = 0.25f;
                                    }

                                    Vector3 pokemonPosition = _battleScreen.OpponentPokemonNPC.Position - BattleSystem.BattleScreen.BattleMapOffset + new Vector3(0, modelOffsetY, 0);
                                    Vector3 ballPosition = new Vector3(pokemonPosition.X, pokemonPosition.Y + 0.15f - 0.35f - ballOffsetY, pokemonPosition.Z) + BattleSystem.BattleScreen.BattleMapOffset;
                                    catchAnimation.AnimationFade(_battleScreen.OpponentPokemonNPC, false, 1.0f, 0.0f, 0, 5);
                                    Entity ballEntity = catchAnimation.SpawnEntity(ballPosition, _ball.Texture, new Vector3(0.3f), 1.0f, 0, 0);
                                    catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Catch", 0, 4);
                                }
                                else
                                {
                                    catchAnimation.AnimationPlaySound(@"Battle\Pokeball\Break", 0, 0);
                                }
                                _animationList.Add(catchAnimation);
                            }
                            _animationHasStarted = true;
                        }
                        else
                        {
                            if (_animationList.Count == 0)
                            {
                                _animationIndex = 1;
                            }
                        }
                        break;
                    case 1:
                        if (_inBall == true)
                        {
                            CatchPokemon();
                            BattleSystem.Battle.Caught = true;
                            _animationIndex = 2;
                        }
                        else
                        {
                            Core.SetScreen(PreScreen);
                            ((BattleSystem.BattleScreen)Core.CurrentScreen).Battle.InitializeRound(
                                (BattleSystem.BattleScreen)Core.CurrentScreen,
                                new BattleRoundConst { StepType = BattleRoundConst.StepTypes.Text, Argument = "It broke free!" });
                        }
                        break;
                    case 2:
                        if (_showPokedexEntry == true)
                        {
                            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, new PokedexViewScreen(Core.CurrentScreen, _p, true), Color.White, false));
                        }
                        _animationIndex = 3;
                        break;
                    case 3:
                        Core.SetScreen(new NameObjectScreen(Core.CurrentScreen, _p));
                        _animationIndex = 4;
                        break;
                    case 4:
                        if (_p.catchBall.ID == 186)
                        {
                            _p.FullRestore();
                        }

                        PlayerStatistics.Track("Caught Pokemon", 1);
                        StorePokemon();
                        _animationIndex = 5;
                        break;
                    case 5:
                        Core.SetScreen(PreScreen);
                        BattleSystem.Battle.Won = true;

                        if (bool.Parse(GameModeManager.GetGameRuleValue("GainExpAfterCatch", "0")) == true && BattleSystem.BattleScreen.CanReceiveEXP == true)
                        {
                            ((BattleSystem.BattleScreen)Core.CurrentScreen).BattleQuery.Clear();
                            if (((BattleSystem.BattleCamera)Camera).TargetMode == true)
                            {
                                Camera.Position = new Vector3(_battleScreen.OpponentPokemonNPC!.Position.X - 2.5f, _battleScreen.OpponentPokemonNPC.Position.Y + 0.25f + modelOffsetY, _battleScreen.OpponentPokemonNPC.Position.Z + 0.5f) - BattleSystem.BattleScreen.BattleMapOffset;
                                Camera.Pitch = -0.25f;
                                Camera.Yaw = MathHelper.Pi * 1.5f + 0.25f;
                                ((BattleSystem.BattleCamera)Camera).TargetMode = false;
                            }

                            GainCatchEXP((BattleSystem.BattleScreen)Core.CurrentScreen);
                            ((BattleSystem.BattleScreen)Core.CurrentScreen).BattleQuery.Add(new EndBattleQueryObject(false));
                        }
                        else
                        {
                            ((BattleSystem.BattleScreen)Core.CurrentScreen).EndBattle(false);
                        }
                        break;
                }
            }
        }
    }

    private void CatchPokemon()
    {
        if (_p.OriginalItem != null)
        {
            _p.OriginalItem = null;
        }
        _p.ResetTemp();

        String s = Localization.GetString("battle_catch_CaughtPokemon", "Gotcha!~[POKEMONNAME] was caught!").Replace("[POKEMONNAME]", _p.GetName());

        String dexID = PokemonForms.GetPokemonDataFileName(_p.Number, _p.AdditionalData, true);

        if (Core.Player.HasPokedex == true)
        {
            if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) < 2)
            {
                s += "*" + Localization.GetString("battle_catch_PokemonAddedToDex", "[POKEMONNAME]'s data was~added to the Pokédex.").Replace("[POKEMONNAME]", _p.GetName());
                _showPokedexEntry = true;
            }
        }

        if (_p.IsShiny == true)
        {
            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 3);
        }
        else
        {
            if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) < 3)
            {
                Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 2);
            }
        }

        _p.SetCatchInfos(_ball, Localization.GetString("CatchMethod_Caught", "Caught at"));

        MusicManager.Pause();
        String musicLoop = Screen.Level.CurrentRegion.Split(',')[0] + "_wild_defeat";
        if (MusicManager.SongExists(musicLoop) == false)
        {
            musicLoop = "wild_defeat";
        }
        MusicManager.Play(musicLoop, false, 0.0f);
        SoundManager.PlaySound("success_catch", true);
        TextBox.Show(s, [], false, false);

        if (_battleScreen.IsTrainerBattle == false)
        {
            if (BattleSystem.BattleScreen.TempPokeFile != String.Empty)
            {
                if (Core.Player.PokeFiles.Contains(BattleSystem.BattleScreen.TempPokeFile) == false)
                {
                    Core.Player.PokeFiles.Add(BattleSystem.BattleScreen.TempPokeFile);
                }
            }
        }
    }

    private void StorePokemon()
    {
        String s = String.Empty;

        if (Core.Player.Pokemons.Count < 6)
        {
            if (_battleScreen.BattleMode == BattleSystem.BattleScreen.BattleModes.BugContest && Core.Player.Pokemons.Count > 1 && Core.Player.Pokemons[1].catchBall.ID == 177)
            {
                Core.Player.Pokemons.RemoveAt(1);
            }

            Core.Player.Pokemons.Add(_p);
        }
        else
        {
            String boxName = StorageSystemScreen.GetBoxName(StorageSystemScreen.DepositPokemon(_p, Player.Temp.PCBoxIndex));
            BattleCatchScreen.sentToBox = true;
            s = Localization.GetString("battle_catch_PokemonTransferedToPC", "It was transfered to Box~\"[BOXNAME]\"~on the PC.").Replace("[BOXNAME]", boxName);
        }

        if (_p.IsShiny == true)
        {
            if (_p.Number != 130)
            {
                GameJolt.Emblem.AchieveEmblem("stars");
            }
        }

        Core.Player.AddPoints(3, "Caught Pokémon.");

        if (s != String.Empty)
        {
            TextBox.Show(s);
        }
    }

    private bool StayInBall()
    {
        Pokemon cp = _p;
        int maxHP = cp.MaxHP;
        int currentHP = cp.HP;
        int catchRate = cp.CatchRate;
        float ballRate = _ball.CatchMultiplier;
        int pokemonStartFriendship = cp.Friendship;

        switch (_ball.OriginalName.ToLower())
        {
            case "repeat ball":
                String dexID = PokemonForms.GetPokemonDataFileName(cp.Number, cp.AdditionalData);
                if (dexID.Contains("_") == false)
                {
                    if (PokemonForms.GetAdditionalDataForms(cp.Number) != null && PokemonForms.GetAdditionalDataForms(cp.Number).Contains(cp.AdditionalData))
                    {
                        dexID = cp.Number + ";" + cp.AdditionalData;
                    }
                    else
                    {
                        dexID = cp.Number.ToString();
                    }
                }
                if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) > 1)
                {
                    ballRate = 2.5f;
                }
                break;
            case "nest ball":
                ballRate = (float)((41 - cp.Level) / 10);
                ballRate = (int)MathHelper.Clamp(ballRate, 1, 4);
                break;
            case "net ball":
                if (cp.IsType(Element.Types.Bug) == true || cp.IsType(Element.Types.Water) == true)
                {
                    ballRate = 3.5f;
                }
                break;
            case "dive ball":
                if (BattleSystem.BattleScreen.DiveBattle == true)
                {
                    ballRate = 3.5f;
                }
                break;
            case "lure ball":
                if (BattleSystem.BattleScreen.DiveBattle == true)
                {
                    ballRate = 5.0f;
                }
                break;
            case "dusk ball":
                if (Screen.Level.World.EnvironmentType == World.EnvironmentTypes.Cave || Screen.Level.World.EnvironmentType == World.EnvironmentTypes.Dark)
                {
                    ballRate = 3.5f;
                }
                else if (Screen.Level.World.EnvironmentType == World.EnvironmentTypes.Outside && World.GetTime() == 0)
                {
                    ballRate = 3.5f;
                }
                break;
            case "fast ball":
                if (cp.BaseSpeed >= 100)
                {
                    ballRate = 4.0f;
                }
                break;
            case "level ball":
                if ((int)Math.Floor(_battleScreen.SelfPokemon!.Level / 4.0) > cp.Level)
                {
                    ballRate = 8.0f;
                }
                else if ((int)Math.Floor(_battleScreen.SelfPokemon.Level / 2.0) > cp.Level)
                {
                    ballRate = 4.0f;
                }
                else if (_battleScreen.SelfPokemon.Level > cp.Level)
                {
                    ballRate = 2.0f;
                }
                break;
            case "love ball":
                if (_battleScreen.SelfPokemon!.Number == cp.Number && _battleScreen.SelfPokemon.Gender != cp.Gender)
                {
                    ballRate = 8.0f;
                }
                break;
            case "moon ball":
                foreach (EvolutionCondition ev in cp.evolutionConditions)
                {
                    foreach (EvolutionCondition.Condition con in ev.Conditions)
                    {
                        if (con.ConditionType == EvolutionCondition.ConditionTypes.Item && con.Argument == "8" && ev.Trigger == EvolutionCondition.EvolutionTrigger.ItemUse)
                        {
                            ballRate = 4.0f;
                            break;
                        }
                    }
                    if (ballRate == 4.0f) break;
                }
                break;
            case "heavy ball":
                float weight = cp.pokedexEntry?.Weight ?? 0f;
                if (weight > 451.5f && weight < 677.3f)
                {
                    ballRate = 2.0f;
                }
                else if (weight > 677.3f && weight < 903.0f)
                {
                    ballRate = 3.0f;
                }
                else if (weight > 903.0f)
                {
                    ballRate = 4.0f;
                }
                break;
            case "friend ball":
                cp.Friendship = 200;
                break;
            case "quick ball":
                if (_battleScreen.FieldEffects.Rounds < 2)
                {
                    ballRate = 5.0f;
                }
                break;
            case "timer ball":
                ballRate = ((int)(1 + _battleScreen.FieldEffects.Rounds * 0.3)).Clamp(1, 4);
                break;
            case "dream ball":
                if (cp.Status == Pokemon.StatusProblems.Sleep)
                {
                    ballRate = 3.0f;
                }
                break;
        }

        float status = 1.0f;
        switch (cp.Status)
        {
            case Pokemon.StatusProblems.Poison:
            case Pokemon.StatusProblems.BadPoison:
            case Pokemon.StatusProblems.Burn:
            case Pokemon.StatusProblems.Paralyzed:
                status = 1.5f;
                break;
            case Pokemon.StatusProblems.Sleep:
            case Pokemon.StatusProblems.Freeze:
                status = 2.5f;
                break;
        }

        int captureRate = (int)Math.Floor((1 + (maxHP * 3 - currentHP * 2) * catchRate * ballRate * status) / (double)(maxHP * 3));

        if (_battleScreen.PokemonSafariStatus < 0)
        {
            for (int i = 1; i <= _battleScreen.PokemonSafariStatus; i++)
            {
                captureRate *= 2;
            }
        }
        else if (_battleScreen.PokemonSafariStatus > 0)
        {
            for (int i = 1; i <= _battleScreen.PokemonSafariStatus.ToPositive(); i++)
            {
                captureRate = (int)(captureRate / 2);
            }
        }

        if (captureRate <= 0)
        {
            captureRate = 1;
        }

        float criticalMultiplier = captureRate;
        switch (Pokedex.CountEntries(Core.Player.PokedexData, [2, 3]))
        {
            case >= 0 and <= 30:
                criticalMultiplier *= 0.0f;
                break;
            case >= 31 and <= 150:
                criticalMultiplier *= 0.5f;
                break;
            case >= 151 and <= 300:
                criticalMultiplier *= 1.0f;
                break;
            case >= 301 and <= 450:
                criticalMultiplier *= 1.5f;
                break;
            case >= 451 and <= 600:
                criticalMultiplier *= 2.0f;
                break;
            default:
                criticalMultiplier *= 2.5f;
                break;
        }

        if (Core.Player.Inventory.GetItemAmount("657") > 0)
        {
            criticalMultiplier *= 2.0f;
        }
        int criticalCaptureChance = (int)Math.Floor(criticalMultiplier / 6);
        int criticalCheck = Core.Random.Next(0, 256);
        if (criticalCheck < criticalCaptureChance && _ball.ID != 1)
        {
            _criticalCapture = true;
        }

        int b = (int)(1048560 / Math.Sqrt(Math.Sqrt(16711680.0 / captureRate)));
        int r = Core.Random.Next(0, 65536);

        if (r > b)
        {
            cp.Friendship = pokemonStartFriendship;
            return false;
        }
        else
        {
            return true;
        }
    }

    public static void GainCatchEXP(BattleSystem.BattleScreen battlescreen)
    {
        battlescreen.BattleMenu.Visible = false;

        List<int> expPokemon = [];
        foreach (int i in battlescreen.ParticipatedPokemon)
        {
            if (Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
            {
                expPokemon.Add(i);
            }
        }

        int pokemonCount = Core.Player.Pokemons.Count - 1;
        if (BattleCatchScreen.sentToBox == false)
        {
            pokemonCount -= Core.Player.Pokemons.Count - 2;
        }
        for (int i = 0; i <= pokemonCount; i++)
        {
            if (Core.Player.Inventory.GetItemAmount("658") > 0 && Core.Player.EnableExpAll == true)
            {
                if (expPokemon.Contains(i) == false && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
                {
                    expPokemon.Add(i);
                }
            }
            else
            {
                if (expPokemon.Contains(i) == false && Core.Player.Pokemons[i].Item != null && Core.Player.Pokemons[i].Item.OriginalName.ToLower() == "exp. share" && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted && Core.Player.Pokemons[i].IsEgg == false)
                {
                    expPokemon.Add(i);
                }
            }
        }

        for (int i = 0; i <= expPokemon.Count - 1; i++)
        {
            int pokeIndex = expPokemon[i];
            List<BattleSystem.Attack> attackLearnList = [];
            int levelUpAmount = 0;
            int originalLevel = Core.Player.Pokemons[pokeIndex].Level;
            if (Core.Player.Pokemons[pokeIndex].Level < int.Parse(GameModeManager.GetGameRuleValue("MaxLevel", "100")))
            {
                int exp = BattleSystem.BattleCalculation.GainExp(Core.Player.Pokemons[pokeIndex], battlescreen, expPokemon, pokeIndex);
                battlescreen.BattleQuery.Add(new TextQueryObject(Core.Player.Pokemons[pokeIndex].GetDisplayName() + " gained " + exp + " experience points."));

                int moveLevel = originalLevel;

                for (int e = 1; e <= exp; e++)
                {
                    int[] oldStats;
                    Pokemon poke = Core.Player.Pokemons[pokeIndex];
                    oldStats = [poke.MaxHP, poke.Attack, poke.Defense, poke.SpAttack, poke.SpDefense, poke.Speed];
                    Core.Player.Pokemons[pokeIndex].GetExperience(1, false);

                    if (moveLevel < Core.Player.Pokemons[pokeIndex].Level)
                    {
                        moveLevel = Core.Player.Pokemons[pokeIndex].Level;

                        Core.Player.AddPoints(((int)Math.Sqrt(Core.Player.Pokemons[pokeIndex].Level)).Clamp(1, 10), "Leveled up a Pokémon to level " + moveLevel.ToString() + ".");
                        Core.Player.Pokemons[pokeIndex].ChangeFriendShip(Pokemon.FriendShipCauses.LevelUp);

                        Core.Player.Pokemons[pokeIndex].hasLeveledUp = true;
                        battlescreen.BattleQuery.Add(new PlaySoundQueryObject(@"Battle\exp_max", false));
                        battlescreen.BattleQuery.Add(new TextQueryObject(Localization.GetString("level_up_PokemonReachedLevel", "[POKEMONNAME] reached~level [LEVELNUMBER]!").Replace("[POKEMONNAME]", Core.Player.Pokemons[pokeIndex].GetDisplayName()).Replace("[LEVELNUMBER]", moveLevel.ToString())));
                        battlescreen.BattleQuery.Add(new DisplayLevelUpQueryObject(Core.Player.Pokemons[pokeIndex], oldStats));
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
                        List<BattleSystem.Attack> aList = Core.Player.Pokemons[pokeIndex].attackLearns[originalLevel + l];
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
                    battlescreen.BattleQuery.Add(new LearnMovesQueryObject(Core.Player.Pokemons[pokeIndex], attackLearnList[a], battlescreen));
                }
            }

            Core.Player.Pokemons[pokeIndex].GainEffort(battlescreen.OpponentPokemon!);
        }

        battlescreen.ParticipatedPokemon.Clear();
    }
}
