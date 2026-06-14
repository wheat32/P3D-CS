using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;

namespace P3D;

public class ViewModelScreen : Screen
{
    private ModelEntity _model = null!;
    private WallBlock _ground = null!;
    private Camera _previousCamera = null!;

    private bool _normalModel = true;
    private String _pokemonAnimationName = String.Empty;
    private bool _canViewShiny = false;
    private float _turnDelay = 10.0F;

    public ViewModelScreen(Screen currentScreen, String pokemonAnimationName, bool canViewShiny)
    {
        PreScreen = currentScreen;
        Identification = Identifications.ViewModelScreen;

        _model = (ModelEntity)Entity.GetNewEntity("ModelEntity", new Vector3(0), [], [], false, new Vector3(MathHelper.Pi * 0.5F, 0, 0), new Vector3(0.1F), BaseModel.BlockModel, 0, "Models\\" + pokemonAnimationName + "\\Normal", true, new Vector3(1), 0, String.Empty, String.Empty, new Vector3(0), null)!;
        _ground = (WallBlock)Entity.GetNewEntity("WallBlock", new Vector3(0, -0.5F, 0), [TextureManager.GetTexture("Textures\\ModelViewer\\Ground")], [-1, -1, -1, -1, -1, -1, -1, -1, 0, 0], false, new Vector3(0.0F), new Vector3(4.0F, 1.0F, 4.0F), BaseModel.BlockModel, 0, String.Empty, true, new Vector3(1.0F), 0, String.Empty, String.Empty, new Vector3(0.0F), [])!;

        CanBePaused = true;
        MouseVisible = false;

        _previousCamera = Screen.Camera;
        _pokemonAnimationName = pokemonAnimationName;
        _canViewShiny = canViewShiny;

        Screen.Camera = new ViewModelCamera();
    }

    public override void Draw()
    {
        SkyDome.Draw(Camera.FOV);
        _model.Render();

        Screen.Effect!.View = Screen.Camera.View;
        Screen.Effect.Projection = Screen.Camera.Projection;
        _ground.Render();
    }

    public override void Update()
    {
        Camera.Update();
        SkyDome.Update();

        _turnDelay -= 0.1F;

        if (KeyBoardHandler.KeyDown(Keys.Left) == true)
        {
            _model.Rotation.Y -= 0.025F;
            _model.CreatedWorld = false;
            _ground.Rotation.Y -= 0.025F;
            _ground.CreatedWorld = false;
            _turnDelay = 10.0F;
        }
        if (KeyBoardHandler.KeyDown(Keys.Right) == true)
        {
            _model.Rotation.Y += 0.025F;
            _model.CreatedWorld = false;
            _ground.Rotation.Y += 0.025F;
            _ground.CreatedWorld = false;
            _turnDelay = 10.0F;
        }

        if (_turnDelay <= 0.0F)
        {
            _turnDelay = 0.0F;
            _model.Rotation.Y -= 0.015F;
            _model.CreatedWorld = false;
            _ground.Rotation.Y -= 0.015F;
            _ground.CreatedWorld = false;
        }

        if (Controls.Accept(true, true, true) == true && _canViewShiny == true)
        {
            _normalModel = !_normalModel;
            if (_normalModel == false)
            {
                SoundManager.PlaySound("select");
                _model.LoadModel("Models\\" + _pokemonAnimationName + "\\Shiny");
            }
            else
            {
                SoundManager.PlaySound("select");
                _model.LoadModel("Models\\" + _pokemonAnimationName + "\\Normal");
            }
        }

        _model.UpdateEntity();
        _model.Update();

        _ground.UpdateEntity();
        _ground.Update();

        if (Controls.Dismiss(true, true, true) == true)
        {
            Screen.Camera = _previousCamera;
            Core.SetScreen(PreScreen!);
            SoundManager.PlaySound("select");
        }
    }
}
