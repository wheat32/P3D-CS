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
        public Title(String text, float duration, Microsoft.Xna.Framework.Color color,
                     float scale, Microsoft.Xna.Framework.Vector2 offset, bool centered)
        {
        }
    }

    public ActionScript ActionScript { get; } = new ActionScript(null);
    public bool TrainerEncountered { get; set; }
    public List<Title> Titles { get; } = [];

    public OverworldScreen()
    {
        Identification = Identifications.OverworldScreen;
    }
}

// TODO Phase 4: full OverworldCamera port
public class OverworldCamera : Camera
{
    public bool ThirdPerson;
    public Microsoft.Xna.Framework.Vector3 ThirdPersonOffset;
    public Microsoft.Xna.Framework.Vector3 CPosition { get; set; }
    public bool YawLocked { get; set; }
    public bool _debugWalk;

    public OverworldCamera() : base("Overworld") { }

    public float GetAimYawFromDirection(int direction)
    {
        return direction * (Microsoft.Xna.Framework.MathHelper.Pi / 2f);
    }

    public void SetThirdPerson(bool enabled, bool reset) { }
    public void UpdateThirdPersonCamera() { }
    public void UpdateFrustum() { }
    public void UpdateViewMatrix() { }
}

// TODO Phase 5: full BattleCamera port
public class BattleCamera : Camera
{
    public Microsoft.Xna.Framework.Vector3 CPosition { get; set; }
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
                       String currentText, int maxChars, List<Microsoft.Xna.Framework.Graphics.Texture2D> sprites,
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
}

public class MysteryEvent
{
    public MysteryEventScreen.EventTypes EventType { get; set; }
    public String Value { get; set; } = "";
}

// ---- Item-related Phase 6 screen stubs ----

public class PartyScreen : Screen
{
    public Screens.UI.ISelectionScreen.ScreenMode Mode { get; set; }
    public bool CanExit { get; set; }
    public String EvolutionItemID { get; set; } = "";
    public event Action<Object[]>? SelectedObject;
    public Action? ExitedSub;

    public PartyScreen(Screen preScreen, Items.Item item, Func<int, bool> onSelect, String title, bool forUse)
    {
        Identification = Identifications.PartyScreen;
        PreScreen = preScreen;
    }

    public void SetupLearnAttack(BattleSystem.Attack attack, int slot, Items.Item item) { }
}

public class NewInventoryScreen : Screen
{
    public NewInventoryScreen() { Identification = Identifications.InventoryScreen; }
    public void LoadItems() { }
}

public class LearnAttackScreen : Screen
{
    public LearnAttackScreen(Screen preScreen, Pokemon pokemon, BattleSystem.Attack attack, String itemID)
    {
        PreScreen = preScreen;
    }
}

public class EvolutionScreen : Screen
{
    public EvolutionScreen(Screen preScreen, List<int> pokemonIndices, String itemID, EvolutionCondition.EvolutionTrigger trigger)
    {
        PreScreen = preScreen;
    }
}

public class TransitionScreen : Screen
{
    public TransitionScreen(Screen preScreen, Screen nextScreen, Microsoft.Xna.Framework.Color color, bool fadeIn)
    {
        PreScreen = preScreen;
    }
}

// EvolutionCondition is defined in Pokemon/Monster/EvolutionCondition.cs

public class MailSystemScreen : Screen
{
    public MailSystemScreen(Screen preScreen, String mailID)
    {
        PreScreen = preScreen;
    }
}

public class ChooseAttackScreen : Screen
{
    public ChooseAttackScreen(Screen preScreen, Pokemon pokemon, Items.Item item, Func<int, bool> onSelect, String title)
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

    public BattleIntroScreen(Screen preScreen, BattleSystem.BattleScreen battleScreen,
                              BattleSystem.Trainer trainer, String musicName, int introType)
    {
        PreScreen = preScreen;
    }
}

// ---- Phase 6 screen stubs (referenced by ScriptV1) ----

public class StorageSystemScreen : Screen
{
    public StorageSystemScreen(Screen preScreen) { PreScreen = preScreen; }
}

public class ApricornScreen : Screen
{
    public ApricornScreen(Screen preScreen, String arg) { PreScreen = preScreen; }
}

public class TradeScreen : Screen
{
    public TradeScreen(Screen preScreen, String storeData, bool canBuy, bool canSell,
                       String currencyIndicator, String extra)
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

    public NameObjectScreen(Screen preScreen, Microsoft.Xna.Framework.Graphics.Texture2D sprite,
                             bool arg1, bool arg2, String type, String defaultName,
                             Action<String>? confirmSub)
    {
        PreScreen = preScreen;
    }
}
