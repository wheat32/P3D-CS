using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class BattleIntroScreen : Screen
{
    private int _oldX, _oldY;

    private int _animationType;
    private List<Rectangle> _animations = [];

    public Screen OldScreen;
    public Screen NewScreen;

    private bool _ready = false;
    private int _value = 0;

    private BattleSystem.Trainer _trainer;

    private float _minDelay = 4.0f;
    private DateTime _startTime;
    private TimeSpan _duration;

    public enum BattleType : int
    {
        PVP = 0,
        TRAINER = 1,
        SAFARI = 2,
        BUG_CATCHING = 3,
        ROAMING = 4,
        WILD = 5,
    }

    public String MusicLoop = String.Empty;

    public BattleIntroScreen(Screen oldScreen, Screen newScreen, int introType)
    {
        String musicLoop = Screen.Level.CurrentRegion.Split(',')[0] + "_wild_intro";
        if (BattleSystem.BattleScreen.CustomBattleMusic != String.Empty || MusicManager.SongExists(BattleSystem.BattleScreen.CustomBattleMusic) == true)
        {
            musicLoop = BattleSystem.BattleScreen.CustomBattleMusic + "_intro";
        }
        else
        {
            if (BattleSystem.BattleScreen.RoamingBattle == true)
            {
                if (BattleSystem.BattleScreen.RoamingPokemonStorage != null && BattleSystem.BattleScreen.RoamingPokemonStorage.MusicLoop != String.Empty)
                {
                    musicLoop = BattleSystem.BattleScreen.RoamingPokemonStorage.MusicLoop + "_intro";
                }
            }
        }

        if (ShouldPlayNightTheme(musicLoop))
        {
            musicLoop = musicLoop + "_night";
        }

        if (MusicManager.SongExists(musicLoop) == false)
        {
            musicLoop = "johto_wild_intro";
        }

        Constructor(oldScreen, newScreen, null!, musicLoop, introType);
    }

    public BattleIntroScreen(Screen oldScreen, Screen newScreen, int introType, String musicLoop)
    {
        if (musicLoop == String.Empty)
        {
            musicLoop = Screen.Level.CurrentRegion.Split(',')[0] + "_wild_intro";
            if (MusicManager.SongExists(musicLoop) == true)
            {
                if (BattleSystem.BattleScreen.RoamingBattle == true)
                {
                    if (BattleSystem.BattleScreen.RoamingPokemonStorage != null && BattleSystem.BattleScreen.RoamingPokemonStorage.MusicLoop != String.Empty)
                    {
                        musicLoop = BattleSystem.BattleScreen.RoamingPokemonStorage.MusicLoop + "_intro";
                    }
                }
            }

            if (ShouldPlayNightTheme(musicLoop))
            {
                musicLoop = musicLoop + "_night";
            }

            if (MusicManager.SongExists(musicLoop) == false)
            {
                musicLoop = "johto_wild_intro";
            }
        }
        Constructor(oldScreen, newScreen, null!, musicLoop, introType);
    }

    public BattleIntroScreen(Screen oldScreen, Screen newScreen, BattleSystem.Trainer trainer, String musicName, int introType)
    {
        Constructor(oldScreen, newScreen, trainer, musicName, introType);
    }

    private void Constructor(Screen oldScreen, Screen newScreen, BattleSystem.Trainer trainer, String musicLoop, int introType)
    {
        OldScreen = oldScreen;
        NewScreen = newScreen;
        CanChat = false;
        CanBePaused = false;
        _trainer = trainer;
        MusicLoop = musicLoop;

        _animationType = introType;
        if (Screen.Level.IsDark == true && _animationType > 4)
        {
            _animationType -= 5;
        }

        Identification = Identifications.BattleIniScreen;
    }

    public override void Draw()
    {
        OldScreen.Draw();

        Color c = Color.Black;
        if (_animationType > 4)
        {
            c = Color.White;
        }

        switch (_animationType)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 5:
            case 6:
            case 7:
            case 8:
                foreach (Rectangle animation in _animations)
                {
                    Canvas.DrawRectangle(animation, c);
                }
                break;
            case 4:
            case 9:
                foreach (Rectangle animation in _animations)
                {
                    Canvas.DrawBorder(_value, animation, c);
                }
                break;
            case 10:
                DrawTrainerIntro();
                break;
            case 11:
                DrawFaceshotIntro();
                break;
            case 12:
                DrawBlurIntro();
                break;
        }
    }

    private Texture2D? _blurTexture = null;
    private List<Rectangle> _blurLayers = [];
    private List<Rectangle> _whiteLayers = [];
    private float _blurDelay = 0.5f;
    private Vector2 _currentBlurPosition = new Vector2(0.0f);
    private float _currentBlurIntensity = 10.0f;
    private int _currentBlurZoom = 0;

    private void DrawBlurIntro()
    {
        if (_blurTexture != null)
        {
            int startIndex = 0;
            if (_blurLayers.Count > 10)
            {
                startIndex = _blurLayers.Count - 10;
            }

            for (int i = startIndex; i <= _blurLayers.Count - 1; i++)
            {
                int usedZoom = _blurLayers[i].Width - Core.windowSize.Width;
                float rot = (float)(Math.Sin(usedZoom * 0.01f) * 0.1f);
                Rectangle r = _blurLayers[i];
                Vector2 origin = new Vector2(Core.windowSize.Width / 2.0f, Core.windowSize.Height / 2.0f);
                Core.SpriteBatch.Draw(_blurTexture, new Rectangle(r.X + (int)(r.Width / 2), r.Y + (int)(r.Height / 2), r.Width, r.Height), null, new Color(255, 255, 255, 100), rot, origin, SpriteEffects.None, 0.0f);
            }

            for (int i = 0; i <= _whiteLayers.Count - 1; i++)
            {
                Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(255, 255, 255, 50));
            }
        }
    }

    private void UpdateBlurIntro()
    {
        if (_blurTexture == null)
        {
            RenderTarget2D r = new RenderTarget2D(Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height);
            Core.GraphicsDevice.SetRenderTarget(r);
            Core.Draw();
            Core.GraphicsDevice.SetRenderTarget(null);
            _blurTexture = r;
        }

        _blurDelay -= 0.1f;
        if (_blurDelay <= 0.0f)
        {
            _blurDelay = 0.5f;
            if (Core.Random.Next(0, 75) < (int)_currentBlurIntensity)
            {
                _whiteLayers.Add(Core.windowSize);
            }
            Vector2 v = new Vector2(
                Core.Random.Next((int)(_currentBlurPosition.X - _currentBlurIntensity), (int)(_currentBlurPosition.X + _currentBlurIntensity)),
                Core.Random.Next((int)(_currentBlurPosition.Y - _currentBlurIntensity), (int)(_currentBlurPosition.Y + _currentBlurIntensity)));

            _blurLayers.Add(new Rectangle((int)(v.X - (_currentBlurZoom / 2)), (int)(v.Y - (_currentBlurZoom / 2)), Core.windowSize.Width + _currentBlurZoom, Core.windowSize.Height + _currentBlurZoom));

            _currentBlurIntensity += 2;
            _currentBlurZoom += 55;
        }

        if (_currentBlurIntensity == 80 || SongOver() == true)
        {
            _ready = true;
        }
    }

    private int _animationAfterReady = 0;

    private void DrawTrainerIntro()
    {
        Vector2 barPosition = new Vector2(_trainer.BarImagePosition.X * 128, _trainer.BarImagePosition.Y * 128);
        Vector2 vsPosition = new Vector2(_trainer.VSImagePosition.X * 128, _trainer.VSImagePosition.Y * 128 + 64);
        Texture2D trainerTexture1 = TextureManager.GetTexture(@"Textures\NPC\" + _trainer.SpriteName);
        Texture2D? trainerTexture2 = null;
        Size trainer1FrameSize;
        Size trainer2FrameSize = new Size(0, 0);

        if (_trainer.VSImageOrigin != "VSIntro")
        {
            vsPosition.Y -= 64;
        }
        if (trainerTexture1.Width == trainerTexture1.Height / 2)
        {
            trainer1FrameSize = new Size((int)(trainerTexture1.Width / 2), (int)(trainerTexture1.Height / 4));
        }
        else if (trainerTexture1.Width == trainerTexture1.Height)
        {
            trainer1FrameSize = new Size((int)(trainerTexture1.Width / 4), (int)(trainerTexture1.Height / 4));
        }
        else
        {
            trainer1FrameSize = new Size((int)(trainerTexture1.Width / 3), (int)(trainerTexture1.Height / 4));
        }
        Texture2D t1 = TextureManager.GetTexture(@"GUI\Intro\VSIntro", new Rectangle((int)barPosition.X, (int)barPosition.Y, 128, 64), String.Empty);
        Texture2D t2 = TextureManager.GetTexture(@"GUI\Intro\" + _trainer.VSImageOrigin, new Rectangle((int)vsPosition.X, (int)vsPosition.Y, _trainer.VSImageSize.Width, _trainer.VSImageSize.Height), String.Empty);
        Texture2D t3 = TextureManager.GetTexture(@"NPC\" + _trainer.SpriteName, new Rectangle(0, trainer1FrameSize.Height * 2, trainer1FrameSize.Width, trainer1FrameSize.Height), String.Empty);
        Texture2D? t4 = null;
        if (_trainer.DoubleTrainer == true)
        {
            trainerTexture2 = TextureManager.GetTexture(@"Textures\NPC\" + _trainer.SpriteName2);
            if (trainerTexture1.Width == trainerTexture1.Height / 2)
            {
                trainer2FrameSize = new Size((int)(trainerTexture2.Width / 2), (int)(trainerTexture2.Height / 4));
            }
            else if (trainerTexture1.Width == trainerTexture1.Height)
            {
                trainer2FrameSize = new Size((int)(trainerTexture2.Width / 4), (int)(trainerTexture2.Height / 4));
            }
            else
            {
                trainer2FrameSize = new Size((int)(trainerTexture2.Width / 3), (int)(trainerTexture2.Height / 4));
            }
            t4 = TextureManager.GetTexture(@"NPC\" + _trainer.SpriteName2, new Rectangle(0, trainer2FrameSize.Height * 2, trainer2FrameSize.Width, trainer2FrameSize.Height), String.Empty);
        }

        if (_trainer.GameJoltID != String.Empty)
        {
            if (GameJolt.Emblem.HasDownloadedSprite(_trainer.GameJoltID) == true)
            {
                Texture2D? t = GameJolt.Emblem.GetOnlineSprite(_trainer.GameJoltID);
                if (t != null)
                {
                    Vector2 spriteSize;
                    if (t.Width == t.Height / 2)
                    {
                        spriteSize = new Vector2((int)(t.Width / 2), (int)(t.Height / 4));
                    }
                    else if (t.Width == t.Height)
                    {
                        spriteSize = new Vector2((int)(t.Width / 4), (int)(t.Height / 4));
                    }
                    else
                    {
                        spriteSize = new Vector2((int)(t.Width / 3), (int)(t.Height / 4));
                    }
                    t3 = TextureManager.GetTexture(t, new Rectangle(0, (int)(spriteSize.Y * 2), (int)spriteSize.X, (int)spriteSize.Y));
                }
            }
        }

        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, (int)((_value / 1140.0) * Core.windowSize.Height * 1.5f)), Color.Black);

        for (int i = -256; i <= Core.windowSize.Width; i += 256)
        {
            int offset = _value + (_animationAfterReady * 7);
            while (offset >= 256) { offset -= 256; }
            Core.SpriteBatch.Draw(t1, new Rectangle((int)(i + offset), (int)(Core.windowSize.Height / 2 - 64), 256, 128), Color.White);
        }

        if (_trainer.DoubleTrainer == true)
        {
            Core.SpriteBatch.Draw(t3, new Rectangle(Core.windowSize.Width - 540, (int)(Core.windowSize.Height / 2 - 105 - (MathHelper.Min(t3.Height * 10, 256) / 2)), MathHelper.Min(t3.Width * 10, 256), MathHelper.Min(t3.Height * 10, 256)), Color.White);
            Core.SpriteBatch.Draw(t4!, new Rectangle(Core.windowSize.Width - 280, (int)(Core.windowSize.Height / 2 - 105 - (MathHelper.Min(t4!.Height * 10, 256) / 2)), MathHelper.Min(t4.Width * 10, 256), MathHelper.Min(t4.Height * 10, 256)), Color.White);

            String secondTrainerSpace = _trainer.TrainerType2 == String.Empty ? String.Empty : " ";
            String t = ReplaceIntroName(_trainer.TrainerType) + " " + ReplaceIntroName(_trainer.Name) + " & " + ReplaceIntroName(_trainer.TrainerType2) + secondTrainerSpace + ReplaceIntroName(_trainer.Name2);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2(Core.windowSize.Width - FontManager.InGameFont.MeasureString(t).X - 50 + 2, (int)(Core.windowSize.Height / 2 + 20 + 2)), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2(Core.windowSize.Width - FontManager.InGameFont.MeasureString(t).X - 50, (int)(Core.windowSize.Height / 2 + 20)), Color.White);
        }
        else
        {
            Core.SpriteBatch.Draw(t3, new Rectangle(Core.windowSize.Width - 310, (int)(Core.windowSize.Height / 2 - 105 - (MathHelper.Min(t3.Height * 10, 256) / 2)), MathHelper.Min(t3.Width * 10, 256), MathHelper.Min(t3.Height * 10, 256)), Color.White);

            String t = ReplaceIntroName(_trainer.TrainerType) + " " + ReplaceIntroName(_trainer.Name);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2(Core.windowSize.Width - FontManager.InGameFont.MeasureString(t).X - 50 + 2, (int)(Core.windowSize.Height / 2 + 20 + 2)), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, t, new Vector2(Core.windowSize.Width - FontManager.InGameFont.MeasureString(t).X - 50, (int)(Core.windowSize.Height / 2 + 20)), Color.White);
        }
        Core.SpriteBatch.Draw(t2, new Rectangle(480 - (int)((int)(1.29 * _value) / 3), (int)(Core.windowSize.Height / 2 - 20) - (int)((int)(1 * _value) / 3), (int)(1.12 * (int)(_value / 1.5f)), 1 * (int)(_value / 1.5f)), Color.White);
    }

    private String ReplaceIntroName(String name)
    {
        String n = name.Replace("<rivalname>", Core.Player.RivalName);
        n = n.Replace("<rival.name>", Core.Player.RivalName);
        n = n.Replace("<playername>", Core.Player.Name);
        n = n.Replace("<player.name>", Core.Player.Name);
        n = n.Replace("[POKE]", "Poké");
        return n;
    }

    private int _blackPosition = 0;
    private int _trainerPosition = 0;
    private int _barOffset = 0;
    private int _textPosition = 0;

    private void DrawFaceshotIntro()
    {
        Vector2 barPosition = new Vector2(_trainer.BarImagePosition.X * 128, _trainer.BarImagePosition.Y * 128);
        Vector2 vsPosition = new Vector2(_trainer.VSImagePosition.X * 128, _trainer.VSImagePosition.Y * 128 + 64);
        Texture2D trainerTexture1 = TextureManager.GetTexture(@"Textures\NPC\" + _trainer.SpriteName);
        Size trainer1FrameSize;

        if (trainerTexture1.Width == trainerTexture1.Height / 2)
        {
            trainer1FrameSize = new Size((int)(trainerTexture1.Width / 2), (int)(trainerTexture1.Height / 4));
        }
        else if (trainerTexture1.Width == trainerTexture1.Height)
        {
            trainer1FrameSize = new Size((int)(trainerTexture1.Width / 4), (int)(trainerTexture1.Height / 4));
        }
        else
        {
            trainer1FrameSize = new Size((int)(trainerTexture1.Width / 3), (int)(trainerTexture1.Height / 4));
        }

        Texture2D t1 = TextureManager.GetTexture(@"GUI\Intro\VSIntro", new Rectangle((int)barPosition.X, (int)barPosition.Y, 128, 64), String.Empty);
        Texture2D t2 = TextureManager.GetTexture(@"GUI\Intro\VSIntro", new Rectangle((int)vsPosition.X, (int)vsPosition.Y, 64, 64), String.Empty);
        Texture2D t3 = TextureManager.GetTexture(trainerTexture1, new Rectangle(0, trainer1FrameSize.Height * 2, trainer1FrameSize.Width, trainer1FrameSize.Height));

        if (_trainer.GameJoltID != String.Empty)
        {
            if (GameJolt.Emblem.HasDownloadedSprite(_trainer.GameJoltID) == true)
            {
                Texture2D? t = GameJolt.Emblem.GetOnlineSprite(_trainer.GameJoltID);
                if (t != null)
                {
                    Vector2 spriteSize;
                    if (t.Width == t.Height / 2)
                    {
                        spriteSize = new Vector2((int)(t.Width / 2), (int)(t.Height / 4));
                    }
                    else if (t.Width == t.Height)
                    {
                        spriteSize = new Vector2((int)(t.Width / 4), (int)(t.Height / 4));
                    }
                    else
                    {
                        spriteSize = new Vector2((int)(t.Width / 3), (int)(t.Height / 4));
                    }
                    t3 = TextureManager.GetTexture(t, new Rectangle(0, (int)(spriteSize.Y * 2), (int)spriteSize.X, (int)spriteSize.Y));
                }
            }
        }

        for (int i = -512; i <= Core.windowSize.Width; i += 512)
        {
            int offset = _barOffset + (_animationAfterReady * 7);
            while (offset >= 512) { offset -= 512; }
            Core.SpriteBatch.Draw(t1, new Rectangle((int)(i + offset), (int)(Core.windowSize.Height / 2 - 128), 512, 256), Color.White);
        }

        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, _blackPosition), Color.Black);
        Canvas.DrawRectangle(new Rectangle(0, Core.windowSize.Height - _blackPosition, Core.windowSize.Width, _blackPosition), Color.Black);
        Core.SpriteBatch.Draw(t3, new Rectangle((int)(Core.windowSize.Width - _trainerPosition), (int)(Core.windowSize.Height / 2 + 128 - (int)(MathHelper.Min(t3.Height * 10, 256) * 0.875f)), MathHelper.Min(t3.Width * 10, 256), (int)(MathHelper.Min(t3.Height * 10, 256) * 0.875f)), new Rectangle(0, 0, t3.Width, (int)(t3.Height * 0.875f)), Color.White);
        Core.SpriteBatch.Draw(t2, new Rectangle((int)(_trainerPosition * 1.5f - 64 * 7), (int)(Core.windowSize.Height / 2) - 192, 64 * 7, 64 * 7), Color.White);

        if (_textPosition > 0)
        {
            int tWidth = (int)(FontManager.InGameFont.MeasureString(_trainer.TrainerType).X * 3.0f);
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _trainer.TrainerType, new Vector2((_textPosition - tWidth).Clamp(-tWidth, (int)(Core.windowSize.Width / 2 - tWidth / 2)), 50), Color.White, 0.0f, new Vector2(0), 3.0f, SpriteEffects.None, 0.0f);
            if (_textPosition > 300)
            {
                tWidth = (int)(FontManager.InGameFont.MeasureString(_trainer.Name).X * 3.0f);
                Core.SpriteBatch.DrawString(FontManager.InGameFont, _trainer.Name, new Vector2((Core.windowSize.Width - (_textPosition - 300)).Clamp((int)(Core.windowSize.Width / 2 - tWidth / 2), Core.windowSize.Width), Core.windowSize.Height - 180), Color.White, 0.0f, new Vector2(0), 3.0f, SpriteEffects.None, 0.0f);
            }
        }
    }

    public override void Update()
    {
        if (_minDelay > 0.0f)
        {
            _minDelay -= 0.1f;
            if (_minDelay <= 0.0f)
            {
                _minDelay = 0.0f;
            }
        }

        if (_ready == true)
        {
            _animationAfterReady += 1;
            if (_minDelay == 0.0f && SongOver())
            {
                Core.SetScreen(NewScreen);
                if (NewScreen.GetType() == typeof(BattleSystem.BattleScreen))
                {
                    BattleSystem.BattleScreen b = (BattleSystem.BattleScreen)NewScreen;

                    if (b.IsPVPBattle == true)
                    {
                        b.InitializePVP(b.Trainer, b.OverworldScreen);
                    }
                    else
                    {
                        if (b.IsTrainerBattle == true)
                        {
                            b.InitializeTrainer(b.Trainer, b.OverworldScreen, b.defaultMapType);
                        }
                        else
                        {
                            if (Screen.Level.IsSafariZone == true)
                            {
                                b.InitializeSafari(b.WildPokemon!, b.OverworldScreen!, b.defaultMapType);
                            }
                            else
                            {
                                if (Screen.Level.IsBugCatchingContest == true)
                                {
                                    b.InitializeBugCatch(b.WildPokemon!, b.OverworldScreen!, b.defaultMapType);
                                }
                                else
                                {
                                    b.InitializeWild(b.WildPokemon!, b.OverworldScreen!, b.defaultMapType);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            switch (_animationType)
            {
                case 0:
                case 5:
                    UpdateRectangleIntro();
                    break;
                case 1:
                case 6:
                    UpdateHorizontalBars();
                    break;
                case 2:
                case 7:
                    UpdateVerticalBars();
                    break;
                case 3:
                case 8:
                    UpdateBlockIn();
                    break;
                case 4:
                case 9:
                    UpdateBlockOut();
                    break;
                case 10:
                    UpdateTrainerVS();
                    break;
                case 11:
                    UpdateFaceshotIntro();
                    break;
                case 12:
                    UpdateBlurIntro();
                    break;
            }
        }

        ResetCursor();
    }

    private void UpdateFaceshotIntro()
    {
        _barOffset += 14;
        _blackPosition = (_blackPosition + 10).Clamp(0, (int)(Core.windowSize.Height / 2 - 128));
        if (_blackPosition >= (int)(Core.windowSize.Height / 2 - 128))
        {
            _trainerPosition = (_trainerPosition + 20).Clamp(0, 420);
            if (_trainerPosition >= 420)
            {
                _textPosition += (int)Math.Ceiling(Core.windowSize.Width / 50.0);
                if (_textPosition >= (int)(Core.windowSize.Width / 2 - (int)(FontManager.InGameFont.MeasureString(_trainer.Name).X) / 2) + 1200)
                {
                    _ready = true;
                }
            }
        }
    }

    private void UpdateRectangleIntro()
    {
        int rectangleSize = GetRectangleSize();

        int fullRecs = (int)Math.Ceiling((double)Core.windowSize.Height / rectangleSize) * (int)Math.Ceiling((double)Core.windowSize.Width / rectangleSize);
        int currentRecs = _animations.Count;

        if (fullRecs > currentRecs)
        {
            bool validPosition = false;
            Vector2 pos = default;

            while (validPosition == false)
            {
                pos = new Vector2(
                    Core.Random.Next(0, (int)Math.Ceiling((double)(Core.windowSize.Width - rectangleSize) / rectangleSize) + 1) * rectangleSize,
                    Core.Random.Next(0, (int)Math.Ceiling((double)(Core.windowSize.Height - rectangleSize) / rectangleSize) + 1) * rectangleSize);

                validPosition = true;

                if (_animations.Count > 0)
                {
                    foreach (Rectangle r in _animations)
                    {
                        if (r.X == pos.X && r.Y == pos.Y)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
            }

            _animations.Add(new Rectangle((int)pos.X, (int)pos.Y, rectangleSize, rectangleSize));
        }
        else
        {
            _ready = true;
        }
    }

    private int GetRectangleSize()
    {
        double blocksOnScreen = 81.6;
        double pixelAmount = Core.windowSize.Width * Core.windowSize.Height;
        double perRectanglePixels = pixelAmount / blocksOnScreen;
        return (int)Math.Ceiling(Math.Sqrt(perRectanglePixels));
    }

    private void UpdateHorizontalBars()
    {
        if (_animations.Count < 20)
        {
            if (Core.Random.Next(0, 4) == 0)
            {
                bool validPosition = false;
                Vector2 pos = default;

                while (validPosition == false)
                {
                    pos = new Vector2(0, (int)(Core.windowSize.Height / 20) * Core.Random.Next(0, 20));

                    validPosition = true;

                    if (_animations.Count > 0)
                    {
                        foreach (Rectangle r in _animations)
                        {
                            if (r.X == pos.X && r.Y == pos.Y)
                            {
                                validPosition = false;
                                break;
                            }
                        }
                    }
                }

                _animations.Add(new Rectangle((int)pos.X, (int)pos.Y, Core.windowSize.Width, (int)(Core.windowSize.Height / 20)));
            }
        }
        else
        {
            _ready = true;
        }
    }

    private void UpdateVerticalBars()
    {
        if (_animations.Count < 20)
        {
            if (Core.Random.Next(0, 4) == 0)
            {
                bool validPosition = false;
                Vector2 pos = default;

                while (validPosition == false)
                {
                    pos = new Vector2((int)(Core.windowSize.Width / 20) * Core.Random.Next(0, 20), 0);

                    validPosition = true;

                    if (_animations.Count > 0)
                    {
                        foreach (Rectangle r in _animations)
                        {
                            if (r.X == pos.X && r.Y == pos.Y)
                            {
                                validPosition = false;
                                break;
                            }
                        }
                    }
                }

                _animations.Add(new Rectangle((int)pos.X, (int)pos.Y, (int)(Core.windowSize.Width / 20), Core.windowSize.Height));
            }
        }
        else
        {
            _ready = true;
        }
    }

    private void UpdateBlockIn()
    {
        if (_animations.Count == 0)
        {
            _animations.Add(new Rectangle((int)(Core.windowSize.Width / 2 - (Core.windowSize.Width / 100 / 2)), (int)(Core.windowSize.Height / 2 - (Core.windowSize.Height / 100 / 2)), (int)(Core.windowSize.Width / 100), (int)(Core.windowSize.Height / 100)));
        }
        else
        {
            int speed = (int)(_duration.TotalMilliseconds / Core.windowSize.Height * 4);
            if (_animations[0].Height >= Core.windowSize.Height + 128)
            {
                _ready = true;
            }
            Rectangle a = _animations[0];

            a.X -= speed;
            a.Y -= (int)(speed / 16 * 9);

            a.Width += speed * 2;
            a.Height += (int)(speed * 2 / 16 * 9);

            _animations.RemoveAt(0);
            _animations.Add(a);
        }
    }

    private void UpdateBlockOut()
    {
        if (_animations.Count == 0)
        {
            _animations.Add(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height));
        }
        else
        {
            if (_value >= Core.windowSize.Height / 2 + 4)
            {
                _ready = true;
            }
            else
            {
                _value += (int)Math.Ceiling(_duration.TotalMilliseconds / Core.windowSize.Height * 3);
            }
        }
    }

    private void UpdateTrainerVS()
    {
        _value += 7;
        if (_value >= 1140)
        {
            _ready = true;
        }
    }

    public void ResetCursor()
    {
        if (GameController.IsActiveWindow() == true)
        {
            Mouse.SetPosition((int)(Core.windowSize.Width / 2), (int)(Core.windowSize.Height / 2));
            _oldX = (int)(Core.windowSize.Width / 2);
            _oldY = (int)(Core.windowSize.Height / 2);
        }
    }

    public override void ChangeTo()
    {
        Player.Temp.IsInBattle = true;
        Player.Temp.BeforeBattlePosition = Screen.Camera.Position;
        Player.Temp.BeforeBattleLevelFile = Screen.Level.LevelFile;
        Player.Temp.BeforeBattleFacing = Screen.Camera.GetPlayerFacingDirection();
        BattleSystem.BattleScreen b = (BattleSystem.BattleScreen)NewScreen;

        MusicManager.Playlist.Clear();
        MusicManager.Stop();
        if (BattleSystem.BattleScreen.CustomBattleMusic == String.Empty || MusicManager.SongExists(BattleSystem.BattleScreen.CustomBattleMusic) == false)
        {
            BattleType battleType = BattleType.WILD;
            if (b.IsPVPBattle == true)
            {
                battleType = BattleType.PVP;
            }
            else
            {
                if (b.IsTrainerBattle == true)
                {
                    battleType = BattleType.TRAINER;
                }
                else if (Screen.Level.IsSafariZone == true)
                {
                    battleType = BattleType.SAFARI;
                }
                else if (Screen.Level.IsBugCatchingContest == true)
                {
                    battleType = BattleType.BUG_CATCHING;
                }
                else
                {
                    if (BattleSystem.BattleScreen.RoamingBattle == true)
                    {
                        battleType = BattleType.ROAMING;
                    }
                    else
                    {
                        battleType = BattleType.WILD;
                    }
                }
            }

            String loopSong = GetLoopSong(battleType);
            MusicManager.Play(MusicLoop, true, 0.0f, true, loopSong);
        }
        else
        {
            MusicManager.Play(MusicLoop, true, 0.0f, true, BattleSystem.BattleScreen.CustomBattleMusic);
        }
        if (MusicLoop != null)
        {
            SongContainer? song = MusicManager.GetSong(MusicLoop);
            if (song != null && song.Duration.TotalSeconds <= 1)
            {
                _duration = new TimeSpan(0);
                _minDelay = 0;
                _ready = true;
            }
            else if (song != null)
            {
                _duration = song.Duration;
            }
            else
            {
                _duration = new TimeSpan(0);
                _minDelay = 0;
                _ready = true;
            }
        }
        else
        {
            _duration = new TimeSpan(0);
            _minDelay = 0;
            _ready = true;
        }
        _startTime = DateTime.Now;
    }

    private String GetLoopSong(BattleType battleType)
    {
        String fallbackLoopSong = "johto_wild";
        String loopSong = Screen.Level.CurrentRegion.Split(',')[0] + "_wild";
        if (battleType == BattleType.PVP)
        {
            loopSong = "pvp";
        }
        else if (battleType == BattleType.TRAINER)
        {
            fallbackLoopSong = _trainer.GetBattleMusicName();
            loopSong = _trainer.GetBattleMusicName();
        }
        else if (battleType == BattleType.SAFARI)
        {
            fallbackLoopSong = "johto_wild";
            loopSong = Screen.Level.CurrentRegion.Split(',')[0] + "_wild";
        }
        else if (battleType == BattleType.BUG_CATCHING)
        {
            fallbackLoopSong = "johto_wild";
            loopSong = Screen.Level.CurrentRegion.Split(',')[0] + "_wild";
        }
        else if (battleType == BattleType.ROAMING)
        {
            if (BattleSystem.BattleScreen.RoamingPokemonStorage != null && BattleSystem.BattleScreen.RoamingPokemonStorage.MusicLoop != String.Empty)
            {
                loopSong = BattleSystem.BattleScreen.RoamingPokemonStorage.MusicLoop;
            }
        }
        else if (battleType == BattleType.WILD)
        {
            fallbackLoopSong = "johto_wild";
            loopSong = Screen.Level.CurrentRegion.Split(',')[0] + "_wild";
        }
        else
        {
            Console.WriteLine("Unknown Battle Type: " + battleType);
        }

        if (ShouldPlayNightTheme(loopSong))
        {
            loopSong = loopSong + "_night";
            fallbackLoopSong = "johto_wild_night";
        }

        if (MusicManager.SongExists(loopSong) == true)
        {
            return loopSong;
        }
        return fallbackLoopSong;
    }

    private bool SongOver()
    {
        return _startTime + _duration < DateTime.Now;
    }

    private bool ShouldPlayNightTheme(String dayThemeName)
    {
        return World.IsNight() && MusicManager.SongExists(dayThemeName + "_night", false);
    }
}
