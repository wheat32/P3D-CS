using P3D;

namespace P3D.BattleSystem;

public class BattleAnimationScreenV2 : Screen
{
    public BattleAnimationScreenV2(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.BattleAnimationScreen;
    }

    public override void Draw()
    {
        DrawBackgroundLayer();
        DrawBackLayer();
        DrawMainLayer();
        DrawFrontLayer1();
        DrawFrontLayer2();
    }

    public override void Update()
    {
    }

    private void DrawBackgroundLayer() { }
    private void DrawBackLayer() { }
    private void DrawMainLayer() { }
    private void DrawFrontLayer1() { }
    private void DrawFrontLayer2() { }

    public void SetBackground() { }
}
