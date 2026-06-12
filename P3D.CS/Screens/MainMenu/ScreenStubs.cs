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
    public static String LastInput = String.Empty;

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
    public enum EventTypes { MoneyMultiplier, EXPMultiplier, ExtraMove, ExtraItem, Unknown }

    public static int CoinsGained;
    public static List<MysteryEvent> ActivatedMysteryEvents { get; } = [];
    public MysteryEventScreen() { }
    public MysteryEventScreen(Screen preScreen) { PreScreen = preScreen; }
}

public class MysteryEvent
{
    public MysteryEventScreen.EventTypes EventType { get; set; }
    public String Value { get; set; } = String.Empty;
}


// ---- Item-related Phase 6 screen stubs ----

public class PartyScreen : Screen
{
    public Screens.UI.ISelectionScreen.ScreenMode Mode { get; set; }
    public bool CanExit { get; set; }
    public String EvolutionItemID { get; set; } = String.Empty;
    public String SelectButtonText { get; set; } = String.Empty;
    public event Action<Object[]>? SelectedObject;
    public Action? ExitedSub;

    public static int Selected = -1;
    public int CannotChooseIndex = -1;

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
    public static String SelectedItem = String.Empty;
    public Screens.UI.ISelectionScreen.ScreenMode Mode { get; set; }
    public bool CanExit { get; set; }
    public event Action<Object[]>? SelectedObject;

    public NewInventoryScreen()
    {
        Identification = Identifications.InventoryScreen;
    }

    public NewInventoryScreen(Screen preScreen)
    {
        Identification = Identifications.InventoryScreen;
        PreScreen = preScreen;
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

    public TransitionScreen(Screen preScreen, Screen nextScreen, Color color, bool fadeIn, Action afterTransition)
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

// TODO Phase 5: full BattleGrowStatsScreen port
public class BattleGrowStatsScreen : Screen
{
    public BattleGrowStatsScreen(Screen preScreen, Pokemon pokemon, int[] oldStats)
    {
        PreScreen = preScreen;
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

// TODO Phase 6: full SummaryScreen port
public class SummaryScreen : Screen
{
    public SummaryScreen(Screen preScreen, Pokemon[] pokemon, int index)
    {
        PreScreen = preScreen;
        Identification = Identifications.SummaryScreen;
    }
}

// TODO Phase 6: full NewMenuScreen port
public class NewMenuScreen : Screen
{
    public NewMenuScreen(Screen preScreen)
    {
        PreScreen = preScreen;
        Identification = Identifications.NewMenuScreen;
    }
}

public class PVPLobbyScreen : Screen
{
    public enum ScreenStates { Running, Stopped }

    public static bool StoppedBattle;
    public static String DisconnectMessage = String.Empty;
    public static ScreenStates ScreenState = ScreenStates.Running;
    public static bool BattleSuccessful = true;

    public PVPLobbyScreen(Screen preScreen, int mode, bool arg)
    {
        PreScreen = preScreen;
        Identification = Identifications.PVPLobbyScreen;
    }

    public static void SetupBattleResults(BattleSystem.BattleScreen battleScreen) { }
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

