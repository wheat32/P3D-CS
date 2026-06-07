using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// TODO Phase 6: full screen ports

public class SplashScreen : Screen
{
    public SplashScreen(GameController game)
    {
        Identification = Identifications.SplashScreen;
    }
}

public class PressStartScreen : Screen
{
    public PressStartScreen()
    {
        Identification = Identifications.PressStartScreen;
    }
}

public class PauseScreen : Screen
{
    public PauseScreen(Screen preScreen)
    {
        Identification = Identifications.PauseScreen;
        PreScreen = preScreen;
    }
}

public class ChatScreen : Screen
{
    public ChatScreen(Screen preScreen)
    {
        Identification = Identifications.ChatScreen;
        PreScreen = preScreen;
        CanChat = false;
    }

    public static void DrawNewMessages() { }
}

public class ConnectScreen : Screen
{
    public static bool Connected;

    public ConnectScreen()
    {
        Identification = Identifications.ConnectScreen;
    }

    public static void UpdateConnectSet() { }
}

public class JoinServerScreen : Screen
{
    public static bool Online;
    public static Server? SelectedServer;

    public JoinServerScreen()
    {
        Identification = Identifications.JoinServerScreen;
    }
}

public class OverworldScreen : Screen
{
    public class Title
    {
        public String Text = "";
        public float Delay;
        public Color TextColor = Color.White;
        public float Scale = 1.0f;
        public bool IsCentered;
        public Vector2 Position = Vector2.Zero;

        public Title()
        {
        }

        public Title(String text, float duration, Color color, float scale, Vector2 offset, bool centered)
        {
            Text = text;
            Delay = duration;
            TextColor = color;
            Scale = scale;
            Position = offset;
            IsCentered = centered;
        }
    }

    public ActionScript ActionScript { get; } = new ActionScript(null);
    public bool TrainerEncountered { get; set; }
    public List<Title> Titles { get; } = [];
    public List<NotificationPopup> NotificationPopupList { get; } = [];

    public static int FadeValue;
    public static Color FadeColor = Color.Black;
    public static int DrawRodID = -1;

    public OverworldScreen()
    {
        Identification = Identifications.OverworldScreen;
    }
}

// TODO Phase 4: full OverworldCamera port
public class OverworldCamera : Camera
{
    public enum CameraFocusTypes
    {
        Player = 0,
        NPC = 1,
        Entity = 2
    }

    public bool ThirdPerson;
    public Vector3 ThirdPersonOffset;
    public Vector3 CPosition { get; set; }
    public bool YawLocked { get; set; }
    public bool _debugWalk;
    public bool Fixed;
    public bool PreventMovement;
    public float _moved;
    public CameraFocusTypes CameraFocusType = CameraFocusTypes.Player;
    public int CameraFocusID = -1;

    public OverworldCamera() : base("Overworld") { }

    public float GetAimYawFromDirection(int direction)
    {
        return direction * (MathHelper.Pi / 2f);
    }

    public void SetThirdPerson(bool enabled, bool reset) { }
    public void UpdateThirdPersonCamera() { }
    public void UpdateFrustum() { }
    public void UpdateViewMatrix() { }
    public void SetupFocus(CameraFocusTypes focusType, int id) { }
}

// TODO Phase 5: full BattleCamera port
public class BattleCamera : Camera
{
    public Vector3 CPosition { get; set; }
    public BattleCamera() : base("Battle") { }
}

public class MapPreviewScreen : Screen
{
    public static bool MapViewMode;

    public MapPreviewScreen()
    {
        Identification = Identifications.MapPreviewScreen;
    }

    public static void DetectMapPath(String path) { }
}

// Stubs for types referenced by screen stubs

public class Server
{
    public String GetName() => "";
    public String GetAddressString() => "";
}

// TODO Phase 6: full InputScreen port
public class InputScreen : Screen
{
    public static String LastInput = "";

    public enum InputModes
    {
        Text = 0,
        Numbers = 1,
        Name = 2,
        Pokemon = 3
    }

    public bool PasswordMode;

    public delegate void ConfirmInput(String input);

    public InputScreen(Screen currentScreen, String defaultName, InputModes inputMode,
                       String currentText, int maxChars, List<Texture2D> sprites,
                       ConfirmInput? confirmSub = null)
    {
        PreScreen = currentScreen;
        Identification = Identifications.InputScreen;
    }
}

// TODO Phase 6/8: full MysteryEvent and MysteryEventScreen port
public class MysteryEventScreen : Screen
{
    public enum EventTypes { MoneyMultiplier, ExtraMove, ExtraItem, Unknown }

    public static int CoinsGained;
    public static List<MysteryEvent> ActivatedMysteryEvents { get; } = [];
    public MysteryEventScreen() { }
    public MysteryEventScreen(Screen preScreen) { PreScreen = preScreen; }
}

public class MysteryEvent
{
    public MysteryEventScreen.EventTypes EventType { get; set; }
    public String Value { get; set; } = "";
}

// ---- Notification popup ----

public class NotificationPopup
{
    public DateTime _delayDate;

    public void Setup(String message) { }
    public void Setup(String message, int delay) { }
    public void Setup(String message, int delay, int backgroundID) { }
    public void Setup(String message, int delay, int backgroundID, int iconID) { }
    public void Setup(String message, int delay, int backgroundID, int iconID, String sfxName) { }
    public void Setup(String message, int delay, int backgroundID, int iconID, String sfxName, String script) { }
    public void Setup(String message, int delay, int backgroundID, int iconID, String sfxName, String script, bool force) { }
}

// ---- Item-related Phase 6 screen stubs ----

public class PartyScreen : Screen
{
    public Screens.UI.ISelectionScreen.ScreenMode Mode { get; set; }
    public bool CanExit { get; set; }
    public String EvolutionItemID { get; set; } = "";
    public String SelectButtonText { get; set; } = "";
    public event Action<Object[]>? SelectedObject;
    public Action? ExitedSub;

    public static int Selected = -1;

    public PartyScreen(Screen preScreen, Items.Item item, Func<int, bool>? onSelect, String title, bool forUse)
    {
        Identification = Identifications.PartyScreen;
        PreScreen = preScreen;
    }

    public PartyScreen(Screen preScreen, Items.Item item, Func<int, bool>? onSelect, String title,
                       bool canExit, bool canChooseFainted, bool canChooseEgg)
    {
        Identification = Identifications.PartyScreen;
        PreScreen = preScreen;
    }

    public void SetupLearnAttack(BattleSystem.Attack attack, int slot, Items.Item? item) { }
}

public class NewInventoryScreen : Screen
{
    public static String SelectedItem = "";

    public NewInventoryScreen()
    {
        Identification = Identifications.InventoryScreen;
    }

    public NewInventoryScreen(Screen preScreen, List<String> allowedItems, bool forScript)
    {
        Identification = Identifications.InventoryScreen;
        PreScreen = preScreen;
    }

    public NewInventoryScreen(Screen preScreen, int[] allowedPages, Object? dummy,
                               List<String> allowedItems, bool forScript)
    {
        Identification = Identifications.InventoryScreen;
        PreScreen = preScreen;
    }

    public void LoadItems() { }
}

public class LearnAttackScreen : Screen
{
    public LearnAttackScreen(Screen preScreen, Pokemon pokemon, BattleSystem.Attack attack)
    {
        PreScreen = preScreen;
    }

    public LearnAttackScreen(Screen preScreen, Pokemon pokemon, BattleSystem.Attack attack, String itemID)
    {
        PreScreen = preScreen;
    }
}

public class EvolutionScreen : Screen
{
    public EvolutionScreen(Screen preScreen, List<int> pokemonIndices, String itemID,
                           EvolutionCondition.EvolutionTrigger trigger)
    {
        PreScreen = preScreen;
    }

    public EvolutionScreen(Screen preScreen, List<int> pokemonIndices, String evolutionArg,
                           EvolutionCondition.EvolutionTrigger trigger, bool arg)
    {
        PreScreen = preScreen;
    }
}

public class TransitionScreen : Screen
{
    public TransitionScreen(Screen preScreen, Screen nextScreen, Color color, bool fadeIn)
    {
        PreScreen = preScreen;
    }

    public TransitionScreen(Screen preScreen, Screen nextScreen, Color color, bool fadeIn, int fadeSpeed)
    {
        PreScreen = preScreen;
    }
}

// EvolutionCondition is defined in Pokemon/Monster/EvolutionCondition.cs

public class MailSystemScreen : Screen
{
    public MailSystemScreen(Screen preScreen)
    {
        PreScreen = preScreen;
    }

    public MailSystemScreen(Screen preScreen, String mailID)
    {
        PreScreen = preScreen;
    }
}

public class ChooseAttackScreen : Screen
{
    public static int Selected = -1;

    public ChooseAttackScreen(Screen preScreen, Pokemon pokemon, Items.Item? item,
                               Func<int, bool>? onSelect, String title)
    {
        PreScreen = preScreen;
    }

    public ChooseAttackScreen(Screen preScreen, Pokemon pokemon, bool canUseHM,
                               bool canExit, Object? dummy)
    {
        PreScreen = preScreen;
    }
}

// TODO Phase 6/8: full NPCTradeScreen port
public class NPCTradeScreen : Screen
{
    public NPCTradeScreen(Screen preScreen, Pokemon ownPokemon, Pokemon tradePokemon,
                          String trainerName, String afterTradeMessage)
    {
        PreScreen = preScreen;
    }
}

// TODO Phase 6: full BlackOutScreen port
public class BlackOutScreen : Screen
{
    public BlackOutScreen(Screen preScreen)
    {
        PreScreen = preScreen;
        Identification = Identifications.BlackOutScreen;
    }
}

// TODO Phase 5: full BattleIntroScreen port
public class BattleIntroScreen : Screen
{
    public BattleIntroScreen(Screen preScreen, BattleSystem.BattleScreen battleScreen, int introType)
    {
        PreScreen = preScreen;
    }

    public BattleIntroScreen(Screen preScreen, BattleSystem.BattleScreen battleScreen, int introType,
                              String musicLoop)
    {
        PreScreen = preScreen;
    }

    public BattleIntroScreen(Screen preScreen, BattleSystem.BattleScreen battleScreen,
                              BattleSystem.Trainer trainer, String musicName, int introType)
    {
        PreScreen = preScreen;
    }
}

// ---- Phase 6 screen stubs ----

public class StorageSystemScreen : Screen
{
    public StorageSystemScreen(Screen preScreen) { PreScreen = preScreen; }

    public static List<Pokemon> GetAllBoxPokemon() => [];
    public static void DepositPokemon(Pokemon pokemon) { }
    public static void DepositPokemon(Pokemon pokemon, int boxIndex) { }
}

public class ApricornScreen : Screen
{
    public ApricornScreen(Screen preScreen, String arg) { PreScreen = preScreen; }
}

public class TradeScreen : Screen
{
    public TradeScreen(Screen preScreen, String storeData, bool canBuy, bool canSell,
                       String currencyIndicator, String shopIdentifier)
    {
        PreScreen = preScreen;
    }
}

public class MapScreen : Screen
{
    public MapScreen(Screen preScreen, String startRegion, String[] modes)
    {
        PreScreen = preScreen;
    }

    public MapScreen(Screen preScreen, List<String> regions, int startIndex, String[] modes)
    {
        PreScreen = preScreen;
    }
}

public class DonationScreen : Screen
{
    public DonationScreen(Screen preScreen) { PreScreen = preScreen; }
}

public class NameObjectScreen : Screen
{
    public NameObjectScreen(Screen preScreen, Pokemon pokemon)
    {
        PreScreen = preScreen;
    }

    public NameObjectScreen(Screen preScreen, Texture2D sprite,
                             bool arg1, bool arg2, String type, String defaultName,
                             Action<String>? confirmSub)
    {
        PreScreen = preScreen;
    }
}

// ---- Additional Phase 6 stubs ----

public class CreditsScreen : Screen
{
    public CreditsScreen(Screen preScreen)
    {
        PreScreen = preScreen;
        Identification = Identifications.CreditsScreen;
    }

    public void InitializeScreen(String ending, bool canBeSkipped) { }
}

public class SecretBaseScreen : Screen
{
    public SecretBaseScreen()
    {
        Identification = Identifications.SecretBaseScreen;
    }
}

public class PVPLobbyScreen : Screen
{
    public PVPLobbyScreen(Screen preScreen, int mode, bool arg)
    {
        PreScreen = preScreen;
        Identification = Identifications.PVPLobbyScreen;
    }
}

public class HatchEggScreen : Screen
{
    public HatchEggScreen(Screen preScreen, List<Pokemon> eggs, bool canRename, String message)
    {
        PreScreen = preScreen;
        Identification = Identifications.HatchEggScreen;
    }
}

public class TeachMovesScreen : Screen
{
    public static bool LearnedMove;

    public TeachMovesScreen(Screen preScreen, int pokemonIndex)
    {
        PreScreen = preScreen;
        Identification = Identifications.TeachMovesScreen;
    }

    public TeachMovesScreen(Screen preScreen, int pokemonIndex, BattleSystem.Attack[] moves)
    {
        PreScreen = preScreen;
        Identification = Identifications.TeachMovesScreen;
    }
}

public class HallOfFameScreen : Screen
{
    public HallOfFameScreen(Screen preScreen)
    {
        PreScreen = preScreen;
        Identification = Identifications.HallofFameScreen;
    }

    public HallOfFameScreen(Screen preScreen, int type)
    {
        PreScreen = preScreen;
        Identification = Identifications.HallofFameScreen;
    }

    public HallOfFameScreen(Screen preScreen, String path)
    {
        PreScreen = preScreen;
        Identification = Identifications.HallofFameScreen;
    }

    public HallOfFameScreen(Screen preScreen, int type, String data)
    {
        PreScreen = preScreen;
        Identification = Identifications.HallofFameScreen;
    }

    public static int GetHallOfFameCount() => 0;
}

// ---- Save / GameJolt helpers ----

public static class SaveGameHelpers
{
    public static bool GameJoltSaveDone() => true;
    public static void ResetSaveCounter() { }
}

// ---- Daycare ----

public static class Daycare
{
    public static Pokemon? ProduceEgg(int daycareID) => null;
    public static void TriggerCall(int daycareID) { }
    public static String CanBreed(int daycareID, bool withMultiplier) => "false";
}

