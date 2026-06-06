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
    public ActionScriptRunner ActionScript { get; } = new ActionScriptRunner();

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

    public OverworldCamera() : base("Overworld") { }

    public float GetAimYawFromDirection(int direction)
    {
        return direction * (Microsoft.Xna.Framework.MathHelper.Pi / 2f);
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

public class ActionScriptRunner
{
    public bool IsReady = true;
    public String ScriptName = "";
    public int CurrentLine;
}

public static class ActionScript
{
    public static ActionScriptRunner CSL() => new ActionScriptRunner();
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

// TODO Phase 6: full MysteryEventScreen port
public class MysteryEventScreen : Screen
{
    public static int CoinsGained;
    public MysteryEventScreen() { }
}

// ---- Item-related Phase 6 screen stubs ----

public class PartyScreen : Screen
{
    public Screens.UI.ISelectionScreen.ScreenMode Mode { get; set; }
    public bool CanExit { get; set; }
    public String EvolutionItemID { get; set; } = "";
    public event Action<Object[]>? SelectedObject;

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
