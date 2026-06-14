using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.Items;

namespace P3D.BattleSystem;

// Full port of BattleScreen.vb (~2,322 lines) — 2026-06-10

public class BattleScreen : Screen
{
    // ---- After-faint switching ----
    public int NextPokemonIndex = -1;
    public bool OwnFaint = false;
    public bool OppFaint = false;

    // ---- Self-switch (U-Turn, Volt Switch, etc.) ----
    public bool SelfSwitchOwn = false;
    public bool SelfSwitchOpp = false;

    // ---- PvP lead indices (shared, reset per battle) ----
    public static int SelfLeadIndex = 0;
    public static int OpponentLeadIndex = 0;

    // ---- Battle rule flags ----
    public bool IsChoiced = false;
    public bool ClearMainMenuTime = false;
    public bool ClearMoveMenuTime = false;
    public static bool CanGainLoseMoney = true;
    public static bool CanCatch = true;
    public static bool CanRun = true;
    public static bool CanAlwaysRun = false;
    public static bool CanBlackout = true;
    public static bool CanReceiveEXP = true;
    public static bool RoamingBattle = false;
    public static P3D.RoamingPokemon? RoamingPokemonStorage = null;
    public static bool CanUseItems = true;
    public static bool DiveBattle = false;
    public static String TempPokeFile = String.Empty;
    public static bool IsInverseBattle = false;
    public static String CustomBattleMusic = String.Empty;

    // ---- Trainer message counters ----
    public int TrainerBigDamageOwn = 0;
    public int TrainerBigDamageOpp = 0;
    public int TrainerFaintedOwn = 0;
    public int TrainerFaintedOpp = 0;
    public int TrainerRecallOwn = 0;
    public int TrainerRecallOpp = 0;
    public int TrainerSendOutOwn = 1;
    public int TrainerSendOutOpp = 1;

    // ---- Battle mode ----
    public enum BattleModes { Standard, Safari, BugContest, PvP }
    public BattleModes BattleMode = BattleModes.Standard;
    public int PokemonSafariStatus = 0;

    // ---- Core objects ----
    public Battle Battle = null!;
    public FieldEffects FieldEffects = null!;
    public OverworldStorage SavedOverworld = null!;
    public BattleMenu BattleMenu = null!;
    public List<QueryObject> BattleQuery { get; } = [];

    // ---- Pokémon references ----
    public Pokemon? SelfPokemon { get; set; }
    public Pokemon? OpponentPokemon { get; set; }

    // ---- Mega evolution flags ----
    public bool IsMegaEvolvingOwn = false;
    public bool IsMegaEvolvingOpp = false;

    // ---- NPC entities ----
    public NPC? SelfPokemonNPC { get; set; }
    public NPC? OpponentPokemonNPC { get; set; }
    public NPC? SelfTrainerNPC { get; set; }
    public NPC? OpponentTrainerNPC { get; set; }
    public NPC? OpponentTrainer2NPC { get; set; }

    // ---- Switch tracking ----
    public bool HasSwitchedOwn = false;
    public bool ShiftCanContinue = true;

    // ---- Pokemon indices ----
    public int SelfPokemonIndex = 0;
    public int OpponentPokemonIndex = 0;

    // ---- Participation ----
    public List<int> ParticipatedPokemon { get; } = [];

    // ---- Multi-pokemon system (future) ----
    public int PokemonOnSide = 1;
    public List<PokemonProfile> Profiles { get; } = [];

    // ---- Battle settings ----
    public bool IsTrainerBattle = false;
    public Screen? OverworldScreen;
    public int defaultMapType;
    public Trainer? Trainer;

    // ---- Display state ----
    public bool DrawColoredScreen = true;
    public Color ColorOverlay = Color.Black;

    // ---- Battle map ----
    public static Vector3 BattleMapOffset = Vector3.Zero;

    // ---- Render targets ----
    public RenderTarget2D BackgroundTarget = null!;
    public RenderTarget2D NPCTarget = null!;

    // ---- Wild pokemon reference (used in InitializeWild) ----
    public Pokemon? WildPokemon;

    // ---- PvP ----
    public bool IsPVPBattle = false;
    public bool IsRemoteBattle = false;
    public bool IsHost = false;
    public int PartnerNetworkID = 0;
    public BattleStatistics SelfStatistics { get; } = new BattleStatistics();
    public BattleStatistics OpponentStatistics { get; } = new BattleStatistics();
    public String PVPGameJoltID = String.Empty;

    // ---- Networking: client ----
    public bool SentInput = false;
    public static String ReceivedQuery = String.Empty;
    public static bool FirstRound = true;
    public bool ClientWaitForData = false;
    public bool ReceivedPokemonData = false;
    public Dictionary<int, QueryObject> TempPVPBattleQuery { get; } = [];
    public String LockData = "{}";
    public bool ClientWonBattle = true;

    // ---- Networking: host ----
    public bool SentHostData = false;
    public static String ReceivedInput = String.Empty;

    // ---- Camera state ----
    private List<int> _lastCameraSettings = [];
    private int _lastCamera = 2;
    private const int CAMERA_SETTING_COUNT = 4;

    // ----------------------------------------------------------------
    //  Constructors
    // ----------------------------------------------------------------

    public BattleScreen(Pokemon wildPokemon, Screen overworldScreen,
                        Spawner.EncounterMethods method)
    {
        WildPokemon = wildPokemon;
        OverworldScreen = overworldScreen;
        defaultMapType = (int)method;
        IsTrainerBattle = false;
        MouseVisible = false;
        PVPGameJoltID = String.Empty;
        Battle.Caught = false;
    }

    public BattleScreen(Trainer trainer, Screen overworldScreen, int introType)
    {
        Trainer = trainer;
        OverworldScreen = overworldScreen;
        defaultMapType = introType;
        IsTrainerBattle = true;
        MouseVisible = false;
        PVPGameJoltID = String.Empty;
        FirstRound = true;
    }

    // ----------------------------------------------------------------
    //  GetScreenStatus override
    // ----------------------------------------------------------------

    public override String GetScreenStatus()
    {
        String pokemonString = "SelfPokemon=SELFEMPTY" + Environment.NewLine
            + "OpponentPokemon=OPPEMPTY";
        if (SelfPokemon != null)
        {
            pokemonString = pokemonString.Replace("SELFEMPTY", SelfPokemon.GetSaveData());
        }
        if (OpponentPokemon != null)
        {
            pokemonString = pokemonString.Replace("OPPEMPTY", OpponentPokemon.GetSaveData());
        }

        String values = "Values=; CanCatch=" + CanCatch + "; CanRun=" + CanRun
            + "; CanAlwaysRun=" + CanAlwaysRun + "; CanBlackout=" + CanBlackout
            + "; CanReceiveEXP=" + CanReceiveEXP + "; RoamingBattle=" + RoamingBattle
            + "; CanUseItems=" + CanUseItems + "; DiveBattle=" + DiveBattle
            + "; TempPokeFile=" + TempPokeFile + "; IsInverseBattle=" + IsInverseBattle
            + "; CanGainLoseMoney=" + CanGainLoseMoney;

        String s = "BattleMode=" + BattleMode + Environment.NewLine
            + "IsTrainerBattle=" + IsTrainerBattle + Environment.NewLine
            + "IsPVPBattle=" + IsPVPBattle + Environment.NewLine
            + "LoadedBattleMap=" + Level.LevelFile + Environment.NewLine
            + pokemonString + Environment.NewLine
            + values + Environment.NewLine
            + "IsRemoteBattle=" + IsRemoteBattle + Environment.NewLine
            + "IsHost=" + IsHost + Environment.NewLine
            + "MenuVisible=" + BattleMenu.Visible;
        return s;
    }

    // ----------------------------------------------------------------
    //  InitializeScreen (private shared setup)
    // ----------------------------------------------------------------

    private void InitializeScreen()
    {
        Identification = Identifications.BattleScreen;
        CanBePaused = true;
        MouseVisible = true;
        CanChat = true;
        CanDrawDebug = true;
        CanMuteAudio = true;
        CanTakeScreenshot = true;

        Screen.TextBox.Showing = false;
        Screen.PokemonImageView.Showing = false;
        Screen.ImageView.Showing = false;
        Screen.ChooseBox.Showing = false;

        Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
        Effect.FogEnabled = true;
        SkyDome = new SkyDome();
        Camera = new BattleCamera();

        Battle = new Battle();
        FieldEffects = new FieldEffects();

        Level = new Level();
        LoadBattleMap();

        if (Core.Player.Badges.Count > 0)
        {
            FieldEffects.Weather = BattleWeather.GetBattleWeather(
                SavedOverworld.Level.World.CurrentMapWeather);
        }

        UpdateFadeIn = true;

        ReceivedInput = String.Empty;
        ReceivedQuery = String.Empty;

        BattleMenu = new BattleMenu();
        BattleMenu.Reset();

        BackgroundTarget = new RenderTarget2D(
            Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height,
            false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        NPCTarget = new RenderTarget2D(
            Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height,
            false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
    }

    // ----------------------------------------------------------------
    //  InitializeWild
    // ----------------------------------------------------------------

    public void InitializeWild(Pokemon wildPokemon, Screen overworldScreen, int mapType)
    {
        SavedOverworld = new OverworldStorage();
        SavedOverworld.OverworldScreen = overworldScreen;
        SavedOverworld.Camera = Screen.Camera;
        SavedOverworld.Level = Screen.Level;
        SavedOverworld.Effect = Screen.Effect;
        SavedOverworld.SkyDome = Screen.SkyDome;

        InitializeScreen();

        PlayerStatistics.Track("Wild battles", 1);
        defaultMapType = mapType;
        OpponentPokemon = wildPokemon;

        if (OpponentPokemon.CatchRate == -1)
        {
            CanCatch = false;
        }

        if (Core.Player.Pokemons.Count == 0)
        {
            Pokemon p1 = Pokemon.GetPokemonByID(247);
            p1.Generate(15, true);
            Core.Player.Pokemons.Add(p1);
        }

        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            String formData = PokemonForms.GetFormDataInParty(Core.Player.Pokemons[i]);
            if (formData.Equals(String.Empty) == false
                && PokemonForms.GetTypeAdditionFromItem(Core.Player.Pokemons[i]).Equals(String.Empty))
            {
                Core.Player.Pokemons[i].LoadDefinitions(Core.Player.Pokemons[i].Number, formData);
                Core.Player.Pokemons[i].ClearTextures();
            }
        }

        int meIndex = 0;
        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            if (Core.Player.Pokemons[i].IsEgg == false
                && Core.Player.Pokemons[i].HP > 0
                && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted)
            {
                meIndex = i;
                break;
            }
        }
        SelfPokemon = Core.Player.Pokemons[meIndex];
        SelfPokemonIndex = meIndex;
        IsTrainerBattle = false;
        ParticipatedPokemon.Add(meIndex);

        String ownModel = GetModelName(true);
        String oppModel = GetModelName(false);
        float ownEntityOffsetY = 0.0f;
        float oppEntityOffsetY = 0.0f;
        if (ownModel.Equals(String.Empty) == false) { ownEntityOffsetY = -0.5f; }
        if (oppModel.Equals(String.Empty) == false) { oppEntityOffsetY = -0.5f; }

        SelfPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(12, ownEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(SelfPokemon!, true), 3,
                wildPokemon.GetDisplayName(), 0, true, "Still", new List<Rectangle>() },
            1.0f, null, 0.0f, ownModel);
        OpponentPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(15, oppEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(wildPokemon, true), 1,
                wildPokemon.GetDisplayName(), 1, true, "Still", new List<Rectangle>() },
            1.0f, null, 0.0f, oppModel);

        if (ownModel.Equals(String.Empty) == false)
        {
            SelfPokemonNPC!.Scale = new Vector3(SelfPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(ownModel);
            SelfPokemonNPC.Rotation = NPC.GetRotationFromInteger(SelfPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(ownModel);
        }
        if (oppModel.Equals(String.Empty) == false)
        {
            OpponentPokemonNPC!.Scale = new Vector3(OpponentPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(oppModel);
            OpponentPokemonNPC.Rotation = NPC.GetRotationFromInteger(OpponentPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(oppModel);
        }
        Screen.Level.Entities.Add(SelfPokemonNPC!);
        Screen.Level.Entities.Add(OpponentPokemonNPC!);

        String ownSkin = Core.Player.Skin;
        if (SavedOverworld.Level.Surfing == true) { ownSkin = Core.Player.TempSurfSkin; }
        if (SavedOverworld.Level.Riding == true) { ownSkin = Core.Player.TempRideSkin; }

        SelfTrainerNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(10, 0, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { ownSkin, 3, "Player", 2, false, "Still", new List<Rectangle>() });
        Screen.Level.Entities.Add(SelfTrainerNPC!);

        ScreenFadeQueryObject cq = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 5);
        cq.PassThis = true;

        CameraQueryObject q = new CameraQueryObject(
            new Vector3(13, 0, 15), new Vector3(21, 0, 15),
            0.05f, 0.05f, -0.8f, 1.4f, 0.0f, 0.0f, 0.016f, 0.016f);
        q.PassThis = true;

        String crySuffixOpp = PokemonForms.GetCrySuffix(OpponentPokemon!);
        PlaySoundQueryObject q1 = new PlaySoundQueryObject(
            OpponentPokemon!.Number.ToString(), true, 5.0f, crySuffixOpp);
        if (OpponentPokemon.IsShiny == true)
        {
            q1 = new PlaySoundQueryObject(@"Battle\shiny", false, 5.0f);
        }

        TextQueryObject q2 = new TextQueryObject(
            "Wild " + OpponentPokemon.GetDisplayName() + " appeared!");

        CameraQueryObject q22 = new CameraQueryObject(
            new Vector3(14, 0, 15), new Vector3(13, 0, 15),
            0.05f, 0.05f, MathHelper.PiOver2, -0.8f, 0.0f, 0.0f, 0.05f, 0.05f);

        CameraQueryObject q3 = new CameraQueryObject(
            new Vector3(14, 0, 11), new Vector3(14, 0, 15),
            0.01f, 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0.0f, 0.0f);
        q3.PassThis = true;

        String crySuffixOwn = PokemonForms.GetCrySuffix(SelfPokemon!);
        PlaySoundQueryObject q31 = new PlaySoundQueryObject(
            SelfPokemon.Number.ToString(), true, 3.0f, crySuffixOwn);
        TextQueryObject q4 = new TextQueryObject("Go, " + SelfPokemon.GetDisplayName() + "!");

        BattleQuery.AddRange(new QueryObject[] { cq, q1, q, q2, q22, q3, q31, q4 });

        ToggleMenuQueryObject q5 = new ToggleMenuQueryObject(BattleMenu.Visible);

        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;

        Battle.SwitchInOwn(this, meIndex, true, -1);
        Battle.SwitchInOpp(this, true, 0);

        BattleQuery.AddRange(new QueryObject[] { cq1, q5, cq2 });

        for (int i = 0; i <= 99; i++) { InsertCasualCameramove(); }

        BattleMode = BattleModes.Standard;
        BattleMenu.Reset();
        DownloadOnlineSprites();
    }

    // ----------------------------------------------------------------
    //  InitializeTrainer
    // ----------------------------------------------------------------

    public void InitializeTrainer(Trainer trainer, Screen overworldScreen, int mapType)
    {
        SavedOverworld = new OverworldStorage();
        SavedOverworld.OverworldScreen = overworldScreen;
        SavedOverworld.Camera = Screen.Camera;
        SavedOverworld.Level = Screen.Level;
        SavedOverworld.Effect = Screen.Effect;
        SavedOverworld.SkyDome = Screen.SkyDome;

        InitializeScreen();

        if (IsPVPBattle == false && IsRemoteBattle == false)
        {
            PlayerStatistics.Track("Trainer battles", 1);
        }
        else
        {
            FieldEffects.Weather = BattleWeather.WeatherTypes.Clear;
            TempPVPBattleQuery.Clear();
        }

        defaultMapType = mapType;
        OpponentPokemon = trainer.Pokemons[0];

        if (Core.Player.Pokemons.Count == 0)
        {
            Pokemon p1 = Pokemon.GetPokemonByID(247);
            p1.Generate(15, true);
            Core.Player.Pokemons.Add(p1);
        }

        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            String formData = PokemonForms.GetFormDataInParty(Core.Player.Pokemons[i]);
            if (formData.Equals(String.Empty) == false
                && PokemonForms.GetTypeAdditionFromItem(Core.Player.Pokemons[i]).Equals(String.Empty))
            {
                Core.Player.Pokemons[i].LoadDefinitions(Core.Player.Pokemons[i].Number, formData);
                Core.Player.Pokemons[i].ClearTextures();
            }
        }

        int meIndex = 0;
        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            if (Core.Player.Pokemons[i].IsEgg == false
                && Core.Player.Pokemons[i].HP > 0
                && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted)
            {
                meIndex = i;
                break;
            }
        }
        SelfPokemon = Core.Player.Pokemons[meIndex];
        SelfPokemonIndex = meIndex;

        if (IsPVPBattle == true)
        {
            SelfPokemon = Core.Player.Pokemons[SelfLeadIndex];
            SelfPokemonIndex = SelfLeadIndex;
            OpponentPokemon = trainer.Pokemons[OpponentLeadIndex];
            OpponentPokemonIndex = OpponentLeadIndex;
        }

        IsTrainerBattle = true;
        ParticipatedPokemon.Add(meIndex);

        int initiallyVisibleOwn = 1;
        if (IsPVPBattle == true && Core.Player.ShowBattleAnimations != 0 && IsPVPBattle == false)
        {
            initiallyVisibleOwn = 0;
        }
        int initiallyVisibleOpp = 1;
        if (Core.Player.ShowBattleAnimations != 0 && IsPVPBattle == false)
        {
            initiallyVisibleOpp = 0;
        }

        String ownModel = GetModelName(true);
        String oppModel = GetModelName(false);
        float ownEntityOffsetY = 0.0f;
        float oppEntityOffsetY = 0.0f;
        if (ownModel.Equals(String.Empty) == false) { ownEntityOffsetY = -0.5f; }
        if (oppModel.Equals(String.Empty) == false) { oppEntityOffsetY = -0.5f; }

        SelfPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(12, ownEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(SelfPokemon!, true), 3,
                SelfPokemon!.GetDisplayName(), 0, true, "Still", new List<Rectangle>() },
            (float)initiallyVisibleOwn, null, 0.0f, ownModel);
        OpponentPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(15, oppEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(OpponentPokemon!, true), 1,
                OpponentPokemon!.GetDisplayName(), 1, true, "Still", new List<Rectangle>() },
            (float)initiallyVisibleOpp, null, 0.0f, oppModel);

        if (ownModel.Equals(String.Empty) == false)
        {
            SelfPokemonNPC!.Scale = new Vector3(SelfPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(ownModel);
            SelfPokemonNPC.Rotation = NPC.GetRotationFromInteger(SelfPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(ownModel);
        }
        if (oppModel.Equals(String.Empty) == false)
        {
            OpponentPokemonNPC!.Scale = new Vector3(OpponentPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(oppModel);
            OpponentPokemonNPC.Rotation = NPC.GetRotationFromInteger(OpponentPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(oppModel);
        }
        Screen.Level.Entities.Add(SelfPokemonNPC!);
        Screen.Level.Entities.Add(OpponentPokemonNPC!);

        String ownSkin = Core.Player.Skin;
        if (SavedOverworld.Level.Surfing == true) { ownSkin = Core.Player.TempSurfSkin; }
        if (SavedOverworld.Level.Riding == true) { ownSkin = Core.Player.TempRideSkin; }

        SelfTrainerNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(10, 0, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { ownSkin, 3, "Player", 2, false, "Still", new List<Rectangle>() });
        Screen.Level.Entities.Add(SelfTrainerNPC!);

        if (trainer.DoubleTrainer == false)
        {
            OpponentTrainerNPC = (NPC)Entity.GetNewEntity("NPC",
                new Vector3(17, 0, 13) + BattleMapOffset,
                (Texture2D[])[null!], (int[])[0, 0], false,
                Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
                Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
                new Object[] { trainer.SpriteName, 1, "Player", 3, false, "Still",
                    new List<Rectangle>() });
            Screen.Level.Entities.Add(OpponentTrainerNPC!);
        }
        else
        {
            OpponentTrainerNPC = (NPC)Entity.GetNewEntity("NPC",
                new Vector3(17, 0, 12.5f) + BattleMapOffset,
                (Texture2D[])[null!], (int[])[0, 0], false,
                Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
                Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
                new Object[] { trainer.SpriteName, 1, "Player", 3, false, "Still",
                    new List<Rectangle>() });
            OpponentTrainer2NPC = (NPC)Entity.GetNewEntity("NPC",
                new Vector3(17, 0, 13.5f) + BattleMapOffset,
                (Texture2D[])[null!], (int[])[0, 0], false,
                Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
                Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
                new Object[] { trainer.SpriteName2, 1, "Player", 3, false, "Still",
                    new List<Rectangle>() });
            Screen.Level.Entities.Add(OpponentTrainerNPC!);
            Screen.Level.Entities.Add(OpponentTrainer2NPC!);
        }

        ScreenFadeQueryObject cq = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 5);
        cq.PassThis = true;

        CameraQueryObject q = new CameraQueryObject(
            new Vector3(13, 0, 15), new Vector3(21, 0, 15),
            0.05f, 0.05f, -0.8f, 1.4f, 0.0f, 0.0f, 0.016f, 0.016f);
        q.PassThis = true;

        TextQueryObject q1 = new TextQueryObject(
            ScriptVersion2.ScriptCommander.Parse(trainer.BattleStartMessage).ToString()!);
        TextQueryObject q11 = new TextQueryObject(
            trainer.Name + ": \"" + "Go, " + OpponentPokemon!.GetDisplayName() + "!\"");

        float oppAnimationOffsetY = 0.0f;
        if (OpponentPokemonNPC!.Model != null) { oppAnimationOffsetY = 0.5f; }
        float ownAnimationOffsetY = 0.0f;
        if (SelfPokemonNPC!.Model != null) { ownAnimationOffsetY = 0.5f; }

        String crySuffixOpp = PokemonForms.GetCrySuffix(OpponentPokemon!);
        AnimationQueryObject ballThrowOpp = new AnimationQueryObject(OpponentPokemonNPC!, false);

        if (Core.Player.ShowBattleAnimations != 0 && IsPVPBattle == false)
        {
            ballThrowOpp.AnimationPlaySound(@"Battle\Pokeball\Throw", 0, 0);
            ballThrowOpp.AnimationSetPosition(null!, false, 15, 0.5f, 13, 0, 0);
            Entity ballThrowEntity = ballThrowOpp.SpawnEntity(
                new Vector3(2, -0.15f, 0), OpponentPokemon!.catchBall!.Texture!,
                new Vector3(0.3f), 1.0f);
            ballThrowOpp.AnimationMove(ballThrowEntity, true, 0,
                0.35f + oppAnimationOffsetY, 0, 0.1f, false, true, 0.0f, 0.5f,
                0.0f, 0.3f, 0.025f);

            ballThrowOpp.AnimationPlaySound(@"Battle\Pokeball\Open", 3, 0);

            int smokeSpawnedOpp = 0;
            while (smokeSpawnedOpp <= 38)
            {
                Vector3 smokeDestination = new Vector3(
                    Core.Random.Next(-10, 10) / 10.0f,
                    Core.Random.Next(-10, 10) / 10.0f + oppAnimationOffsetY,
                    Core.Random.Next(-10, 10) / 10.0f);
                Texture2D smokeTexture = TextureManager.GetTexture(@"Textures\Battle\Smoke");
                Vector3 smokeScale = new Vector3(Core.Random.Next(2, 6) / 10.0f);
                float smokeSpeed = Core.Random.Next(1, 3) / 20.0f;
                Entity smokeEntity = ballThrowOpp.SpawnEntity(
                    new Vector3(0, oppAnimationOffsetY, 0), smokeTexture, smokeScale, 1.0f, 3);
                ballThrowOpp.AnimationMove(smokeEntity, true,
                    smokeDestination.X, smokeDestination.Y, smokeDestination.Z,
                    smokeSpeed, false, false, 3.0f, 0.0f);
                Interlocked.Increment(ref smokeSpawnedOpp);
            }

            ballThrowOpp.AnimationFade(null!, false, 1, 1.0f, 3, 0);
            ballThrowOpp.AnimationPlaySound(OpponentPokemon!.Number.ToString(), 4, 0,
                false, true, crySuffixOpp);
            ballThrowOpp.AnimationMove(null!, false, 0, -0.5f + oppEntityOffsetY, 0,
                0.05f, false, false, 4, 0);
        }
        else
        {
            ballThrowOpp.AnimationPlaySound(OpponentPokemon!.Number.ToString(), 0, 0,
                false, true, crySuffixOpp);
        }

        String crySuffixOwn = PokemonForms.GetCrySuffix(SelfPokemon!);
        CameraQueryObject q2 = new CameraQueryObject(
            new Vector3(14, 0, 15), new Vector3(13, 0, 15),
            0.05f, 0.05f, MathHelper.PiOver2, -0.8f, 0.0f, 0.0f, 0.05f, 0.05f);
        CameraQueryObject q3 = new CameraQueryObject(
            new Vector3(14, 0, 11), new Vector3(14, 0, 15),
            0.01f, 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0.0f, 0.0f);
        q3.PassThis = true;

        PlaySoundQueryObject q31 = new PlaySoundQueryObject(
            SelfPokemon.Number.ToString(), true, 3.0f, crySuffixOwn);
        TextQueryObject q4 = new TextQueryObject(
            "Go, " + SelfPokemon.GetDisplayName() + "!");

        BattleQuery.AddRange(new QueryObject[] { cq, q, q1, q11, ballThrowOpp, q2, q3, q31, q4 });

        ToggleMenuQueryObject q5 = new ToggleMenuQueryObject(BattleMenu.Visible);
        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;

        Battle.SwitchInOwn(this, meIndex, true, SelfPokemonIndex);
        Battle.SwitchInOpp(this, true, OpponentPokemonIndex);

        BattleQuery.AddRange(new QueryObject[] { cq1, q5, cq2 });

        if (trainer.SendOutXOppMessage.ContainsKey(1) == true)
        {
            QueryObject s1 = FocusOppPlayer();
            TextQueryObject s2 = new TextQueryObject(
                ScriptVersion2.ScriptCommander.Parse(
                    trainer.SendOutXOppMessage[1]).ToString()!);
            BattleQuery.AddRange(new QueryObject[] { s1, s2 });
        }
        if (trainer.SendOutXOwnMessage.ContainsKey(1) == true)
        {
            QueryObject s1 = FocusOppPlayer();
            TextQueryObject s2 = new TextQueryObject(
                ScriptVersion2.ScriptCommander.Parse(
                    trainer.SendOutXOwnMessage[1]).ToString()!);
            BattleQuery.AddRange(new QueryObject[] { s1, s2 });
        }

        for (int i = 0; i <= 99; i++) { InsertCasualCameramove(); }

        String dexID = PokemonForms.GetPokemonDataFileName(
            OpponentPokemon!.Number, OpponentPokemon.AdditionalData);
        if (dexID.Contains("_") == false)
        {
            if (PokemonForms.GetAdditionalDataForms(OpponentPokemon.Number) != null
                && PokemonForms.GetAdditionalDataForms(OpponentPokemon.Number)!
                    .Contains(OpponentPokemon.AdditionalData))
            {
                dexID = OpponentPokemon.Number + ";" + OpponentPokemon.AdditionalData;
            }
            else
            {
                dexID = OpponentPokemon.Number.ToString();
            }
        }
        if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) == 0)
        {
            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 1);
        }

        BattleMode = BattleModes.Standard;
        BattleMenu.Reset();
        DownloadOnlineSprites();
    }

    // ----------------------------------------------------------------
    //  InitializeSafari
    // ----------------------------------------------------------------

    public void InitializeSafari(Pokemon wildPokemon, Screen overworldScreen, int mapType)
    {
        SavedOverworld = new OverworldStorage();
        SavedOverworld.OverworldScreen = overworldScreen;
        SavedOverworld.Camera = Screen.Camera;
        SavedOverworld.Level = Screen.Level;
        SavedOverworld.Effect = Screen.Effect;
        SavedOverworld.SkyDome = Screen.SkyDome;

        InitializeScreen();
        PlayerStatistics.Track("Safari battles", 1);
        defaultMapType = mapType;
        OpponentPokemon = wildPokemon;

        if (Core.Player.Pokemons.Count == 0)
        {
            Pokemon p1 = Pokemon.GetPokemonByID(247);
            p1.Generate(15, true);
            Core.Player.Pokemons.Add(p1);
        }

        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            String formData = PokemonForms.GetFormDataInParty(Core.Player.Pokemons[i]);
            if (formData.Equals(String.Empty) == false
                && PokemonForms.GetTypeAdditionFromItem(Core.Player.Pokemons[i]).Equals(String.Empty))
            {
                Core.Player.Pokemons[i].LoadDefinitions(Core.Player.Pokemons[i].Number, formData);
                Core.Player.Pokemons[i].ClearTextures();
            }
        }

        int meIndex = 0;
        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            if (Core.Player.Pokemons[i].IsEgg == false
                && Core.Player.Pokemons[i].HP > 0
                && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted)
            {
                meIndex = i;
                break;
            }
        }
        SelfPokemon = Core.Player.Pokemons[meIndex];
        SelfPokemonIndex = meIndex;
        IsTrainerBattle = false;
        ParticipatedPokemon.Add(meIndex);

        String ownModel = GetModelName(true);
        String oppModel = GetModelName(false);
        float ownEntityOffsetY = 0.0f;
        float oppEntityOffsetY = 0.0f;
        if (ownModel.Equals(String.Empty) == false) { ownEntityOffsetY = -0.5f; }
        if (oppModel.Equals(String.Empty) == false) { oppEntityOffsetY = -0.5f; }

        SelfPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(12, ownEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(SelfPokemon!, true), 3,
                wildPokemon.GetDisplayName(), 0, true, "Still", new List<Rectangle>() },
            1.0f, null, 0.0f, ownModel);
        OpponentPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(15, oppEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(wildPokemon, true), 1,
                wildPokemon.GetDisplayName(), 1, true, "Still", new List<Rectangle>() },
            1.0f, null, 0.0f, oppModel);

        if (ownModel.Equals(String.Empty) == false)
        {
            SelfPokemonNPC!.Scale = new Vector3(SelfPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(ownModel);
            SelfPokemonNPC.Rotation = NPC.GetRotationFromInteger(SelfPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(ownModel);
        }
        if (oppModel.Equals(String.Empty) == false)
        {
            OpponentPokemonNPC!.Scale = new Vector3(OpponentPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(oppModel);
            OpponentPokemonNPC.Rotation = NPC.GetRotationFromInteger(OpponentPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(oppModel);
        }
        Screen.Level.Entities.Add(SelfPokemonNPC!);
        Screen.Level.Entities.Add(OpponentPokemonNPC!);

        String ownSkin = Core.Player.Skin;
        if (SavedOverworld.Level.Surfing == true) { ownSkin = Core.Player.TempSurfSkin; }
        if (SavedOverworld.Level.Riding == true) { ownSkin = Core.Player.TempRideSkin; }

        SelfTrainerNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(10, 0, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { ownSkin, 3, "Player", 2, false, "Still", new List<Rectangle>() });
        Screen.Level.Entities.Add(SelfTrainerNPC!);

        String crySuffixOpp = PokemonForms.GetCrySuffix(OpponentPokemon!);
        PlaySoundQueryObject q1 = new PlaySoundQueryObject(
            OpponentPokemon!.Number.ToString(), true, 5.0f, crySuffixOpp);

        ScreenFadeQueryObject cq = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 5);
        cq.PassThis = true;
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(13, 0, 15), new Vector3(21, 0, 15),
            0.05f, 0.05f, -0.8f, 1.4f, 0.0f, 0.0f, 0.016f, 0.016f);
        q.PassThis = true;

        TextQueryObject q2 = new TextQueryObject(
            "Wild " + OpponentPokemon.GetDisplayName() + " appeared!");
        ToggleMenuQueryObject q5 = new ToggleMenuQueryObject(BattleMenu.Visible);

        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;

        BattleQuery.AddRange(new QueryObject[] { cq, q, q1, q2 });
        BattleQuery.AddRange(new QueryObject[] { cq1, q5, cq2 });

        for (int i = 0; i <= 99; i++) { InsertCasualCameramove(); }

        BattleMode = BattleModes.Safari;
        BattleMenu.Reset();
        DownloadOnlineSprites();
    }

    // ----------------------------------------------------------------
    //  InitializeBugCatch
    // ----------------------------------------------------------------

    public void InitializeBugCatch(Pokemon wildPokemon, Screen overworldScreen, int mapType)
    {
        SavedOverworld = new OverworldStorage();
        SavedOverworld.OverworldScreen = overworldScreen;
        SavedOverworld.Camera = Screen.Camera;
        SavedOverworld.Level = Screen.Level;
        SavedOverworld.Effect = Screen.Effect;
        SavedOverworld.SkyDome = Screen.SkyDome;

        InitializeScreen();
        PlayerStatistics.Track("Bug-Catching contest battles", 1);
        defaultMapType = mapType;
        OpponentPokemon = wildPokemon;

        if (Core.Player.Pokemons.Count == 0)
        {
            Pokemon p1 = Pokemon.GetPokemonByID(10);
            p1.Generate(15, true);
            Core.Player.Pokemons.Add(p1);
        }

        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            String formData = PokemonForms.GetFormDataInParty(Core.Player.Pokemons[i]);
            if (formData.Equals(String.Empty) == false
                && PokemonForms.GetTypeAdditionFromItem(Core.Player.Pokemons[i]).Equals(String.Empty))
            {
                Core.Player.Pokemons[i].LoadDefinitions(Core.Player.Pokemons[i].Number, formData);
                Core.Player.Pokemons[i].ClearTextures();
            }
        }

        int meIndex = 0;
        for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
        {
            if (Core.Player.Pokemons[i].IsEgg == false
                && Core.Player.Pokemons[i].HP > 0
                && Core.Player.Pokemons[i].Status != Pokemon.StatusProblems.Fainted)
            {
                meIndex = i;
                break;
            }
        }
        SelfPokemon = Core.Player.Pokemons[meIndex];
        SelfPokemonIndex = meIndex;
        IsTrainerBattle = false;
        ParticipatedPokemon.Add(meIndex);

        String ownModel = GetModelName(true);
        String oppModel = GetModelName(false);
        float ownEntityOffsetY = 0.0f;
        float oppEntityOffsetY = 0.0f;
        if (ownModel.Equals(String.Empty) == false) { ownEntityOffsetY = -0.5f; }
        if (oppModel.Equals(String.Empty) == false) { oppEntityOffsetY = -0.5f; }

        SelfPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(12, ownEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(SelfPokemon!, true), 3,
                wildPokemon.GetDisplayName(), 0, true, "Still", new List<Rectangle>() },
            1.0f, null, 0.0f, ownModel);
        OpponentPokemonNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(15, oppEntityOffsetY, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 1, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { PokemonForms.GetOverworldSpriteName(wildPokemon, true), 1,
                wildPokemon.GetDisplayName(), 1, true, "Still", new List<Rectangle>() },
            1.0f, null, 0.0f, oppModel);

        if (ownModel.Equals(String.Empty) == false)
        {
            SelfPokemonNPC!.Scale = new Vector3(SelfPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(ownModel);
            SelfPokemonNPC.Rotation = NPC.GetRotationFromInteger(SelfPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(ownModel);
        }
        if (oppModel.Equals(String.Empty) == false)
        {
            OpponentPokemonNPC!.Scale = new Vector3(OpponentPokemon!.GetModelProperties().Item1)
                * ModelManager.MODELSCALE * ModelManager.PokeModelScale(oppModel);
            OpponentPokemonNPC.Rotation = NPC.GetRotationFromInteger(OpponentPokemonNPC.faceRotation)
                + ModelManager.PokeModelRotation(oppModel);
        }
        Screen.Level.Entities.Add(SelfPokemonNPC!);
        Screen.Level.Entities.Add(OpponentPokemonNPC!);

        String ownSkin = Core.Player.Skin;
        if (SavedOverworld.Level.Surfing == true) { ownSkin = Core.Player.TempSurfSkin; }
        if (SavedOverworld.Level.Riding == true) { ownSkin = Core.Player.TempRideSkin; }

        SelfTrainerNPC = (NPC)Entity.GetNewEntity("NPC",
            new Vector3(10, 0, 13) + BattleMapOffset,
            (Texture2D[])[null!], (int[])[0, 0], false,
            Vector3.Zero, Vector3.One, BaseModel.BillModel, 0, String.Empty, true,
            Vector3.One, 0, String.Empty, String.Empty, Vector3.Zero,
            new Object[] { ownSkin, 3, "Player", 2, false, "Still", new List<Rectangle>() });
        Screen.Level.Entities.Add(SelfTrainerNPC!);

        String crySuffixOpp = PokemonForms.GetCrySuffix(OpponentPokemon!);
        PlaySoundQueryObject q1 = new PlaySoundQueryObject(
            OpponentPokemon!.Number.ToString(), true, 5.0f, crySuffixOpp);
        TextQueryObject q2 = new TextQueryObject(
            "Wild " + OpponentPokemon.GetDisplayName() + " appeared!");

        CameraQueryObject q22 = new CameraQueryObject(
            new Vector3(14, 0, 15), new Vector3(13, 0, 15),
            0.05f, 0.05f, MathHelper.PiOver2, -0.8f, 0.0f, 0.0f, 0.05f, 0.05f);
        CameraQueryObject q3 = new CameraQueryObject(
            new Vector3(14, 0, 11), new Vector3(14, 0, 15),
            0.01f, 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0.0f, 0.0f);
        q3.PassThis = true;

        String crySuffixOwn = PokemonForms.GetCrySuffix(SelfPokemon!);
        TextQueryObject q4 = new TextQueryObject("Go, " + SelfPokemon!.GetDisplayName() + "!");

        ScreenFadeQueryObject cq = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 5);
        cq.PassThis = true;
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(13, 0, 15), new Vector3(21, 0, 15),
            0.05f, 0.05f, -0.8f, 1.4f, 0.0f, 0.0f, 0.016f, 0.016f);
        q.PassThis = true;

        BattleQuery.AddRange(new QueryObject[] { cq, q1, q, q2, q22, q3, q4 });

        ToggleMenuQueryObject q5 = new ToggleMenuQueryObject(BattleMenu.Visible);
        ScreenFadeQueryObject cq1 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, true, 16);
        ScreenFadeQueryObject cq2 = new ScreenFadeQueryObject(
            ScreenFadeQueryObject.FadeTypes.Vertical, Color.Black, false, 16);
        cq2.PassThis = true;

        Battle.SwitchInOwn(this, meIndex, true, -1);
        Battle.SwitchInOpp(this, true, 0);

        BattleQuery.AddRange(new QueryObject[] { cq1, q5, cq2 });

        for (int i = 0; i <= 99; i++) { InsertCasualCameramove(); }

        BattleMode = BattleModes.BugContest;
        BattleMenu.Reset();
        DownloadOnlineSprites();
    }

    // ----------------------------------------------------------------
    //  InitializePVP
    // ----------------------------------------------------------------

    public void InitializePVP(Trainer pvpTrainer, Screen overworldScreen)
    {
        IsPVPBattle = true;
        BattleMode = BattleModes.PvP;
        BattleScreen.CanReceiveEXP = false;
        BattleScreen.CanBlackout = false;
        BattleScreen.CanRun = false;
        BattleScreen.CanUseItems = false;
        BattleScreen.CanGainLoseMoney = false;
        PVPLobbyScreen.StoppedBattle = true;
        PVPLobbyScreen.DisconnectMessage = "The battle has ended."
            + Environment.NewLine + Environment.NewLine + "Press any key to exit.";
        PVPLobbyScreen.ScreenState = PVPLobbyScreen.ScreenStates.Stopped;
        InitializeTrainer(pvpTrainer, overworldScreen, 0);
        CanBePaused = false;
        CanChat = false;
    }

    // ----------------------------------------------------------------
    //  LoadBattleMap
    // ----------------------------------------------------------------

    public void LoadBattleMap()
    {
        String levelFile = SavedOverworld.Level.LevelFile;
        String cRegion = SavedOverworld.Level.CurrentRegion.Split(',')[0];
        String[] battleMapData = SavedOverworld.Level.BattleMapData.Split(',');
        String[] surfingBattleMapData = SavedOverworld.Level.SurfingBattleMapData.Split(',');

        if (IsPVPBattle == true)
        {
            levelFile = "pvp.dat";
            BattleMapOffset = Vector3.Zero;
        }
        else
        {
            if (SavedOverworld.Level.BattleMapData.Equals(String.Empty) == false)
            {
                switch (battleMapData.Length)
                {
                    case 1:
                        levelFile = battleMapData[0];
                        break;
                    case 3:
                        BattleMapOffset = new Vector3(
                            float.Parse(battleMapData[0].Replace(".", GameController.DecSeparator)),
                            float.Parse(battleMapData[1].Replace(".", GameController.DecSeparator)),
                            float.Parse(battleMapData[2].Replace(".", GameController.DecSeparator)));
                        break;
                    case 4:
                        levelFile = battleMapData[0];
                        BattleMapOffset = new Vector3(
                            float.Parse(battleMapData[1].Replace(".", GameController.DecSeparator)),
                            float.Parse(battleMapData[2].Replace(".", GameController.DecSeparator)),
                            float.Parse(battleMapData[3].Replace(".", GameController.DecSeparator)));
                        break;
                }
            }
            else
            {
                BattleMapOffset = Vector3.Zero;
            }

            String defaultBattlePath = System.IO.Path.Combine(
                GameController.GamePath, "Content", "Data", "maps", "battle", levelFile);
            String gmBattlePath = System.IO.Path.Combine(
                GameController.GamePath,
                GameModeManager.ActiveGameMode?.MapPath ?? String.Empty,
                "battle", levelFile);

            if (System.IO.File.Exists(defaultBattlePath) == false
                && System.IO.File.Exists(gmBattlePath) == false)
            {
                switch (defaultMapType)
                {
                    case 0:
                        levelFile = cRegion + "0.dat";
                        break;
                    case 2:
                        levelFile = cRegion + "1.dat";
                        break;
                    default:
                        levelFile = cRegion + "0.dat";
                        break;
                }
                BattleMapOffset = Vector3.Zero;
            }

            if (SavedOverworld.Level.Surfing == true)
            {
                if (SavedOverworld.Level.SurfingBattleMapData.Equals(String.Empty) == false)
                {
                    switch (surfingBattleMapData.Length)
                    {
                        case 1:
                            levelFile = surfingBattleMapData[0];
                            break;
                        case 4:
                            levelFile = surfingBattleMapData[0];
                            BattleMapOffset = new Vector3(
                                float.Parse(surfingBattleMapData[1].Replace(".", GameController.DecSeparator)),
                                float.Parse(surfingBattleMapData[2].Replace(".", GameController.DecSeparator)),
                                float.Parse(surfingBattleMapData[3].Replace(".", GameController.DecSeparator)));
                            break;
                        default:
                            levelFile = cRegion + "1.dat";
                            BattleMapOffset = Vector3.Zero;
                            break;
                    }
                    DiveBattle = true;
                }
                else
                {
                    levelFile = cRegion + "1.dat";
                    DiveBattle = true;
                    BattleMapOffset = Vector3.Zero;
                }
            }
        }

        // Final fallback
        String finalBattlePath = System.IO.Path.Combine(
            GameController.GamePath, "Content", "Data", "maps", "battle", levelFile);
        String finalGmPath = System.IO.Path.Combine(
            GameController.GamePath,
            GameModeManager.ActiveGameMode?.MapPath ?? String.Empty,
            "battle", levelFile);
        if (System.IO.File.Exists(finalBattlePath) == false
            && System.IO.File.Exists(finalGmPath) == false)
        {
            switch (defaultMapType)
            {
                case 0:
                    levelFile = "battle0.dat";
                    break;
                case 2:
                    levelFile = "battle1.dat";
                    break;
                default:
                    levelFile = "battle0.dat";
                    break;
            }
            BattleMapOffset = Vector3.Zero;
        }

        Level.Load(@"battle\" + levelFile);
        Level.MapName = SavedOverworld.Level.MapName;
    }

    // ----------------------------------------------------------------
    //  Draw
    // ----------------------------------------------------------------

    public override void Draw()
    {
        DebugDisplay.MaxVertices = 0;
        DebugDisplay.MaxVisibleVertices = 0;

        List<Entity> foregroundEntities = [];
        if (SelfPokemonNPC != null) { foregroundEntities.Add(SelfPokemonNPC); }
        if (OpponentPokemonNPC != null) { foregroundEntities.Add(OpponentPokemonNPC); }
        if (SelfTrainerNPC != null) { foregroundEntities.Add(SelfTrainerNPC); }
        if (OpponentTrainerNPC != null) { foregroundEntities.Add(OpponentTrainerNPC); }

        if (foregroundEntities.Count > 0)
        {
            foregroundEntities = (from f in foregroundEntities
                                  orderby f.CameraDistance descending
                                  select f).ToList();
        }

        List<AnimationQueryObject> foregroundAnimationList = [];
        List<AnimationQueryObject> backgroundAnimationList = [];

        if (BattleQuery.Count > 0)
        {
            int cIndex = 0;
            List<QueryObject> cQuery = [];

            while (cIndex < BattleQuery.Count)
            {
                QueryObject cQueryObject = BattleQuery[cIndex];
                if (cQueryObject.QueryType == QueryObject.QueryTypes.MoveAnimation)
                {
                    AnimationQueryObject anim = (AnimationQueryObject)cQueryObject;
                    if (anim.drawBeforeEntities == true)
                    {
                        backgroundAnimationList.Add(anim);
                        cIndex += 1;
                        continue;
                    }
                    else
                    {
                        foregroundAnimationList.Add(anim);
                        cIndex += 1;
                        continue;
                    }
                }
                else
                {
                    cQuery.Add(cQueryObject);
                }
                if (cQueryObject.PassThis == true)
                {
                    cIndex += 1;
                    continue;
                }
                break;
            }

            cQuery.Reverse();
            if (cQuery.Count > 0)
            {
                foreach (QueryObject cQueryObject in cQuery) { cQueryObject.Draw(this); }
            }
        }

        if (backgroundAnimationList.Count > 0)
        {
            int cIndex = 0;
            List<QueryObject> cQuery = [];

            while (cIndex < backgroundAnimationList.Count)
            {
                QueryObject cQueryObject = backgroundAnimationList[cIndex];
                cQuery.Add(cQueryObject);
                if (cQueryObject.PassThis == true)
                {
                    cIndex += 1;
                    continue;
                }
                break;
            }

            cQuery.Reverse();

            Core.GraphicsDevice.SetRenderTarget(BackgroundTarget);
            Core.GraphicsDevice.Clear(Color.Transparent);
            foreach (QueryObject cQueryObject in cQuery) { cQueryObject.Draw(this); }

            Core.GraphicsDevice.SetRenderTarget(null);
            Core.GraphicsDevice.SetRenderTarget(NPCTarget);
            Core.GraphicsDevice.Clear(Color.Transparent);
            for (int i = 0; i <= foregroundEntities.Count - 1; i++)
            {
                foregroundEntities[i].Render();
                if (foregroundEntities[i].Visible == true)
                {
                    DebugDisplay.MaxVisibleVertices += foregroundEntities[i].VertexCount;
                }
                DebugDisplay.MaxVertices += foregroundEntities[i].VertexCount;
            }

            Core.GraphicsDevice.SetRenderTarget(null);
            SkyDome.Draw(45.0f);
            Level.Draw();
            World.DrawWeather(Screen.Level.World.CurrentMapWeather);
            Core.SpriteBatch.Draw(BackgroundTarget, Core.windowSize, Color.White);
            Core.SpriteBatch.Draw(NPCTarget, Core.windowSize, Color.White);
        }
        else
        {
            SkyDome.Draw(45.0f);
            Level.Draw();
            World.DrawWeather(Screen.Level.World.CurrentMapWeather);
        }

        if (foregroundAnimationList.Count > 0)
        {
            int cIndex = 0;
            List<QueryObject> cQuery = [];

            while (cIndex < foregroundAnimationList.Count)
            {
                QueryObject cQueryObject = foregroundAnimationList[cIndex];
                cQuery.Add(cQueryObject);
                if (cQueryObject.PassThis == true)
                {
                    cIndex += 1;
                    continue;
                }
                break;
            }

            cQuery.Reverse();
            foreach (QueryObject cQueryObject in cQuery) { cQueryObject.Draw(this); }
        }

        if (HasToWaitPVP() == true)
        {
            Canvas.DrawRectangle(
                new Rectangle(0, (int)(Core.windowSize.Height / 2 - 60),
                              Core.windowSize.Width, 120),
                new Color(0, 0, 0, 150));
            String t = "Waiting for the other player  ";
            String displayText = t.Remove(t.Length - 2, 2) + LoadingDots.Dots;
            Core.SpriteBatch.DrawString(
                FontManager.MainFont, displayText,
                new Vector2(
                    (float)(Core.windowSize.Width / 2 - FontManager.MainFont.MeasureString(t).X / 2),
                    (float)(Core.windowSize.Height / 2 - FontManager.MainFont.MeasureString(t).Y / 2)),
                Color.White);
        }
        else
        {
            if (BattleMenu.Visible == true) { BattleMenu.Draw(this); }
        }

        TextBox.Draw();

        if (DrawColoredScreen == true)
        {
            Canvas.DrawRectangle(Core.windowSize, ColorOverlay);
        }

        foregroundAnimationList.Clear();
        backgroundAnimationList.Clear();
    }

    // ----------------------------------------------------------------
    //  Update
    // ----------------------------------------------------------------

    public override void Update()
    {
        if (CheckNetworkPlayer() == false)
        {
            return;
        }

        if (IsRemoteBattle == true && IsHost == false && SentInput == true
            && ReceivedQuery.Equals(String.Empty) == false)
        {
            BattleMenu.Visible = false;
        }
        if (IsRemoteBattle == true && IsHost == false)
        {
            if (ReceivedPokemonData == true && ClientWaitForData == true)
            {
                ClientWaitForData = false;
                ReceivedPokemonData = false;
                BattleMenu.Reset();
                ClearMainMenuTime = true;
                ClearMoveMenuTime = true;
                BattleScreen bsRef1 = this;
                BattleMenu.Update(ref bsRef1);
            }
        }
        if (IsHost == false && LockData.Equals("{}") == false
            && ReceivedPokemonData == false && ClientWaitForData == false
            && IsRemoteBattle == true)
        {
            String lockArgument = LockData.Remove(LockData.Length - 1, 1).Remove(0, 1);
            BattleQuery.Clear();
            BattleQuery.Add(FocusBattle());
            BattleQuery.Insert(0, new ToggleMenuQueryObject(true));

            if (StringHelper.IsNumeric(lockArgument) == true)
            {
                SendClientCommand("MOVE|" + (int)int.Parse(lockArgument));
            }
            else
            {
                SendClientCommand("TEXT|" + lockArgument);
            }
            LockData = "{}";
        }

        BasicEffectWithAlphaTest? _lightEffect = Screen.Effect;
        if (_lightEffect != null) { Lighting.UpdateLighting(ref _lightEffect); Screen.Effect = _lightEffect; }
        if (IsCurrentScreen() == true
            || Core.CurrentScreen.Identification == Identifications.ChatScreen)
        {
            Camera.Update();
            Level.Update();
            SkyDome.Update();
        }

        TextBox.Update();
        if (TextBox.Showing == false)
        {
            int cIndex = 0;
            while (cIndex < BattleQuery.Count)
            {
                QueryObject cQueryObject = BattleQuery[cIndex];
                cQueryObject.Update(this);
                if (cQueryObject.IsReady == true)
                {
                    BattleQuery.RemoveAt(cIndex);
                    if (cQueryObject.PassThis == true)
                    {
                        continue;
                    }
                }
                else
                {
                    if (cQueryObject.PassThis == true)
                    {
                        cIndex += 1;
                        continue;
                    }
                }
                break;
            }

            if (HasToWaitPVP() == false)
            {
                if (BattleMenu.Visible == true)
                {
                    BattleScreen bsRef2 = this;
                    BattleMenu.Update(ref bsRef2);
                }
            }
        }

        bool canEnd = true;
        Screen.Identifications[] blockInteractScreens =
        {
            Screen.Identifications.PartyScreen,
            Screen.Identifications.SummaryScreen,
            Screen.Identifications.PauseScreen,
            Screen.Identifications.ChatScreen,
        };
        if (blockInteractScreens.Contains(Core.CurrentScreen.Identification) == true)
        {
            canEnd = false;
        }
        if (canEnd == true)
        {
            if (GameController.IS_DEBUG_ACTIVE == true || Core.Player.SandBoxMode == true)
            {
                if (KeyBoardHandler.KeyPressed(Keys.K) == true)
                {
                    Battle.Won = true;
                    EndBattle(false);
                }
            }
        }

        if (BattleQuery.Count == 0)
        {
            for (int i = 0; i <= 99; i++) { InsertCasualCameramove(); }
        }

        Screen.Level.World.Initialize(Screen.Level.EnvironmentType,
            World.GetWeatherTypeFromWeather(Screen.Level.World.CurrentMapWeather));
    }

    // ----------------------------------------------------------------
    //  Camera helpers
    // ----------------------------------------------------------------

    public void InsertCasualCameramove()
    {
        if (_lastCameraSettings.Count == CAMERA_SETTING_COUNT)
        {
            _lastCameraSettings.Clear();
        }

        int r = Core.Random.Next(0, CAMERA_SETTING_COUNT);
        while (_lastCameraSettings.Contains(r) == true || _lastCamera == r)
        {
            r = Core.Random.Next(0, CAMERA_SETTING_COUNT);
        }
        _lastCameraSettings.Add(r);
        _lastCamera = r;

        CameraQueryObject? q = null;

        switch (r)
        {
            case 0:
                q = new CameraQueryObject(new Vector3(17, 1, 15), new Vector3(9, 1, 15),
                    0.01f, 0.01f, 1.2f, -1.2f, -0.3f, -0.3f, 0.003f, 0.003f);
                break;
            case 1:
                q = new CameraQueryObject(new Vector3(10.3f, 0.5f, 10), new Vector3(17, 0.5f, 10),
                    0.01f, 0.01f, MathHelper.Pi + 0.5f, MathHelper.Pi - 0.5f,
                    -0.1f, -0.1f, 0.0015f, 0.0015f);
                break;
            case 2:
                q = new CameraQueryObject(new Vector3(14, 0, 11), new Vector3(14, 0, 15),
                    0.01f, 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0.0f, 0.0f);
                break;
            case 3:
                q = new CameraQueryObject(new Vector3(13, 0, 12), new Vector3(17, 0, 12),
                    0.01f, 0.01f, MathHelper.PiOver2 + 0.4f, MathHelper.PiOver2,
                    0.0f, 0.0f, 0.001f, 0.001f);
                break;
        }

        if (q != null) { BattleQuery.Add(q); }
    }

    public QueryObject FocusSelfPokemon()
    {
        float posOffsetY = 0.0f;
        if (SelfPokemonNPC!.Model != null) { posOffsetY = 0.5f; }
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(SelfPokemonNPC!.Position.X + 1.0f,
                        SelfPokemonNPC.Position.Y + 0.5f + posOffsetY,
                        SelfPokemonNPC.Position.Z + 1.0f) - BattleMapOffset,
            Screen.Camera.Position,
            0.06f, 0.06f, (float)(MathHelper.PiOver4) + 0.05f, Screen.Camera.Yaw,
            -0.3f, Screen.Camera.Pitch, 0.04f, 0.04f);
        return q;
    }

    public QueryObject FocusOpponentPokemon()
    {
        float posOffsetY = 0.0f;
        if (OpponentPokemonNPC!.Model != null) { posOffsetY = 0.5f; }
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(OpponentPokemonNPC!.Position.X - 1.0f,
                        OpponentPokemonNPC.Position.Y + 0.5f + posOffsetY,
                        OpponentPokemonNPC.Position.Z + 1.0f) - BattleMapOffset,
            Screen.Camera.Position,
            0.06f, 0.06f, -(float)(MathHelper.PiOver4) - 0.05f, Screen.Camera.Yaw,
            -0.3f, Screen.Camera.Pitch, 0.04f, 0.04f);
        return q;
    }

    public QueryObject FocusOwnPlayer()
    {
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(11, 0.0f, 13.5f), Screen.Camera.Position,
            0.1f, 0.1f, (float)MathHelper.PiOver4, Screen.Camera.Yaw,
            -0.1f, Screen.Camera.Pitch, 0.04f, 0.04f);
        return q;
    }

    public QueryObject FocusOppPlayer()
    {
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(15, 0.0f, 13.5f), Screen.Camera.Position,
            0.1f, 0.1f, -(MathHelper.Pi * 0.5f), Screen.Camera.Yaw,
            -0.1f, Screen.Camera.Pitch, 0.04f, 0.04f);
        return q;
    }

    public QueryObject FocusBattle()
    {
        CameraQueryObject q = new CameraQueryObject(
            new Vector3(13.5f, 0.5f, 15.0f), Screen.Camera.Position,
            0.1f, 0.1f, 0, Screen.Camera.Yaw,
            -0.1f, Screen.Camera.Pitch, 0.04f, 0.04f);
        return q;
    }

    // ----------------------------------------------------------------
    //  EndBattle
    // ----------------------------------------------------------------

    public void EndBattle(bool blackout)
    {
        // Revert battle-only Pokemon forms and fire EndBattle ability hooks.
        foreach (Pokemon p in Core.Player.Pokemons)
        {
            String str = p.AdditionalData.ToLower();
            switch (str)
            {
                case "mega":
                case "mega_x":
                case "mega_y":
                case "primal":
                case "blade":
                    p.AdditionalData = String.Empty;
                    p.ReloadDefinitions();
                    p.CalculateStats();
                    if (str.Equals("blade") == false) { p.RestoreAbility(); }
                    break;
            }
            if (p.Ability != null) { p.Ability.EndBattle(p); }
        }

        // Remove fainted Pokémon when DeathInsteadOfFaint rule is on.
        if (GameModeManager.GetGameRuleValue("DeathInsteadOfFaint", "0").Equals("1"))
        {
            for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
            {
                if (i <= Core.Player.Pokemons.Count - 1)
                {
                    if (Core.Player.Pokemons[i].HP <= 0
                        || Core.Player.Pokemons[i].Status == Pokemon.StatusProblems.Fainted)
                    {
                        Core.Player.Pokemons.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
        }

        // Handle roaming pokemon.
        if (RoamingBattle == true)
        {
            if (FieldEffects.RoamingFled == false && Battle.Fled == false
                && Battle.Caught == true
                || OpponentPokemon!.HP <= 0
                || OpponentPokemon!.Status == Pokemon.StatusProblems.Fainted)
            {
                Core.Player.RoamingPokemonData =
                    P3D.RoamingPokemon.RemoveRoamingPokemon(RoamingPokemonStorage!);
            }
            else
            {
                Core.Player.RoamingPokemonData =
                    P3D.RoamingPokemon.ReplaceRoamingPokemon(RoamingPokemonStorage!);
            }
            if (RoamingPokemonStorage!.ScriptPath.Equals(String.Empty) == false)
            {
                ((OverworldScreen)SavedOverworld.OverworldScreen!).AfterRoamingBattleScript =
                    RoamingPokemonStorage.ScriptPath;
            }
            P3D.RoamingPokemon.ShiftRoamingPokemon(
                int.Parse(RoamingPokemonStorage.WorldID.Equals(String.Empty) ? "0" : RoamingPokemonStorage.WorldID));
        }

        Battle.Fled = false;

        // Record visited pokefile.
        if (IsTrainerBattle == false)
        {
            if (TempPokeFile.Equals(String.Empty) == false)
            {
                if (Core.Player.PokeFiles.Contains(TempPokeFile.ToLower()) == false)
                {
                    Core.Player.PokeFiles.Add(TempPokeFile.ToLower());
                }
            }
        }
        TempPokeFile = String.Empty;

        // Send game-state messages to server.
        if (IsRemoteBattle == false)
        {
            if (ConnectScreen.Connected == true)
            {
                if (Battle.Won == false)
                {
                    if (IsTrainerBattle == true)
                    {
                        Core.ServersManager.ServerConnection.SendGameStateMessage(
                            "got defeated by " + Trainer!.TrainerType + " " + Trainer.Name + ".");
                    }
                    else
                    {
                        Core.ServersManager.ServerConnection.SendGameStateMessage(
                            "got defeated by a wild " + OpponentPokemon!.GetDisplayName() + ".");
                    }
                }
            }
        }
        else
        {
            if (IsHost == true)
            {
                if (Battle.Won == false)
                {
                    Core.ServersManager.ServerConnection.SendGameStateMessage(
                        "hosted a battle: \"Player " + Core.Player.Name
                        + " got defeated by Player " + Trainer!.Name + "\".");
                }
                else
                {
                    Core.ServersManager.ServerConnection.SendGameStateMessage(
                        "hosted a battle: \"Player " + Trainer!.Name
                        + " got defeated by Player " + Core.Player.Name + "\".");
                }
            }
            else
            {
                Battle.Won = ClientWonBattle;
            }
            PVPLobbyScreen.SetupBattleResults(this);
        }

        if (CanBlackout == false) { blackout = false; }

        if (blackout == false)
        {
            ResetVars();

            if (IsTrainerBattle == true)
            {
                ActionScript.RegisterID("trainer_" + Trainer!.TrainerFile);
            }

            if (BattleMode != BattleModes.PvP)
            {
                Abilities.HoneyGather.GatherHoney();
                Abilities.Pickup.TryPickup();
            }

            bool hasLevelUp = false;
            String itemReturnScript = "@Text.Show(";
            int pokeIndex = -1;
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                pokeIndex += 1;
                if (p.hasLeveledUp == true) { hasLevelUp = true; }

                if (IsRemoteBattle == true || IsPVPBattle == true)
                {
                    // Remote/PvP: restore original items.
                    if (p.OriginalItem != null)
                    {
                        p.Item = p.OriginalItem.IsGameModeItem == true
                            ? Item.GetItemByID(p.OriginalItem.gmID)
                            : Item.GetItemByID(p.OriginalItem.ID.ToString());
                        p.Item!.AdditionalData = p.OriginalItem.AdditionalData;
                        if (itemReturnScript.Equals("@Text.Show(") == false)
                        {
                            itemReturnScript += "*";
                        }
                        itemReturnScript += Core.Player.Name + " received~"
                            + p.OriginalItem.OneLineName() + "*and gave it back to~"
                            + p.GetDisplayName() + "!";
                        p.OriginalItem = null;
                    }
                    else
                    {
                        if (p.Item != null
                            && FieldEffects.StolenFromOpponentItems.Count > 0)
                        {
                            foreach (int k in FieldEffects.StolenFromOpponentItems.Keys.ToList())
                            {
                                String stolenItemID = FieldEffects.StolenFromOpponentItems[k].IsGameModeItem == false
                                    ? FieldEffects.StolenFromOpponentItems[k].ID.ToString()
                                    : FieldEffects.StolenFromOpponentItems[k].gmID.ToString();
                                String itemID = p.Item.IsGameModeItem == false
                                    ? p.Item.ID.ToString()
                                    : p.Item.gmID.ToString();
                                if (itemID.Equals(stolenItemID))
                                {
                                    FieldEffects.StolenFromOpponentItems.Remove(k);
                                    p.Item = null;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Local: handle item-swap resolution (own Pokémon may have swapped items).
                    if (p.OriginalItem != null
                        && FieldEffects.StolenFromSelfItems.Count > 0)
                    {
                        for (int otherIndex = 0; otherIndex <= Core.Player.Pokemons.Count - 1; otherIndex++)
                        {
                            if (Core.Player.Pokemons[otherIndex].Item != null)
                            {
                                String otherItemID = GetItemIDString(Core.Player.Pokemons[otherIndex].Item);
                                String otherOriginalItemID = GetItemIDString(Core.Player.Pokemons[otherIndex].OriginalItem);
                                String itemID = GetItemIDString(p.Item);
                                String originalItemID = GetItemIDString(p.OriginalItem);

                                if (itemID.Equals(otherItemID) == false
                                    && otherOriginalItemID.Equals(originalItemID) == false
                                    && otherItemID.Equals(originalItemID)
                                    && otherItemID.Equals(otherOriginalItemID) == false)
                                {
                                    FieldEffects.StolenFromSelfItems.Remove(pokeIndex);
                                    if (p.Item == null)
                                    {
                                        p.Item = Core.Player.Pokemons[otherIndex].Item;
                                        p.OriginalItem = null;
                                        if (itemReturnScript.Equals("@Text.Show(") == false)
                                        {
                                            itemReturnScript += "*";
                                        }
                                        itemReturnScript += Core.Player.Name + " gave the~"
                                            + p.Item!.OneLineName() + " back to~"
                                            + p.GetDisplayName() + "!";
                                        Core.Player.Pokemons[otherIndex].Item = null;
                                    }
                                    else
                                    {
                                        if (otherOriginalItemID.Equals(String.Empty) == false
                                            && itemID.Equals(otherOriginalItemID))
                                        {
                                            foreach (int k in FieldEffects.StolenFromSelfItems.Keys.ToList())
                                            {
                                                Item i2 = FieldEffects.StolenFromSelfItems[k];
                                                String kItemID = i2.IsGameModeItem == true
                                                    ? i2.gmID : i2.ID.ToString();
                                                if (kItemID.Equals(otherOriginalItemID))
                                                {
                                                    FieldEffects.StolenFromSelfItems.Remove(k);
                                                    break;
                                                }
                                            }
                                            p.Item = Core.Player.Pokemons[otherIndex].Item;
                                            Core.Player.Pokemons[otherIndex].Item =
                                                Core.Player.Pokemons[otherIndex].OriginalItem;
                                            p.OriginalItem = null;
                                            Core.Player.Pokemons[otherIndex].OriginalItem = null;
                                            if (itemReturnScript.Equals("@Text.Show(") == false)
                                            {
                                                itemReturnScript += "*";
                                            }
                                            itemReturnScript += Core.Player.Name + " gave the~"
                                                + p.Item!.OneLineName() + " back to~"
                                                + p.GetDisplayName() + "~and gave the "
                                                + Core.Player.Pokemons[otherIndex].Item!.OneLineName()
                                                + "~back to "
                                                + Core.Player.Pokemons[otherIndex].GetDisplayName()
                                                + "!";
                                        }
                                        else
                                        {
                                            String addItemID = p.Item.IsGameModeItem == true
                                                ? p.Item.gmID : p.Item.ID.ToString();
                                            Core.Player.Inventory.AddItem(addItemID, 1);
                                            if (itemReturnScript.Equals("@Text.Show("))
                                            {
                                                itemReturnScript = String.Empty;
                                            }
                                            if (itemReturnScript.Equals(String.Empty) == false)
                                            {
                                                itemReturnScript += ")" + Environment.NewLine;
                                            }
                                            itemReturnScript += "@Sound.Play(Receive_Item)"
                                                + Environment.NewLine + "@Text.Show("
                                                + Core.Player.Name + " found~" + p.Item.OneLineName()
                                                + "!*"
                                                + Core.Player.Inventory.GetMessageReceive(p.Item, 1);
                                            p.Item = Core.Player.Pokemons[otherIndex].Item;
                                            Core.Player.Pokemons[otherIndex].Item = null;
                                            p.OriginalItem = null;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // Opponent stole item — try to recover from StolenFromOwnItems.
                    if (p.OriginalItem != null
                        && FieldEffects.StolenFromSelfItems.Count > 0)
                    {
                        foreach (int k in FieldEffects.StolenFromSelfItems.Keys.ToList())
                        {
                            String stolenItemID = FieldEffects.StolenFromSelfItems[k].IsGameModeItem == false
                                ? FieldEffects.StolenFromSelfItems[k].ID.ToString()
                                : FieldEffects.StolenFromSelfItems[k].gmID.ToString();
                            String itemID = p.OriginalItem.IsGameModeItem == false
                                ? p.OriginalItem.ID.ToString()
                                : p.OriginalItem.gmID.ToString();
                            if (itemID.Equals(stolenItemID))
                            {
                                FieldEffects.StolenFromSelfItems.Remove(k);
                                if (p.Item == null)
                                {
                                    p.Item = p.OriginalItem.IsGameModeItem == true
                                        ? Item.GetItemByID(p.OriginalItem.gmID.ToString())
                                        : Item.GetItemByID(p.OriginalItem.ID.ToString());
                                    p.Item!.AdditionalData = p.OriginalItem.AdditionalData;
                                    if (itemReturnScript.Equals("@Text.Show(") == false)
                                    {
                                        itemReturnScript += "*";
                                    }
                                    itemReturnScript += Core.Player.Name + " found~"
                                        + p.OriginalItem.OneLineName() + "*and gave it back to~"
                                        + p.GetDisplayName() + "!";
                                    p.OriginalItem = null;
                                }
                                else
                                {
                                    String addItemID = p.Item.IsGameModeItem == true
                                        ? p.Item.gmID : p.Item.ID.ToString();
                                    Core.Player.Inventory.AddItem(addItemID, 1);
                                    if (itemReturnScript.Equals("@Text.Show("))
                                    {
                                        itemReturnScript = String.Empty;
                                    }
                                    if (itemReturnScript.Equals(String.Empty) == false)
                                    {
                                        itemReturnScript += ")" + Environment.NewLine;
                                    }
                                    itemReturnScript += "@Sound.Play(Receive_Item)"
                                        + Environment.NewLine + "@Text.Show("
                                        + Core.Player.Name + " found~" + p.Item.OneLineName()
                                        + "!*"
                                        + Core.Player.Inventory.GetMessageReceive(p.Item, 1);
                                    p.Item = p.OriginalItem.IsGameModeItem == true
                                        ? Item.GetItemByID(p.OriginalItem.gmID.ToString())
                                        : Item.GetItemByID(p.OriginalItem.ID.ToString());
                                    p.Item!.AdditionalData = p.OriginalItem.AdditionalData;
                                    if (itemReturnScript.Equals("@Text.Show(") == false)
                                    {
                                        itemReturnScript += "*";
                                    }
                                    itemReturnScript += Core.Player.Name + " found~"
                                        + p.Item.OneLineName() + "*and gave it back to~"
                                        + p.GetDisplayName() + "!";
                                    p.OriginalItem = null;
                                }
                                break;
                            }
                        }
                    }

                    // Own Pokémon stole from opponent — remove from stolen list.
                    if (p.Item != null && FieldEffects.StolenFromOpponentItems.Count > 0)
                    {
                        foreach (int k in FieldEffects.StolenFromOpponentItems.Keys.ToList())
                        {
                            String stolenItemID = FieldEffects.StolenFromOpponentItems[k].IsGameModeItem == false
                                ? FieldEffects.StolenFromOpponentItems[k].ID.ToString()
                                : FieldEffects.StolenFromOpponentItems[k].gmID.ToString();
                            String itemID = p.Item.IsGameModeItem == false
                                ? p.Item.ID.ToString()
                                : p.Item.gmID.ToString();
                            if (itemID.Equals(stolenItemID))
                            {
                                FieldEffects.StolenFromOpponentItems.Remove(k);
                            }
                            break;
                        }
                    }

                    // OriginalItem not found anywhere — just give it back.
                    if (p.OriginalItem != null && p.Item == null)
                    {
                        p.Item = p.OriginalItem.IsGameModeItem == true
                            ? Item.GetItemByID(p.OriginalItem.gmID)
                            : Item.GetItemByID(p.OriginalItem.ID.ToString());
                        p.Item!.AdditionalData = p.OriginalItem.AdditionalData;
                        if (itemReturnScript.Equals("@Text.Show(") == false)
                        {
                            itemReturnScript += "*";
                        }
                        if (IsTrainerBattle == true)
                        {
                            itemReturnScript += Core.Player.Name + " received~"
                                + p.OriginalItem.OneLineName() + "*and gave it back to~"
                                + p.GetDisplayName() + "!";
                        }
                        else
                        {
                            itemReturnScript += Core.Player.Name + " found~"
                                + p.OriginalItem.OneLineName() + "*and gave it back to~"
                                + p.GetDisplayName() + "!";
                        }
                        p.OriginalItem = null;
                    }
                }

                p.ResetTemp();
            }

            if (itemReturnScript.Equals("@Text.Show(") == false)
            {
                itemReturnScript += ")";
                String s = "version=2" + Environment.NewLine
                    + itemReturnScript + Environment.NewLine + ":end";
                ((OverworldScreen)SavedOverworld.OverworldScreen!).ActionScript
                    .StartScript(s, 2, false);
            }

            if (hasLevelUp == false)
            {
                Core.SetScreen(new TransitionScreen(
                    this, SavedOverworld.OverworldScreen!, new Color(255, 255, 255),
                    false, ChangeSavedScreen));
            }
            else
            {
                List<int> evolvePokeList = [];
                for (int i = 0; i <= Core.Player.Pokemons.Count - 1; i++)
                {
                    Pokemon p = Core.Player.Pokemons[i];
                    if (p.hasLeveledUp == true && p.evolutionConditions.Count > 0)
                    {
                        p.hasLeveledUp = false;
                        if (p.CanEvolve(EvolutionCondition.EvolutionTrigger.LevelUp, String.Empty) == true)
                        {
                            evolvePokeList.Add(i);
                        }
                    }
                }
                if (evolvePokeList.Count == 0)
                {
                    Core.SetScreen(new TransitionScreen(
                        this, SavedOverworld.OverworldScreen!, new Color(255, 255, 255),
                        false, ChangeSavedScreen));
                }
                else
                {
                    Core.SetScreen(new TransitionScreen(
                        this,
                        new EvolutionScreen(Core.CurrentScreen, evolvePokeList,
                            String.Empty, EvolutionCondition.EvolutionTrigger.LevelUp, true),
                        Color.Black, false));
                }
            }

            // Shedinja berry randomization.
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                if (p.Number == 213)
                {
                    if (p.Item != null && p.Item.IsBerry == true)
                    {
                        if (Core.Random.Next(0, 3) == 0)
                        {
                            p.Item = Item.GetItemByID("139");
                        }
                    }
                }
            }

            FieldEffects.RageFistPower.Self = 0;
            FieldEffects.RageFistPower.Opponent = 0;
        }
        else
        {
            // Blackout branch: restore items then go to blackout screen.
            String itemReturnScript = "@Text.Show(";
            foreach (Pokemon p in Core.Player.Pokemons)
            {
                if (IsRemoteBattle == true)
                {
                    if (p.OriginalItem != null)
                    {
                        p.Item = p.OriginalItem.IsGameModeItem == true
                            ? Item.GetItemByID(p.OriginalItem.gmID)
                            : Item.GetItemByID(p.OriginalItem.ID.ToString());
                        p.Item!.AdditionalData = p.OriginalItem.AdditionalData;
                        if (itemReturnScript.Equals(String.Empty) == false)
                        {
                            itemReturnScript += "*";
                        }
                        itemReturnScript += Core.Player.Name + " received~"
                            + p.OriginalItem.OneLineName() + "*and gave it back to~"
                            + p.GetDisplayName() + "!";
                        p.OriginalItem = null;
                    }
                }
                else
                {
                    if (p.OriginalItem != null)
                    {
                        if (p.Item == null)
                        {
                            p.Item = p.OriginalItem.IsGameModeItem == true
                                ? Item.GetItemByID(p.OriginalItem.gmID.ToString())
                                : Item.GetItemByID(p.OriginalItem.ID.ToString());
                            p.Item!.AdditionalData = p.OriginalItem.AdditionalData;
                            if (itemReturnScript.Equals(String.Empty) == false)
                            {
                                itemReturnScript += "*";
                            }
                            itemReturnScript += Core.Player.Name + " found~"
                                + p.OriginalItem.OneLineName() + "*and gave it back to~"
                                + p.GetDisplayName() + "!";
                            p.OriginalItem = null;
                        }
                        else
                        {
                            String addItemID = p.OriginalItem.IsGameModeItem == true
                                ? p.OriginalItem.gmID : p.OriginalItem.ID.ToString();
                            Core.Player.Inventory.AddItem(addItemID, 1);
                            if (itemReturnScript.Equals(String.Empty) == false)
                            {
                                itemReturnScript += ")" + Environment.NewLine;
                            }
                            itemReturnScript += "@Sound.Play(Receive_Item)"
                                + Environment.NewLine + "@Text.Show("
                                + Core.Player.Name + " found~" + p.OriginalItem.OneLineName()
                                + "!*"
                                + Core.Player.Inventory.GetMessageReceive(p.OriginalItem, 1);
                            p.OriginalItem = null;
                        }
                    }
                }
                p.ResetTemp();
            }

            if (itemReturnScript.Equals("@Text.Show(") == false)
            {
                itemReturnScript += ")";
                String s = "version=2" + Environment.NewLine
                    + itemReturnScript + Environment.NewLine + ":end";
                ((OverworldScreen)SavedOverworld.OverworldScreen!).ActionScript
                    .StartScript(s, 2, false);
            }

            FieldEffects.RageFistPower.Self = 0;
            FieldEffects.RageFistPower.Opponent = 0;

            ResetVars();
            if (SavedOverworld.Level.BlackOutScript.Equals(String.Empty) == false)
            {
                ((OverworldScreen)SavedOverworld.OverworldScreen!).ActionScript
                    .StartScript(SavedOverworld.Level.BlackOutScript, 0, false);
                Core.SetScreen(new TransitionScreen(
                    this, SavedOverworld.OverworldScreen!, Color.Black, false,
                    ChangeSavedScreen));
            }
            else
            {
                Core.SetScreen(new TransitionScreen(
                    this, new BlackOutScreen(this), Color.Black, false));
            }
        }

        BattleMapOffset = Vector3.Zero;
        SelfLeadIndex = 0;
        OpponentLeadIndex = 0;

        BackgroundTarget.Dispose();
        NPCTarget.Dispose();
    }

    /// <summary>Helper: returns the item's ID string (gmID if GameMode item, else numeric ID).</summary>
    private static String GetItemIDString(Item? item)
    {
        if (item == null) { return String.Empty; }
        return item.IsGameModeItem == true ? item.gmID : item.ID.ToString();
    }

    // ----------------------------------------------------------------
    //  ChangeSavedScreen — restores overworld state after transition
    // ----------------------------------------------------------------

    public void ChangeSavedScreen()
    {
        Screen.Level = SavedOverworld.Level;
        Screen.Camera = SavedOverworld.Camera;
        Screen.Effect = SavedOverworld.Effect;
        Screen.SkyDome = SavedOverworld.SkyDome;
        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    // ----------------------------------------------------------------
    //  Trainer helpers
    // ----------------------------------------------------------------

    public bool TrainerHasFightablePokemon()
    {
        foreach (Pokemon p in Trainer!.Pokemons)
        {
            if (p.Status != Pokemon.StatusProblems.Fainted)
            {
                return true;
            }
        }
        return false;
    }

    public void SendInNewTrainerPokemon(int index)
    {
        int i = index;
        if (i == -1)
        {
            if (IsPVPBattle == true)
            {
                i = 0;
                while (Trainer!.Pokemons[i].Status == Pokemon.StatusProblems.Fainted
                       || OpponentPokemonIndex == i
                       || Trainer.Pokemons[i].HP <= 0)
                {
                    i += 1;
                }
            }
            else
            {
                if (NextPokemonIndex != -1)
                {
                    i = NextPokemonIndex;
                }
                else
                {
                    i = Core.Random.Next(0, Trainer!.Pokemons.Count);
                    while (Trainer.Pokemons[i].Status == Pokemon.StatusProblems.Fainted
                           || OpponentPokemonIndex == i
                           || Trainer.Pokemons[i].HP <= 0)
                    {
                        i = Core.Random.Next(0, Trainer.Pokemons.Count);
                    }
                }
            }
        }

        OpponentPokemonIndex = i;
        OpponentPokemon = Trainer!.Pokemons[i];

        String dexID = PokemonForms.GetPokemonDataFileName(
            OpponentPokemon.Number, OpponentPokemon.AdditionalData);
        if (dexID.Contains("_") == false)
        {
            if (PokemonForms.GetAdditionalDataForms(OpponentPokemon.Number) != null
                && PokemonForms.GetAdditionalDataForms(OpponentPokemon.Number)!
                    .Contains(OpponentPokemon.AdditionalData))
            {
                dexID = OpponentPokemon.Number + ";" + OpponentPokemon.AdditionalData;
            }
            else
            {
                dexID = OpponentPokemon.Number.ToString();
            }
        }
        if (Pokedex.GetEntryType(Core.Player.PokedexData, dexID) == 0)
        {
            Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, dexID, 1);
        }
        NextPokemonIndex = -1;
    }

    // ----------------------------------------------------------------
    //  GetModelName
    // ----------------------------------------------------------------

    public String GetModelName(bool own)
    {
        if (Core.Player.ShowModelsInBattle == false || IsRemoteBattle == true)
        {
            return String.Empty;
        }
        Pokemon? poke = own == true ? SelfPokemon : OpponentPokemon;
        if (poke == null) { return String.Empty; }

        String n = PokemonForms.GetAnimationName(poke);
        String s = poke.IsShiny == true ? "Shiny" : "Normal";
        String p = @"Models\Pokemon\" + n + @"\" + s;

        if (ModelManager.ModelExist(p) == true)
        {
            return p;
        }
        return String.Empty;
    }

    // ----------------------------------------------------------------
    //  ResetVars (static)
    // ----------------------------------------------------------------

    public static void ResetVars()
    {
        CanCatch = true;
        CanRun = true;
        CanAlwaysRun = false;
        CanGainLoseMoney = true;
        CanBlackout = true;
        CanReceiveEXP = true;
        RoamingBattle = false;
        CanUseItems = true;
        DiveBattle = false;
        IsInverseBattle = false;
        CustomBattleMusic = String.Empty;
        RoamingPokemonStorage = null;
        if (Core.CurrentScreen.Identification == Identifications.BattleScreen
            || Core.CurrentScreen.Identification == Identifications.BattleCatchScreen)
        {
            BattleScreen bs = (BattleScreen)Core.CurrentScreen;
            if (bs.SavedOverworld.Level.BattleVariables.Equals(String.Empty) == false)
            {
                Screen.Level.SetBattleVariables(bs.SavedOverworld.Level.BattleVariables);
            }
        }
    }

    // ----------------------------------------------------------------
    //  GetTrainerMoney
    // ----------------------------------------------------------------

    public int GetTrainerMoney()
    {
        int money = Trainer!.Money;

        if (BattleScreen.CanGainLoseMoney == false) { money = 0; }

        money += FieldEffects.PayDayCounter.Self;

        if (FieldEffects.AmuletCoin > 0) { money *= 2; }

        foreach (MysteryEvent mysteryEvent
                 in MysteryEventScreen.ActivatedMysteryEvents)
        {
            if (mysteryEvent.EventType == MysteryEventScreen.EventTypes.MoneyMultiplier)
            {
                money = (int)(money * float.Parse(
                    mysteryEvent.Value.Replace(".", GameController.DecSeparator)));
            }
        }

        return money;
    }

    // ----------------------------------------------------------------
    //  AddToQuery
    // ----------------------------------------------------------------

    public void AddToQuery(int index, QueryObject o)
    {
        if (index == -1)
        {
            BattleQuery.Add(o);
        }
        else
        {
            BattleQuery.Insert(index, o);
        }
    }

    // ----------------------------------------------------------------
    //  Networking — PvP helpers
    // ----------------------------------------------------------------

    public bool HasToWaitPVP()
    {
        if (IsPVPBattle == true && IsRemoteBattle == true)
        {
            if (IsHost == true)
            {
                if (ReceivedInput.Equals(String.Empty)) { return true; }
            }
            else
            {
                if (ClientWaitForData == true) { return true; }
                if (ReceivedQuery.Equals(String.Empty) && SentInput == true) { return true; }
            }
        }
        return false;
    }

    private bool CheckNetworkPlayer()
    {
        if (IsRemoteBattle == true)
        {
            if (Core.ServersManager.ServerConnection.Connected == true)
            {
                bool partnerOnServer = false;
                foreach (P3D.Servers.Player p in Core.ServersManager.PlayerCollection)
                {
                    if (p.ServersID == PartnerNetworkID)
                    {
                        partnerOnServer = true;
                        break;
                    }
                }
                if (partnerOnServer == false)
                {
                    PVPLobbyScreen.StoppedBattle = true;
                    PVPLobbyScreen.DisconnectMessage = "The other player disconnected."
                        + Environment.NewLine + Environment.NewLine + "Press any key to exit.";
                    PVPLobbyScreen.ScreenState = PVPLobbyScreen.ScreenStates.Stopped;
                    Battle.Won = true;
                    EndBattle(false);
                    PVPLobbyScreen.BattleSuccessful = false;
                    return false;
                }
            }
            else
            {
                PVPLobbyScreen.StoppedBattle = true;
                PVPLobbyScreen.DisconnectMessage = "You got disconnected from the server."
                    + Environment.NewLine + Environment.NewLine + "Press any key to exit.";
                PVPLobbyScreen.ScreenState = PVPLobbyScreen.ScreenStates.Stopped;
                Battle.Won = false;
                EndBattle(false);
                PVPLobbyScreen.BattleSuccessful = false;
                return false;
            }
        }
        return true;
    }

    // ---- Client-side networking ----

    public void SendClientCommand(String c)
    {
        Core.ServersManager.ServerConnection.SendPackage(
            new P3D.Servers.Package(
                P3D.Servers.Package.PackageTypes.BattleClientData,
                Core.ServersManager.ID,
                P3D.Servers.Package.ProtocolTypes.TCP,
                new List<String> { PartnerNetworkID.ToString(), c }));
        SentInput = true;
        Logger.Debug("[Battle]: Sent Client command");
    }

    public static void ReceiveHostEndRoundData(String data)
    {
        List<String> newQueries = [];
        String tempData = String.Empty;
        String cData = data;

        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            if (System.IO.Directory.Exists(GameController.GamePath + @"\PvP Log\") == false)
            {
                System.IO.Directory.CreateDirectory(GameController.GamePath + @"\PvP Log\");
            }
            String shownData = data.Replace("}{", "}" + Environment.NewLine + "{")
                .Replace("}|{", "}|" + Environment.NewLine + Environment.NewLine + "{");
            System.IO.File.WriteAllText(
                GameController.GamePath + @"\PvP Log\HostEndRoundData.dat", shownData);
        }

        while (cData.Length > 0)
        {
            if (cData[0].ToString().Equals("|")
                && tempData.Length > 0
                && tempData[tempData.Length - 1].ToString().Equals("}"))
            {
                newQueries.Add(tempData);
                tempData = String.Empty;
            }
            else
            {
                tempData += cData[0].ToString();
            }
            cData = cData.Remove(0, 1);
        }
        if (tempData.StartsWith("{") == true && tempData.EndsWith("}") == true)
        {
            newQueries.Add(tempData);
        }

        Screen s = Core.CurrentScreen;
        while (s.PreScreen != null && s.Identification != Identifications.BattleScreen)
        {
            s = s.PreScreen;
        }

        if (s.Identification == Identifications.BattleScreen)
        {
            BattleScreen bs = (BattleScreen)s;
            bs.LockData = newQueries[0];
            bs.OpponentStatistics.FromString(newQueries[1]);
            bs.SelfStatistics.FromString(newQueries[2]);
            bs.OpponentPokemon = Pokemon.GetPokemonByData(newQueries[3]);
            bs.SelfPokemon = Pokemon.GetPokemonByData(newQueries[4]);

            String weatherInfo = newQueries[5].Remove(newQueries[5].Length - 1, 1).Remove(0, 1);
            bs.FieldEffects.Weather = (BattleWeather.WeatherTypes)int.Parse(weatherInfo);

            String canSwitchInfo = newQueries[6].Remove(newQueries[6].Length - 1, 1).Remove(0, 1);
            bs.FieldEffects.ClientCanSwitch = bool.Parse(canSwitchInfo);

            for (int i = 0; i <= 6; i++) { newQueries.RemoveAt(0); }

            int ownCount = Core.Player.Pokemons.Count;
            int oppCount = bs.Trainer!.Pokemons.Count;

            bs.Trainer.Pokemons.Clear();
            Core.Player.Pokemons.Clear();

            for (int i = 0; i <= oppCount - 1; i++)
            {
                bs.Trainer.Pokemons.Add(Pokemon.GetPokemonByData(newQueries[i]));
                if (bs.Trainer.Pokemons[bs.Trainer.Pokemons.Count - 1]
                    .GetSaveData().Equals(bs.OpponentPokemon!.GetSaveData()))
                {
                    bs.OpponentPokemonIndex = bs.Trainer.Pokemons.Count - 1;
                }
            }
            for (int i = oppCount; i <= oppCount + ownCount - 1; i++)
            {
                Core.Player.Pokemons.Add(Pokemon.GetPokemonByData(newQueries[i]));
                if (Core.Player.Pokemons[Core.Player.Pokemons.Count - 1]
                    .GetSaveData().Equals(bs.SelfPokemon!.GetSaveData()))
                {
                    bs.SelfPokemonIndex = Core.Player.Pokemons.Count - 1;
                }
            }

            Logger.Debug("[Battle]: Received Host End Round data");
            bs.ReceivedPokemonData = true;
        }
    }

    public static void ReceiveHostData(String data)
    {
        List<String> newQueries = [];
        String tempData = String.Empty;
        String cData = data;

        Screen s = Core.CurrentScreen;
        while (s.PreScreen != null && s.Identification != Identifications.BattleScreen)
        {
            s = s.PreScreen;
        }

        if (s.Identification == Identifications.BattleScreen
            && GameController.IS_DEBUG_ACTIVE == true)
        {
            if (System.IO.Directory.Exists(GameController.GamePath + @"\PvP Log\") == false)
            {
                System.IO.Directory.CreateDirectory(GameController.GamePath + @"\PvP Log\");
            }
            String shownData = data.Replace("}{", "}" + Environment.NewLine + "{")
                .Replace("}|{", "}|" + Environment.NewLine + Environment.NewLine + "{");
            System.IO.File.WriteAllText(
                GameController.GamePath + @"\PvP Log\HostData.dat", shownData);
        }

        while (cData.Length > 0)
        {
            if (cData[0].ToString().Equals("|")
                && tempData.Length > 0
                && tempData[tempData.Length - 1].ToString().Equals("}"))
            {
                newQueries.Add(tempData);
                tempData = String.Empty;
            }
            else
            {
                tempData += cData[0].ToString();
            }
            cData = cData.Remove(0, 1);
        }
        if (tempData.StartsWith("{") == true && tempData.EndsWith("}") == true)
        {
            newQueries.Add(tempData);
        }

        if (s.Identification == Identifications.BattleScreen)
        {
            BattleScreen bs = (BattleScreen)s;
            bs.BattleQuery.Clear();
            foreach (String q in newQueries)
            {
                QueryObject? query = QueryObject.FromString(q);
                if (query != null) { bs.BattleQuery.Add(query); }
            }
            for (int i = 0; i <= 99; i++) { bs.InsertCasualCameramove(); }

            foreach (QueryObject q in bs.BattleQuery)
            {
                if (q.QueryType == QueryObject.QueryTypes.Textbox)
                {
                    if (((TextQueryObject)q).Text.Equals("You lost the battle!"))
                    {
                        bs.ClientWonBattle = false;
                    }
                }
            }
        }
        Logger.Debug("[Battle]: Received Host data (movie)");
        ReceivedQuery = data;
    }

    // ---- Host-side networking ----

    public static void ReceiveClientData(String data)
    {
        Logger.Debug("[Battle]: Received Client data");
        ReceivedInput = data;

        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            if (System.IO.Directory.Exists(GameController.GamePath + @"\PvP Log\") == false)
            {
                System.IO.Directory.CreateDirectory(GameController.GamePath + @"\PvP Log\");
            }
            String shownData = data.Replace("}{", "}" + Environment.NewLine + "{")
                .Replace("}|{", "}|" + Environment.NewLine + Environment.NewLine + "{");
            System.IO.File.WriteAllText(
                GameController.GamePath + @"\PvP Log\ClientCommand.dat", shownData);
        }

        Screen s = Core.CurrentScreen;
        while (s.PreScreen != null && s.Identification != Identifications.BattleScreen)
        {
            s = s.PreScreen;
        }

        BattleScreen bvScreen = (BattleScreen)s;
        bvScreen.BattleMenu.Visible = false;

        if ((bvScreen.OppFaint && bvScreen.IsRemoteBattle) == false)
        {
            if (bvScreen.HasSwitchedOwn == false)
            {
                bvScreen.Battle.StartMultiTurnAction(bvScreen);
            }
        }
        else
        {
            bvScreen.BattleMenu.Visible = true;
        }
    }

    public void SendEndRoundData()
    {
        String lockData = "{}";
        BattleRoundConst oppStep = Battle.GetOppStep(this, Battle.OwnStep);
        if (Battle.selectedMoveOpp == false)
        {
            if (oppStep.StepType.Equals(BattleRoundConst.StepTypes.Move))
            {
                lockData = "{" + ((Attack)oppStep.Argument!).ID + "}";
            }
            else
            {
                lockData = "{" + oppStep.Argument + "}";
            }
        }

        String d = lockData + "|"
            + SelfStatistics + "|" + OpponentStatistics + "|"
            + SelfPokemon!.GetSaveData() + "|" + OpponentPokemon!.GetSaveData() + "|"
            + "{" + (int)FieldEffects.Weather + "}" + "|"
            + "{" + BattleCalculation.CanSwitch(this, false) + "}";

        foreach (Pokemon p in Core.Player.Pokemons)
        {
            if (d.Equals(String.Empty) == false) { d += "|"; }
            d += p.GetSaveData();
        }
        foreach (Pokemon p in Trainer!.Pokemons)
        {
            if (d.Equals(String.Empty) == false) { d += "|"; }
            d += p.GetSaveData();
        }

        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            if (System.IO.Directory.Exists(GameController.GamePath + @"\PvP Log\") == false)
            {
                System.IO.Directory.CreateDirectory(GameController.GamePath + @"\PvP Log\");
            }
            String shownData = d.Replace("}{", "}" + Environment.NewLine + "{")
                .Replace("}|{", "}|" + Environment.NewLine + Environment.NewLine + "{");
            System.IO.File.WriteAllText(
                GameController.GamePath + @"\PvP Log\SentEndRoundData.dat", shownData);
        }
        Logger.Debug("[Battle]: Sent End Round data");
        Core.ServersManager.ServerConnection.SendPackage(
            new P3D.Servers.Package(
                P3D.Servers.Package.PackageTypes.BattlePokemonData,
                Core.ServersManager.ID,
                P3D.Servers.Package.ProtocolTypes.TCP,
                new List<String> { PartnerNetworkID.ToString(), d }));
    }

    public void SendHostQuery()
    {
        String d = String.Empty;
        List<QueryObject> sendQuery = [];

        for (int i = 0; i <= BattleQuery.Count - 1; i++)
        {
            if (TempPVPBattleQuery.ContainsKey(i) == false)
            {
                sendQuery.Add(BattleQuery[i]);
            }
            else
            {
                sendQuery.Add(TempPVPBattleQuery[i]);
            }
        }

        foreach (QueryObject q in sendQuery)
        {
            if (d.Equals(String.Empty) == false) { d += "|"; }
            d += q.ToString();
        }

        Logger.Debug("[Battle]: Sent Host Query");
        if (GameController.IS_DEBUG_ACTIVE == true)
        {
            if (System.IO.Directory.Exists(GameController.GamePath + @"\PvP Log\") == false)
            {
                System.IO.Directory.CreateDirectory(GameController.GamePath + @"\PvP Log\");
            }
            String shownData = d.Replace("}{", "}" + Environment.NewLine + "{")
                .Replace("}|{", "}|" + Environment.NewLine + Environment.NewLine + "{");
            System.IO.File.WriteAllText(
                GameController.GamePath + @"\PvP Log\SentHostQuery.dat", shownData);
        }
        Core.ServersManager.ServerConnection.SendPackage(
            new P3D.Servers.Package(
                P3D.Servers.Package.PackageTypes.BattleHostData,
                Core.ServersManager.ID,
                P3D.Servers.Package.ProtocolTypes.TCP,
                new List<String> { PartnerNetworkID.ToString(), d }));
        SentHostData = true;
        TempPVPBattleQuery.Clear();
    }

    // ----------------------------------------------------------------
    //  GameJolt sprite downloading
    // ----------------------------------------------------------------

    private void DownloadOnlineSprites()
    {
        if (Core.Player.IsGameJoltSave == true) { DownloadSprites(); }
    }

    private void DownloadSprites()
    {
        SelfTrainerNPC!.SetupSprite(SelfTrainerNPC.TextureID, Core.GameJoltSave.GameJoltID, true);
        if (PVPGameJoltID.Equals(String.Empty) == false)
        {
            OpponentTrainerNPC!.SetupSprite(
                OpponentTrainerNPC.TextureID, PVPGameJoltID, true);
        }
    }

    // ----------------------------------------------------------------
    //  Profiles and Targets
    // ----------------------------------------------------------------

    public PokemonProfile? GetProfile(PokemonTarget target)
    {
        foreach (PokemonProfile p in Profiles)
        {
            if (p.FieldPosition == target) { return p; }
        }
        return null;
    }

    // ----------------------------------------------------------------
    //  new IsCurrentScreen (hides base)
    // ----------------------------------------------------------------

    public new bool IsCurrentScreen() => Core.CurrentScreen == this;
}
