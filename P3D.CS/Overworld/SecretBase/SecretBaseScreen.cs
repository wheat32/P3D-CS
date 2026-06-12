namespace P3D;

public class SecretBaseScreen : Screen
{
    public OverworldStorage SavedOverworld { get; }
    public SecretBase SecretBase { get; }

    public SecretBaseScreen()
    {
        Identification = Identifications.SecretBaseScreen;
        CanBePaused = true;

        SavedOverworld = new OverworldStorage();
        SavedOverworld.SetToCurrentEnvironment();

        SecretBase = new SecretBase();

        Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);

        Camera = new SecretBaseCamera();
        Level = new Level();
        Level.Load("|");

        SecretBase.LoadSecretBaseFromStore(Screen.Level);
        MusicManager.Play(Level.MusicLoop, true, 0.01f);
    }

    public override void Update()
    {
        Camera!.Update();
        Level!.Update();
    }

    public override void Draw()
    {
        TextBox.Draw();
        if (IsCurrentScreen() == true)
        {
            ChooseBox.Draw();
        }
        Level!.Draw();
    }
}
