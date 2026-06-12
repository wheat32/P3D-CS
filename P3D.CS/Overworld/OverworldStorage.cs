namespace P3D;

public class OverworldStorage
{
    public Screen? OverworldScreen { get; set; }
    public Level? Level { get; set; }
    public Camera? Camera { get; set; }
    public BasicEffectWithAlphaTest? Effect { get; set; }
    public SkyDome? SkyDome { get; set; }

    public void SetToCurrentEnvironment()
    {
        Screen s = Core.CurrentScreen;
        while (s.Identification != Screen.Identifications.OverworldScreen && s.PreScreen != null)
        {
            s = s.PreScreen;
        }
        OverworldScreen = s;
        Camera = Screen.Camera;
        Level = Screen.Level;
        Effect = Screen.Effect;
        SkyDome = Screen.SkyDome;
    }
}
