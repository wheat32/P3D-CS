namespace VoltorbFlip;

// TODO Phase 6: full VoltorbFlipScreen port
public class VoltorbFlipScreen : P3D.Screen
{
    public static int CurrentLevel = 1;
    public static int PreviousLevel = 1;
    public static int ConsecutiveWins;
    public static int TotalFlips;
    public static int CurrentCoins;
    public static int TotalCoins = -1;

    public VoltorbFlipScreen(P3D.Screen preScreen)
    {
        PreScreen = preScreen;
        Identification = P3D.Screen.Identifications.VoltorbFlipScreen;
    }
}
