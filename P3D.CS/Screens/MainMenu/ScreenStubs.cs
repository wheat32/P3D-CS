using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

// SplashScreen is fully ported in Screens/MainMenu/SplashScreen.cs

// PressStartScreen is fully ported in Screens/MainMenu/PressStartScreen.cs

// PauseScreen is fully ported in Screens/MainMenu/PauseScreen.cs

public class ChatScreen : Screen
{
    public ChatScreen(Screen preScreen)
    {
        Identification = Identifications.ChatScreen;
        PreScreen = preScreen;
        CanChat = false;
    }

    public static void DrawNewMessages() { }
    public void EnterPMChat(String username) { }
}

// ConnectScreen is fully ported in Screens/MainMenu/ConnectScreen.cs

// JoinServerScreen is fully ported in Screens/MainMenu/JoinServerScreen.cs

// MapPreviewScreen is fully ported in Screens/MapPreview/MapPreviewScreen.cs

// Stubs for types referenced by screen stubs

public class Server
{
    public String IdentifierName { get; set; } = String.Empty;
    public String GetName() => IdentifierName;
    public String GetAddressString() => String.Empty;
    public override String ToString() => IdentifierName;
}

// InputScreen is fully ported in Screens/InputScreen.cs

// TODO Phase 9: full MysteryEvent and MysteryEventScreen port
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


// PartyScreen is fully ported in Screens/Pokemon/PartyScreen.cs

// NewInventoryScreen is fully ported in Screens/NewInventoryScreen.cs

// LearnAttackScreen is fully ported in Screens/Pokemon/LearnAttackScreen.cs

// EvolutionScreen is fully ported in Screens/Pokemon/EvolutionScreen.cs

// TransitionScreen is fully ported in Screens/TransitionScreen.cs

// EvolutionCondition is defined in Pokemon/Monster/EvolutionCondition.cs

// MailSystemScreen is fully ported in Screens/PC/MailSystemScreen.cs

// ChooseAttackScreen is fully ported in Screens/Pokemon/ChooseAttackScreen.cs

// NPCTradeScreen is fully ported in Screens/NPCTradeScreen.cs

// BattleIntroScreen is fully ported in Screens/Battle/BattleIntroScreen.cs

// ---- Phase 6 screen stubs ----

// StorageSystemScreen is fully ported in Screens/StorageSystemScreen.cs

// ApricornScreen is fully ported in Screens/Inventory/ApricornScreen.cs

// TradeScreen is fully ported in Screens/TradeScreen.cs

// MapScreen is fully ported in Screens/MapScreen.cs

// DonationScreen is fully ported in Screens/DonationScreen.cs

// PokemonStatusScreen is fully ported in Screens/Pokemon/PokemonStatusScreen.cs

// PokedexViewScreen is fully ported in Screens/PokedexScreen.cs

// NameObjectScreen is fully ported in Screens/Pokemon/NameObjectScreen.cs

// ---- Additional Phase 6 stubs ----

// CreditsScreen is fully ported in Screens/Credits/CreditsScreen.cs

// SummaryScreen is fully ported in Screens/Pokemon/SummaryScreen.cs

// NewMenuScreen is fully ported in Screens/NewMenuScreen.cs

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

// HatchEggScreen is fully ported in Screens/Pokemon/HatchEggScreen.cs

// TeachMovesScreen is fully ported in Screens/Pokemon/TeachMovesScreen.cs

// HallOfFameScreen is fully ported in Screens/PC/HallOfFameScreen.cs

// NewOptionScreen is fully ported in Screens/NewOptionScreen.cs

// NewGameScreen is fully ported in Screens/MainMenu/NewGameScreen.cs

// SaveScreen is fully ported in Screens/SaveScreen.cs

// PokedexSelectScreen is fully ported in Screens/PokedexScreen.cs

public class DirectTradeScreen : Screen
{
    public DirectTradeScreen(Screen preScreen, int networkID, bool isRequester) { PreScreen = preScreen; }
}

public class RegisterBattleScreen : Screen
{
    public RegisterBattleScreen(Screen preScreen) { PreScreen = preScreen; }
}

public class WonderTradeScreen : Screen
{
    public WonderTradeScreen(Screen preScreen) { PreScreen = preScreen; }
}

// StatisticsScreen is fully ported in Screens/StatisticsScreen.cs

// ---- Save / GameJolt helpers ----

public static class SaveGameHelpers
{
    public static bool GameJoltSaveDone() => true;
    public static bool StartedDownloadCheck { get; } = false;
    public static bool EncounteredErrors { get; } = false;
    public static void ResetSaveCounter() { }
}

// Daycare is fully ported in Screens/Pokemon/Daycare.cs

