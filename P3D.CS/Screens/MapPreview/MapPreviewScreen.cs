using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class MapPreviewScreen : Screen
{
    public static bool MapViewMode = false;
    public static String MapViewModeMapPath = String.Empty;
    public static Vector3 MapViewModePosition = Vector3.Zero;

    private Texture2D _particlesTexture = null!;

    private struct MapDisplay
    {
        public Vector3 Position;
        public String Text;
        public Color Color;
    }

    private List<MapDisplay> _textDisplays = [];

    public MapPreviewScreen()
    {
        Identification = Identifications.MapPreviewScreen;
        CanChat = false;
        MouseVisible = false;
        CanBePaused = false;
        CanDrawDebug = true;
        CanGoFullscreen = true;
        CanMuteAudio = false;
        CanTakeScreenshot = true;

        Effect = new BasicEffectWithAlphaTest(Core.GraphicsDevice);
        Effect.FogEnabled = true;

        Camera = new MapPreviewCamera();
        SkyDome = new SkyDome();
        Level = new Level();
        Level.Load(MapViewModeMapPath);

        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);

        _particlesTexture = TextureManager.GetTexture("GUI\\Overworld\\Particles");
    }

    public override void Update()
    {
        ControlMap();

        if (Screen.Effect != null) { BasicEffectWithAlphaTest eff = Screen.Effect; Lighting.UpdateLighting(ref eff); Screen.Effect = eff; }
        Camera.Update();
        PickWarp();
        UpdateHiddenItems();
        Level.Update();
        SkyDome.Update();

        Screen.Level.World.Initialize(Screen.Level.EnvironmentType, Screen.Level.WeatherType);
    }

    private void UpdateHiddenItems()
    {
        foreach (Entity e in Screen.Level.Entities)
        {
            if (e.EntityID.ToLower() != "itemobject") continue;
            if (e.Opacity <= 0.0F)
            {
                ItemObject i = (ItemObject)e;
                if (i.IsHiddenItem() == true) i.Opacity = 1.0F;
            }
        }
    }

    private void PickWarp()
    {
        _textDisplays.Clear();
        Ray ray = Screen.Camera.Ray;

        List<Entity> entities = [];
        entities.AddRange(Screen.Level.Entities.ToArray());
        if (Core.GameOptions.LoadOffsetMaps > 0)
            entities.AddRange(Screen.Level.OffsetmapEntities.ToArray());

        foreach (Entity e in entities)
        {
            switch (e.EntityID)
            {
                case "WarpBlock":
                {
                    if (e.Shaders.Count > 0 && e.Shaders.Last().X == 1.51337135F)
                        e.Shaders.RemoveAt(e.Shaders.Count - 1);
                    float? result = ray.Intersects(e.ViewBox);
                    if (result.HasValue == true && result.Value < 12.0F && result.Value > 0.1F)
                    {
                        e.Visible = true;
                        e.Shaders.Add(new Vector3(1.51337135F));
                        _textDisplays.Add(new MapDisplay { Text = "To: " + e.AdditionalValue.GetSplit(0), Position = e.Position, Color = new Color(0, 232, 255, 200) });
                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                            ((WarpBlock)e).Warp(true);
                    }
                    break;
                }
                case "NPC":
                {
                    float? result = ray.Intersects(e.ViewBox);
                    if (result.HasValue == true && result.Value < 4.0F && result.Value > 0.1F)
                    {
                        String t = e.ActionValue switch
                        {
                            0 => ((NPC)e).Name + ": \"" + GetDisplayText(e.AdditionalValue) + "\"",
                            1 => ((NPC)e).Name + ": Script start (" + e.AdditionalValue + ")",
                            3 => ((NPC)e).Name + ": Direct script input",
                            _ => ((NPC)e).Name + ": Script start (" + e.AdditionalValue + ")"
                        };
                        _textDisplays.Add(new MapDisplay { Text = t, Position = e.Position, Color = Color.LightCoral });
                    }
                    break;
                }
                case "ScriptBlock":
                {
                    float? result = ray.Intersects(e.ViewBox);
                    if (result.HasValue == true && result.Value < 4.0F && result.Value > 0.1F)
                    {
                        ScriptBlock s = (ScriptBlock)e;
                        String t = s.GetActivationID() switch
                        {
                            0 => "ScriptBlock: Script start (" + s.ScriptID + ")",
                            1 => "ScriptBlock: \"" + GetDisplayText(s.ScriptID) + "\"",
                            2 => "ScriptBlock: Direct script input",
                            _ => String.Empty
                        };
                        if (t != String.Empty) _textDisplays.Add(new MapDisplay { Text = t, Position = e.Position, Color = Color.LightGreen });
                    }
                    break;
                }
                case "SignBlock":
                {
                    float? result = ray.Intersects(e.ViewBox);
                    if (result.HasValue == true && result.Value < 4.0F && result.Value > 0.1F)
                    {
                        String t = e.ActionValue switch
                        {
                            0 or 3 => "Sign: \"" + GetDisplayText(e.AdditionalValue) + "\"",
                            1 => "Sign: Script start (" + e.AdditionalValue + ")",
                            2 => "Sign: Direct script input",
                            _ => String.Empty
                        };
                        if (t != String.Empty) _textDisplays.Add(new MapDisplay { Text = t, Position = e.Position, Color = Color.LightYellow });
                    }
                    break;
                }
            }
        }
    }

    private String GetDisplayText(String source)
    {
        String text = source.Replace("~", " ").Replace("*", " ");
        if (text.Length > 16) text = text.Remove(16) + "...";
        return text;
    }

    private void ControlMap()
    {
        if (KeyBoardHandler.KeyPressed(KeyBindings.EscapeKey) == true || ControllerHandler.ButtonPressed(Buttons.Start) == true)
            Core.GameInstance.Exit();

        if (Controls.Up(true, true, false, false, false, true) == true) Core.GameOptions.RenderDistance += 1;
        if (Controls.Down(true, true, false, false, false, true) == true) Core.GameOptions.RenderDistance -= 1;
        Core.GameOptions.RenderDistance = Math.Clamp(Core.GameOptions.RenderDistance, 0, 5);

        if (Controls.Left(false, true, false, false, false, true) == true) Core.GameOptions.LoadOffsetMaps -= 1;
        if (Controls.Right(false, true, false, false, false, true) == true) Core.GameOptions.LoadOffsetMaps += 1;
        Core.GameOptions.LoadOffsetMaps = Math.Clamp(Core.GameOptions.LoadOffsetMaps, 0, 100);

        if (KeyBoardHandler.KeyPressed(Keys.R) == true || ControllerHandler.ButtonPressed(Buttons.Back) == true)
        {
            Core.OffsetMaps.Clear();
            Level.Load(Level.LevelFile);
        }

        if (KeyBoardHandler.KeyPressed(KeyBindings.SpecialKey) == true || ControllerHandler.ButtonPressed(Buttons.X) == true)
            Camera.Position = MapViewModePosition;
    }

    public override void Draw()
    {
        SkyDome.Draw(Camera.FOV);
        Level.Draw();
        World.DrawWeather(Screen.Level.World.CurrentMapWeather);

        if (Core.GameOptions.ShowGUI == true)
        {
            Vector2 p = Core.GetMiddlePosition(new Size(16, 16));
            Core.SpriteBatch.Draw(_particlesTexture, new Rectangle((int)p.X, (int)p.Y, 16, 16), new Rectangle(0, 0, 9, 9), Color.White);

            String offsetString = "OFF";
            if (Core.GameOptions.LoadOffsetMaps > 0)
                offsetString = "ON (QUALITY: " + (100 - Core.GameOptions.LoadOffsetMaps).ToString() + ")";

            String t = "MAP: " + Level.LevelFile + Environment.NewLine +
                "LEVEL: " + Level.MapName + Environment.NewLine +
                "RENDERDISTANCE: " + Core.GameOptions.RenderDistance.ToString() + Environment.NewLine +
                "OFFSETMAPS: " + offsetString;
            Core.SpriteBatch.DrawString(FontManager.MiniFont, t, new Vector2(2, Core.windowSize.Height - FontManager.MiniFont.MeasureString(t).Y - 2), Color.White);

            String t2 = "WASD: Move around" + Environment.NewLine +
                "MOUSE SCROLL: Change camera speed" + Environment.NewLine +
                "LEFT MOUSE CLICK: Interact" + Environment.NewLine +
                "ARROW KEYS UP/DOWN: Change RenderDistance" + Environment.NewLine +
                "ARROW KEYS LEFT/RIGHT: Change Offset Map Quality" + Environment.NewLine +
                "R: Reload map" + Environment.NewLine +
                "Q: Replace player" + Environment.NewLine +
                "ESC: Close Map Preview";
            Core.SpriteBatch.DrawString(FontManager.MiniFont, t2, new Vector2(Core.windowSize.Width - FontManager.MiniFont.MeasureString(t2).X - 2, Core.windowSize.Height - FontManager.MiniFont.MeasureString(t2).Y - 2), Color.White);

            String t3 = "MAP PREVIEW MODE";
            Core.SpriteBatch.DrawString(FontManager.MiniFont, t3, new Vector2(Core.windowSize.Width - FontManager.MiniFont.MeasureString(t3).X - 2, 2), Color.White);

            DrawMapDisplays();
        }
    }

    private void DrawMapDisplays()
    {
        foreach (MapDisplay des in _textDisplays)
        {
            Vector2 pt = des.Position.ProjectPoint(Screen.Camera.View, Screen.Camera.Projection);
            pt.X -= FontManager.ChatFont.MeasureString(des.Text).X / 2.0F;

            Core.SpriteBatch.DrawString(FontManager.ChatFont, des.Text, new Vector2(pt.X + 2, pt.Y + 2), new Color(0, 0, 0, 128), 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
            Core.SpriteBatch.DrawString(FontManager.ChatFont, des.Text, pt, des.Color, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
        }
    }

    public static void DetectMapPath(String arg)
    {
        String[] data = arg.Split('|');
        if (data.Length == 1)
        {
            MapViewMode = true;
            MapViewModeMapPath = data[0];
            MapViewModePosition = Vector3.Zero;
        }
        else if (data.Length == 4)
        {
            MapViewMode = true;
            MapViewModeMapPath = data[0];
            MapViewModePosition = new Vector3(
                float.Parse(data[1].Replace(".", GameController.DecSeparator)),
                float.Parse(data[2].Replace(".", GameController.DecSeparator)),
                float.Parse(data[3].Replace(".", GameController.DecSeparator)));
        }
    }
}
