using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.BattleSystem;

namespace P3D;

/// <summary>Manages the collection of entities that compose a map.</summary>
public class Level
{
    // Constants

    private const int OFFSET_MAP_UPDATE_INTERVAL_MS = 16;
    private const int DEFAULT_OFFSET_DELAY = 50;


    // _dayTime stores World.DayTimes internally; DayTime property exposes it as int (1–4).
    // The backing type differs from the property type so the field keyword cannot be used.
    private World.DayTimes _dayTime = World.GetTime();

    // Internal state — not exposed as properties

    private int _offsetMapUpdateDelay = DEFAULT_OFFSET_DELAY;
    private System.Timers.Timer _offsetTimer = new System.Timers.Timer();
    private bool _isUpdatingOffsetMaps;

    // Read-only infrastructure set in constructor

    public RouteSign RouteSign { get; } = new RouteSign();
    public BackdropRenderer BackdropRenderer { get; private set; } = new BackdropRenderer();
    public PokemonEncounter? PokemonEncounterRef { get; private set; }

    // Public data

    public WarpDataStruct WarpData;
    public PokemonEncounterDataStruct PokemonEncounterData;
    public String BattleVariables { get; set; } = "";
    public String SurfingBattleMapData { get; set; } = "";

    // Entity collections

    public List<Entity> Entities { get; set; } = [];
    public List<Entity> Floors { get; set; } = [];
    public List<Shader> Shaders { get; set; } = [];
    public List<NetworkPlayer> NetworkPlayers { get; set; } = [];
    public List<NetworkPokemon> NetworkPokemon { get; set; } = [];
    public List<Entity> OffsetmapEntities { get; set; } = [];
    public List<Entity> OffsetmapFloors { get; set; } = [];

    // Player entities

    public OwnPlayer? OwnPlayer { get; set; }
    public OverworldPokemon? OverworldPokemon { get; set; }

    // Map geometry

    public Terrain Terrain { get; } = new Terrain(Terrain.TerrainTypes.Plain);

    // Level state

    public bool Surfing { get; set; }
    public bool Riding { get; set; }
    public bool UsedStrength { get; set; }
    public bool IsDark { get; set; }
    public int WalkedSteps { get; set; }
    public bool IsRadioOn { get; set; }
    public bool IsOutside { get; set; }
    public bool IsSafariZone { get; set; }
    public bool IsBugCatchingContest { get; set; }
    public bool ShowOverworldPokemon { get; set; } = true;
    public bool WildPokemonGrass { get; set; } = true;
    public bool WildPokemonFloor { get; set; }
    public bool WildPokemonWater { get; set; } = true;

    // Map properties

    public String BlackOutScript { get; set; } = "";
    public String MapName { get; set; } = "";
    public String MusicLoop { get; set; } = "";
    public String LevelFile { get; set; } = "";
    public String CurrentRegion { get; set; } = "Johto";
    public String RegionalForm { get; set; } = "";
    public String BugCatchingContestData { get; set; } = "";
    public String BattleMapData { get; set; } = "";
    public bool CanTeleport { get; set; } = true;
    public bool CanDig { get; set; }
    public bool CanFly { get; set; }
    public int RideType { get; set; }
    public int WeatherType { get; set; }
    public int EnvironmentType { get; set; }
    public int HiddenAbilityChance { get; set; }
    public int LightingType { get; set; }

    // World / audio

    public World? World { get; set; }
    public GameJolt.PokegearScreen.RadioStation? SelectedRadioStation { get; set; }
    public List<decimal> AllowedRadioChannels { get; set; } = [];

    // DayTime — converts between World.DayTimes enum and int (1=Night 2=Morning 3=Day 4=Evening)

    public int DayTime
    {
        get
        {
            switch (_dayTime)
            {
                case World.DayTimes.Night:
                    return 1;
                case World.DayTimes.Morning:
                    return 2;
                case World.DayTimes.Day:
                    return 3;
                case World.DayTimes.Evening:
                    return 4;
                default:
                    return (int)World.GetTime() + 1;
            }
        }
        set
        {
            switch (value)
            {
                case 1:
                    _dayTime = World.DayTimes.Night;
                    break;
                case 2:
                    _dayTime = World.DayTimes.Morning;
                    break;
                case 3:
                    _dayTime = World.DayTimes.Day;
                    break;
                case 4:
                    _dayTime = World.DayTimes.Evening;
                    break;
                default:
                    _dayTime = World.GetTime();
                    break;
            }
        }
    }

    // DisabledMenus — normalises "All" / "StartMenus" shorthands and auto-appends StartMenus

    public String DisabledMenus
    {
        get => field.ToLower();
        set
        {
            if ("all".Equals(value.ToLower()))
            {
                field = "PokeGear,Pokedex,Pokemon,Bag,TrainerCard,Options,Save,All";
            }
            else if ("startmenus".Equals(value.ToLower()))
            {
                field = "Pokedex,Pokemon,Bag,TrainerCard,Options,Save,StartMenus";
            }
            else if (value.Equals(""))
            {
                field = "None";
            }
            else
            {
                field = value;
            }

            String lower = field.ToLower();
            if (lower.Contains("startmenus") == false && lower.Contains("none") == false &&
                lower.Contains("pokedex") && lower.Contains("pokemon") &&
                lower.Contains("bag") && lower.Contains("trainercard") &&
                lower.Contains("options") && lower.Contains("save") &&
                lower.Contains("pokegear") == false)
            {
                field += ",StartMenus";
            }
        }
    } = "";

    // Constructor

    public Level()
    {
        WarpData = new WarpDataStruct();
        PokemonEncounterData = new PokemonEncounterDataStruct();
        PokemonEncounterRef = new PokemonEncounter(this);

        BackdropRenderer = new BackdropRenderer();
        BackdropRenderer.Initialize();

        StartOffsetMapUpdate();
    }

    // Offset map update cycle

    public void StartOffsetMapUpdate()
    {
        if (_offsetTimer != null)
        {
            _offsetTimer.Stop();
        }

        _offsetTimer = new System.Timers.Timer();
        _offsetTimer.Interval = OFFSET_MAP_UPDATE_INTERVAL_MS;
        _offsetTimer.AutoReset = true;
        _offsetTimer.Elapsed += UpdateOffsetMap;
        _offsetTimer.Start();

        Logger.Debug("Started Offset map update");
    }

    public void StopOffsetMapUpdate()
    {
        _offsetTimer.Stop();
        while (_isUpdatingOffsetMaps == true)
        {
            Thread.Sleep(1);
        }

        Logger.Debug("Stopped Offset map update");
    }

    // Loading

    /// <summary>Loads a level from a level file. Pass a path starting with "|" to skip loading.</summary>
    public void Load(String levelpath, bool reload = false)
    {
        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            DebugFileWatcher.TriggerReload();
        }

        World = new P3D.World(0, 0);

        if (levelpath.StartsWith("|") == false)
        {
            StopOffsetMapUpdate();
            List<Object> loaderParams = [];
            loaderParams.AddRange(new Object[] { levelpath, false, Vector3.Zero, 0, new List<String>() });
            LevelLoader levelLoader = new LevelLoader();
            levelLoader.LoadLevel(loaderParams.ToArray(), reload);
        }
        else
        {
            Logger.Debug("Don't attempt to load a levelfile.");
        }

        Screen.Camera.Update();

        OwnPlayer = new OwnPlayer(
            Screen.Camera.Position.X, Screen.Camera.Position.Y - 0.1f, Screen.Camera.Position.Z,
            [TextureManager.DefaultTexture], Core.Player.Skin, 0, 0, "", "Gold", 0);
        OwnPlayer.UpdateEntity();

        OverworldPokemon = new OverworldPokemon(
            Screen.Camera.Position.X, Screen.Camera.Position.Y, Screen.Camera.Position.Z + 1);
        OverworldPokemon.ChangeRotation();

        Entities.Add(OwnPlayer);
        Entities.Add(OverworldPokemon);

        Screen.Camera.Update();

        Surfing = Core.Player.StartSurfing;

        if (Surfing == true && OwnPlayer.SkinName.StartsWith("[POKEMON|") == false)
        {
            Core.Player.TempSurfSkin = OwnPlayer.SkinName;

            int pokemonNumber = Core.Player.Pokemons[Core.Player.SurfPokemon].Number;
            String skinName = "[POKEMON|N]" + pokemonNumber +
                PokemonForms.GetOverworldAddition(Core.Player.Pokemons[Core.Player.SurfPokemon]);

            if (Core.Player.Pokemons[Core.Player.SurfPokemon].IsShiny == true)
            {
                skinName = "[POKEMON|S]" + pokemonNumber +
                    PokemonForms.GetOverworldAddition(Core.Player.Pokemons[Core.Player.SurfPokemon]);
            }

            OwnPlayer.SetTexture(skinName, false);
            OwnPlayer.UpdateEntity();

            if ("surf".Equals(MusicManager._currentSongName) == false &&
                (IsRadioOn == false || GameJolt.PokegearScreen.StationCanPlay(SelectedRadioStation) == false))
            {
                MusicManager.Play("surf", true);
            }
        }

        StartOffsetMapUpdate();
    }

    // Draw

    public void Draw()
    {
        BackdropRenderer.Draw();

        if (Screen.Effect != null)
        {
            Screen.Effect.View = Screen.Camera.View;
            Screen.Effect.Projection = Screen.Camera.Projection;
        }

        DebugDisplay.DrawnVertices = 0;
        DebugDisplay.MaxVisibleVertices = 0;
        DebugDisplay.MaxVertices = 0;
        DebugDisplay.MaxDistance = 0;

        List<Entity> allEntities = [..Entities];
        List<Entity> allFloors = [..Floors];

        if (Core.GameOptions.LoadOffsetMaps > 0)
        {
            allEntities.AddRange(OffsetmapEntities);
            allFloors.AddRange(OffsetmapFloors);
        }

        if (allEntities.Count > 0)
        {
            allEntities = allEntities.OrderByDescending(e => e.CameraDistance).ToList();
        }

        if (allFloors.Count > 0)
        {
            allFloors = allFloors.OrderByDescending(e => e.CameraDistance).ToList();
        }

        for (int i = 0; i <= allFloors.Count - 1; i++)
        {
            if (i <= allFloors.Count - 1)
            {
                allFloors[i].Render();
                if (allFloors[i].Visible == true)
                {
                    DebugDisplay.MaxVisibleVertices += allFloors[i].VertexCount;
                }
                DebugDisplay.MaxVertices += allFloors[i].VertexCount;
            }
        }

        for (int i = 0; i <= allEntities.Count - 1; i++)
        {
            if (i <= allEntities.Count - 1)
            {
                allEntities[i].Render();
                if (allEntities[i].Visible == true)
                {
                    DebugDisplay.MaxVisibleVertices += allEntities[i].VertexCount;
                }
                DebugDisplay.MaxVertices += allEntities[i].VertexCount;
            }
        }

        if (IsDark == true)
        {
            DrawFlashOverlay();
        }
    }

    // Update

    public void Update()
    {
        BackdropRenderer.Update();

        UpdatePlayerWarp();
        PokemonEncounterRef?.TriggerBattle();

        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            if (KeyBoardHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.R) == true &&
                Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen)
            {
                Core.OffsetMaps.Clear();
                Logger.Debug(String.Format("Reload map file: {0}", LevelFile));
                Load(LevelFile, true);
                if (Screen.Level?.OverworldPokemon != null)
                {
                    Screen.Level.OverworldPokemon.Visible = false;
                }
            }
        }

        if (JoinServerScreen.Online == true)
        {
            Core.ServersManager.PlayerManager.UpdatePlayers();
        }

        UpdateEntities();
    }

    public void UpdateEntities()
    {
        if (LevelLoader.IsBusy == false)
        {
            for (int i = 0; i <= Entities.Count - 1; i++)
            {
                if (i <= Entities.Count - 1)
                {
                    if (Entities.Count - 1 >= i && Entities[i].CanBeRemoved == true)
                    {
                        Entities.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        if (Entities[i].NeedsUpdate == true)
                        {
                            Entities[i].Update();
                        }
                        Entities[i].UpdateEntity();
                    }
                }
                else
                {
                    break;
                }
            }
        }

        for (int i = 0; i <= Floors.Count - 1; i++)
        {
            if (i <= Floors.Count - 1)
            {
                Floors[i].UpdateEntity();
            }
        }

        SortEntities();
    }

    public void SortEntities()
    {
        if (LevelLoader.IsBusy == false)
        {
            Entities = Entities.OrderByDescending(e => e.CameraDistance).ToList();
        }
    }

    private void UpdateOffsetMap(Object? sender, System.Timers.ElapsedEventArgs e)
    {
        _isUpdatingOffsetMaps = true;

        if (Core.GameOptions.LoadOffsetMaps > 0)
        {
            if (_offsetMapUpdateDelay <= 0)
            {
                if (LevelLoader.IsBusy == false)
                {
                    OffsetmapEntities = OffsetmapEntities.OrderByDescending(x => x.CameraDistance).ToList();
                }

                _offsetMapUpdateDelay = Core.GameOptions.LoadOffsetMaps - 1;

                for (int i = 0; i <= OffsetmapEntities.Count - 1; i++)
                {
                    if (i <= OffsetmapEntities.Count - 1)
                    {
                        if (OffsetmapEntities[i].CanBeRemoved == true)
                        {
                            OffsetmapEntities.RemoveAt(i);
                            i -= 1;
                        }
                        else
                        {
                            if (OffsetmapEntities[i] is NPC npc)
                            {
                                npc.Update();
                            }
                            OffsetmapEntities[i].UpdateEntity();
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = OffsetmapFloors.Count - 1; i >= 0; i--)
                {
                    if (i <= OffsetmapFloors.Count - 1)
                    {
                        OffsetmapFloors[i].UpdateEntity();
                    }
                }
            }
            else
            {
                _offsetMapUpdateDelay -= 1;
            }
        }

        _isUpdatingOffsetMaps = false;
    }

    private void DrawFlashOverlay()
    {
        Core.SpriteBatch.Draw(
            TextureManager.GetTexture(@"GUI\Overworld\flash_overlay"),
            new Microsoft.Xna.Framework.Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height),
            Microsoft.Xna.Framework.Color.White);
    }

    private void UpdatePlayerWarp()
    {
        if (WarpData.DoWarpInNextTick == false)
        {
            return;
        }

        WildPokemonFloor = false;
        PokemonEncounterData.EncounteredPokemon = false;

        Core.Player.StartSurfing = Surfing;

        Screen.Camera.Position = WarpData.WarpPosition;
        Screen.Camera.Update();

        String tempProperties = CanDig.ToString() + "," + CanFly.ToString();

        bool usingGameJoltTexture = OwnPlayer != null && OwnPlayer.UsingGameJoltTexture;
        Core.Player.Skin = OwnPlayer?.SkinName ?? Core.Player.Skin;

        World = new P3D.World(0, 0);

        StopOffsetMapUpdate();
        List<Object> warpLoaderParams = [];
        warpLoaderParams.AddRange(new Object[] { WarpData.WarpDestination, false, Vector3.Zero, 0, new List<String>() });
        LevelLoader warpLevelLoader = new LevelLoader();
        warpLevelLoader.LoadLevel(warpLoaderParams.ToArray());

        Core.Player.AddVisitedMap(LevelFile);
        UsedStrength = false;
        Surfing = Core.Player.StartSurfing;

        OwnPlayer = new OwnPlayer(
            WarpData.WarpPosition.X, WarpData.WarpPosition.Y, WarpData.WarpPosition.Z,
            [TextureManager.DefaultTexture], Core.Player.Skin, 0, 0, "", "Gold", 0);
        OwnPlayer.SetTexture(Core.Player.Skin, usingGameJoltTexture);
        OwnPlayer.UpdateEntity();

        OverworldPokemon = new OverworldPokemon(
            Screen.Camera.Position.X, Screen.Camera.Position.Y, Screen.Camera.Position.Z + 1);
        OverworldPokemon.Visible = false;
        OverworldPokemon.warped = true;

        Entities.Add(OwnPlayer);
        Entities.Add(OverworldPokemon);

        if (Riding == true && CanRide() == false)
        {
            Riding = false;
            OwnPlayer.SetTexture(Core.Player.TempRideSkin, true);
            Core.Player.Skin = Core.Player.TempRideSkin;
        }

        Screen.Camera.InstantTurn(WarpData.WarpRotations);

        RouteSign.Setup(MapName);

        if (MusicManager.ForceMusic.Equals(""))
        {
            if (IsRadioOn == true && GameJolt.PokegearScreen.StationCanPlay(SelectedRadioStation) == true)
            {
                MusicManager.Play(SelectedRadioStation!.Music, true);
            }
            else
            {
                IsRadioOn = false;
                if (Surfing == true)
                {
                    MusicManager.Play("surf", true);
                }
                else if (Riding == true)
                {
                    MusicManager.Play("ride", true);
                }
                else
                {
                    if (MusicManager.GetSong(MusicLoop) != null)
                    {
                        MusicManager.Play(MusicLoop, true, 0.01f);
                    }
                    else if (MusicManager._currentSongName.Equals("silence") || MusicManager.CurrentSong == null)
                    {
                        MusicManager.Play("silence");
                    }
                }
            }
        }
        else if (MusicManager._currentSongName.Equals(MusicManager.ForceMusic) == false)
        {
            MusicManager.Play(MusicManager.ForceMusic, true);
        }

        Screen.Level?.World?.Initialize(EnvironmentType, WeatherType);

        if (System.IO.File.Exists(GameModeManager.GetMapPath("restplaces.dat")))
        {
            String[] restplaces = System.IO.File.ReadAllLines(GameModeManager.GetMapPath("restplaces.dat"));
            foreach (String line in restplaces)
            {
                String place = line.GetSplit(0, "|");
                if (place.Equals(LevelFile))
                {
                    Core.Player.LastRestPlace = place;
                    Core.Player.LastRestPlacePosition = line.GetSplit(1, "|");
                }
            }
        }

        if (WarpData.IsWarpBlock == true)
        {
            Screen.Camera.StopMovement();
            Screen.Camera.Move(1.0f);
        }

        RoamingPokemon.ShiftRoamingPokemon(-1);

        if (WarpData.WarpSound != null && WarpData.WarpSound.Equals("") == false)
        {
            if (tempProperties.Equals(CanDig.ToString() + "," + CanFly.ToString()) == false)
            {
                SoundManager.PlaySound(WarpData.WarpSound, false);
            }
            else if (tempProperties.Equals("True,False") && CanDig == true && CanFly == false)
            {
                SoundManager.PlaySound(WarpData.WarpSound, false);
            }
            else if (tempProperties.Equals("False,False") && CanDig == false && CanFly == false)
            {
                SoundManager.PlaySound(WarpData.WarpSound, false);
            }
        }

        if (Screen.Camera is OverworldCamera overworldCamera)
        {
            overworldCamera.YawLocked = false;
        }

        NetworkPlayer.ScreenRegionChanged();

        Screen.Camera.Update();

        WarpData.DoWarpInNextTick = false;
        WarpData.IsWarpBlock = false;

        if (Core.ServersManager.ServerConnection.Connected == true)
        {
            Core.ServersManager.PlayerManager.NeedsUpdate = true;
        }
    }

    // Battle variable parsing

    public void SetBattleVariables(String battleVariables)
    {
        foreach (String var in battleVariables.Split(','))
        {
            String varname = var.GetSplit(0, ";");
            String varvalue = var.GetSplit(1, ";");

            switch (varname.ToLower())
            {
                case "canrun":
                    BattleSystem.BattleScreen.CanRun = bool.Parse(varvalue);
                    break;
                case "canalwaysrun":
                    BattleSystem.BattleScreen.CanAlwaysRun = bool.Parse(varvalue);
                    break;
                case "cancatch":
                    BattleSystem.BattleScreen.CanCatch = bool.Parse(varvalue);
                    break;
                case "canblackout":
                    BattleSystem.BattleScreen.CanBlackout = bool.Parse(varvalue);
                    break;
                case "canreceiveexp":
                    BattleSystem.BattleScreen.CanReceiveEXP = bool.Parse(varvalue);
                    break;
                case "canuseitems":
                    BattleSystem.BattleScreen.CanUseItems = bool.Parse(varvalue);
                    break;
                case "frontiertrainer":
                    Trainer.FrontierTrainer = ScriptConversion.ToInteger(varvalue);
                    break;
                case "divebattle":
                    BattleSystem.BattleScreen.DiveBattle = bool.Parse(varvalue);
                    break;
                case "inversebattle":
                    BattleSystem.BattleScreen.IsInverseBattle = bool.Parse(varvalue);
                    break;
                case "custombattlemusic":
                    BattleSystem.BattleScreen.CustomBattleMusic = varvalue;
                    break;
                case "hiddenabilitychance":
                    HiddenAbilityChance = ScriptConversion.ToInteger(varvalue);
                    break;
                case "cangainlosemoney":
                    BattleSystem.BattleScreen.CanGainLoseMoney = bool.Parse(varvalue);
                    break;
            }
        }
    }

    // Query helpers

    public List<NPC> GetNPCs()
    {
        List<NPC> result = [];
        foreach (Entity entity in Entities)
        {
            if (entity.EntityID.Equals("NPC"))
            {
                result.Add((NPC)entity);
            }
        }
        return result;
    }

    public NPC? GetNPC(int id)
    {
        foreach (NPC npc in GetNPCs())
        {
            if (npc.NPCID == id)
            {
                return npc;
            }
        }
        return null;
    }

    public Entity? GetEntity(int id)
    {
        if (id == -1)
        {
            throw new ArgumentException("-1 is the default value for NOT having an ID.");
        }
        foreach (Entity entity in Entities)
        {
            if (entity.ID == id)
            {
                return entity;
            }
        }
        return null;
    }

    public bool CheckTrainerSights()
    {
        bool isInSight = false;
        foreach (Entity entity in Entities)
        {
            if (entity.EntityID.Equals("NPC"))
            {
                NPC npc = (NPC)entity;
                if (npc.IsTrainer == true)
                {
                    if (npc.CheckInSight() == true)
                    {
                        isInSight = true;
                    }
                }
            }
        }
        return isInSight;
    }

    public bool CanRide()
    {
        if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
        {
            return true;
        }

        if (RideType > 0)
        {
            switch (RideType)
            {
                case 1:
                    return true;
                case 2:
                    return false;
                case 3:
                    return true;
            }
        }

        if (CanDig == false && CanFly == false)
        {
            return false;
        }

        return true;
    }

    public bool CanMove()
    {
        foreach (Entity e in Entities)
        {
            if (e.Position.X == Screen.Camera.Position.X &&
                e.Position.Z == Screen.Camera.Position.Z &&
                (int)e.Position.Y == (int)Screen.Camera.Position.Y)
            {
                return e.LetPlayerMove();
            }
        }
        return true;
    }
}

public class Terrain
{
    public enum TerrainTypes
    {
        Plain, Sand, Cave, Rock, TallGrass, LongGrass,
        PondWater, SeaWater, Underwater, DistortionWorld,
        Puddles, Snow, Magma, PvPBattle,
        Grass, Mountain, Ocean, Swamp, Raft, Sky, Indoor, Ice
    }

    public TerrainTypes TerrainType;
    public Terrain? Terrain2 { get; set; }

    public Terrain() { }
    public Terrain(TerrainTypes type) { TerrainType = type; }

    public static TerrainTypes FromString(String input)
    {
        switch (input.ToLower())
        {
            case "plain":
                return TerrainTypes.Plain;
            case "sand":
                return TerrainTypes.Sand;
            case "cave":
                return TerrainTypes.Cave;
            case "rock":
                return TerrainTypes.Rock;
            case "tallgrass":
                return TerrainTypes.TallGrass;
            case "longgrass":
                return TerrainTypes.LongGrass;
            case "pondwater":
                return TerrainTypes.PondWater;
            case "seawater":
                return TerrainTypes.SeaWater;
            case "underwater":
                return TerrainTypes.Underwater;
            case "distortionworld":
                return TerrainTypes.DistortionWorld;
            case "puddles":
                return TerrainTypes.Puddles;
            case "snow":
                return TerrainTypes.Snow;
            case "magma":
                return TerrainTypes.Magma;
            case "pvp":
                return TerrainTypes.PvPBattle;
            default:
                Logger.Log(Logger.LogTypes.Warning, "Terrain: Invalid terrain name: \"" + input + "\". Returning \"Plain\".");
                return TerrainTypes.Plain;
        }
    }
}
