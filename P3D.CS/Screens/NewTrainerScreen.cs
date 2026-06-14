using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class NewTrainerScreen : Screen
{
    private Texture2D? _backTexture;
    private Texture2D? _paperClipTexture;
    private Texture2D? _charTexture;
    private Texture2D? _papersTexture;

    private bool _isIntro = true;
    private float _introY = -100.0f;
    private float _rotation = 0f;
    private int _badgeRegionIndex = 0;
    private int _badgeIndex = 0;

    private SpriteBatch? _spriteBatch;
    private SpriteBatch? _textBatch;
    private SpriteBatch? _charBatch;
    private SpriteBatch? _cardBatch;

    private BadgeAnimation _badgeAnimation = new BadgeAnimation();

    private RenderTarget2D? _target;
    private RenderTarget2D? _target2;

    private class BadgeAnimation
    {
        public float _shakeV;
        public bool _shakeLeft;
        public int _shakeCount;
    }

    public NewTrainerScreen(Screen currentScreen)
    {
        Identification = Identifications.TrainerScreen;
        PreScreen = currentScreen;

        _backTexture = TextureManager.GetTexture(@"Textures\UI\TrainerCard\Back");
        _paperClipTexture = TextureManager.GetTexture(@"Textures\UI\TrainerCard\Paperclip");
        _papersTexture = TextureManager.GetTexture(@"Textures\UI\TrainerCard\Papers");

        if (_backTexture != null && _paperClipTexture != null)
        {
            _target = new RenderTarget2D(Core.GraphicsDevice, _backTexture.Width, _backTexture.Height + _paperClipTexture.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }
        _target2 = new RenderTarget2D(Core.GraphicsDevice, Math.Max(1, Core.windowSize.Width), Math.Max(1, Core.windowSize.Height), false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

        if (Level.Surfing == true)
            _charTexture = TextureManager.GetTexture(@"Textures\NPC\" + Core.Player.TempSurfSkin);
        else if (Level.Riding == true)
            _charTexture = TextureManager.GetTexture(@"Textures\NPC\" + Core.Player.TempRideSkin);
        else
            _charTexture = Level.OwnPlayer?.Texture;

        if (_charTexture != null)
        {
            Size frameSize;
            if (_charTexture.Width == _charTexture.Height / 2)
                frameSize = new Size(_charTexture.Width / 2, _charTexture.Height / 4);
            else if (_charTexture.Width == _charTexture.Height)
                frameSize = new Size(_charTexture.Width / 4, _charTexture.Height / 4);
            else
                frameSize = new Size(_charTexture.Width / 3, _charTexture.Height / 4);
            _charTexture = TextureManager.GetTexture(_charTexture, new Rectangle(0, frameSize.Height * 2, frameSize.Width, frameSize.Height));
        }

        _spriteBatch = new SpriteBatch(Core.GraphicsDevice);
        _textBatch = new SpriteBatch(Core.GraphicsDevice);
        _charBatch = new SpriteBatch(Core.GraphicsDevice);
        _cardBatch = new SpriteBatch(Core.GraphicsDevice);

        bool hasKanto = true;
        for (int i = 1; i <= 8; i++)
            if (Core.Player.Badges.Contains(i) == false) { hasKanto = false; break; }
        bool hasJohto = true;
        for (int i = 9; i <= 16; i++)
            if (Core.Player.Badges.Contains(i) == false) { hasJohto = false; break; }

        if (hasKanto == true) GameJolt.Emblem.AchieveEmblem("kanto");
        if (hasJohto == true) GameJolt.Emblem.AchieveEmblem("johto");
    }

    public override void Draw()
    {
        PreScreen?.Draw();

        if (_target == null || _backTexture == null || _paperClipTexture == null || _spriteBatch == null || _textBatch == null || _charBatch == null || _cardBatch == null || _target2 == null)
            return;

        Core.GraphicsDevice.SetRenderTarget(_target);
        Core.GraphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        _textBatch.Begin();
        _charBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

        _spriteBatch.Draw(_backTexture, new Rectangle(0, _paperClipTexture.Height, _backTexture.Width, _backTexture.Height), Color.White);
        if (_papersTexture != null) _textBatch.Draw(_papersTexture, new Vector2(48, 8 + _paperClipTexture.Height), Color.White);
        _charBatch.Draw(_paperClipTexture, new Rectangle(85, 50, 47, 88), Color.White);
        if (_charTexture != null) _charBatch.Draw(_charTexture, new Rectangle(70, 36 + _paperClipTexture.Height, 128, 128), Color.White);

        if (Core.Player.IsGameJoltSave == true)
        {
            _spriteBatch.Draw(GameJolt.Emblem.GetEmblemBackgroundTexture(Core.GameJoltSave.Emblem), new Rectangle(-10, 290, 256, 64), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            Canvas.DrawRectangle(_textBatch, new Rectangle(-10, 300, 190, 30), new Color(0, 0, 0, 150));

            String emblemName = Core.GameJoltSave.Emblem;
            if (emblemName.Length > 0)
                _textBatch.DrawString(FontManager.MainFont, emblemName[0].ToString().ToUpper() + emblemName.Substring(1, emblemName.Length - 1), new Vector2(15, 305), Color.White);

            DrawLevelProgress();
        }

        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("trainer_screen_title", "Trainer Card"), new Vector2(272, 112), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        Vector2 propOffset = new Vector2(272, 152);
        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("global_name", "Name") + ":", new Vector2(propOffset.X, propOffset.Y), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("global_money", "Money") + ":", new Vector2(propOffset.X, propOffset.Y + 32), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("global_IDNo.", "ID No.") + ":", new Vector2(propOffset.X, propOffset.Y + 64), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("global_time", "Time") + ":", new Vector2(propOffset.X, propOffset.Y + 96), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("global_points", "Points") + ":", new Vector2(propOffset.X, propOffset.Y + 128), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        _textBatch.DrawString(FontManager.MainFont, Core.Player.Name, new Vector2(propOffset.X + 112, propOffset.Y), new Color(80, 80, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, "$" + Core.Player.Money, new Vector2(propOffset.X + 112, propOffset.Y + 32), new Color(80, 80, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, Core.Player.OT, new Vector2(propOffset.X + 112, propOffset.Y + 64), new Color(80, 80, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _textBatch.DrawString(FontManager.MainFont, TimeHelpers.GetDisplayTime(TimeHelpers.GetCurrentPlayTime(), true), new Vector2(propOffset.X + 112, propOffset.Y + 96), new Color(80, 80, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        int points = Core.Player.IsGameJoltSave == true ? Core.GameJoltSave.Points : Core.Player.Points;
        _textBatch.DrawString(FontManager.MainFont, points.ToString(), new Vector2(propOffset.X + 112, propOffset.Y + 128), new Color(80, 80, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        DrawBadges();

        _spriteBatch.End();
        _textBatch.End();
        _charBatch.End();

        Core.GraphicsDevice.SetRenderTarget(_target2);
        Core.GraphicsDevice.Clear(Color.Transparent);

        _cardBatch.Begin();
        _cardBatch.Draw(_target,
            new Rectangle(
                (int)(Core.windowSize.Width / 2 - _target.Width / 2 * Core.SpriteBatch.InterfaceScale()) + (int)(42 * Core.SpriteBatch.InterfaceScale()),
                (int)((60 + _introY) * Core.SpriteBatch.InterfaceScale()),
                (int)(_target.Width * Core.SpriteBatch.InterfaceScale()),
                (int)(_target.Height * Core.SpriteBatch.InterfaceScale())),
            null, Color.White, _rotation, Vector2.Zero, SpriteEffects.None, 0f);
        _cardBatch.End();

        Core.GraphicsDevice.SetRenderTarget(null);
        Core.SpriteBatch.Draw(_target2, new Vector2(0, 0), Color.White);
    }

    private void DrawBadges()
    {
        if (_textBatch == null || _spriteBatch == null) return;

        _textBatch.DrawString(FontManager.MainFont, Localization.GetString("trainer_screen_collected_badges") + ": " + Core.Player.Badges.Count, new Vector2(56, 356), Color.Black);

        String selectedRegion = Badge.GetRegion(_badgeRegionIndex);
        int badgesCount = Badge.GetBadgesCount(selectedRegion);
        String badgeName = String.Empty;

        for (int i = 0; i <= badgesCount - 1; i++)
        {
            int badgeID = Badge.GetBadgeID(selectedRegion, i);
            Color c = Color.White;
            String t = Badge.GetBadgeName(badgeID) + Localization.GetString("trainer_screen_badge");
            float shake = 0f;
            if (Badge.PlayerHasBadge(badgeID) == false) { c = Color.Black; t = Localization.GetString("trainer_screen_empty_badge"); }
            if (i == _badgeIndex) { badgeName = t; shake = _badgeAnimation._shakeV; }

            Texture2D badgeTex = Badge.GetBadgeTexture(badgeID);
            _spriteBatch.Draw(badgeTex, new Rectangle(16 + (i + 1) * 64, 412, 50, 50), null, c, shake,
                new Vector2(badgeTex.Width / 2, badgeTex.Height / 2), SpriteEffects.None, 0f);
        }

        _textBatch.DrawString(FontManager.MainFont, badgeName, new Vector2(555 - (int)FontManager.MainFont.MeasureString(badgeName).X, 356), Color.Black);
    }

    private void DrawLevelProgress()
    {
        if (_spriteBatch == null || _textBatch == null) return;

        int currentLevel = GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points);
        int hasPoints = Core.GameJoltSave.Points;
        int needPointsCurrentLevel = GameJolt.Emblem.GetPointsForLevel(currentLevel);
        int needPointsNextLevel = GameJolt.Emblem.GetPointsForLevel(currentLevel + 1);
        int totalNeedPoints = needPointsNextLevel - needPointsCurrentLevel;
        int hasPointsThisLevel = hasPoints - needPointsCurrentLevel;
        int needPoints = totalNeedPoints - hasPointsThisLevel;

        Texture2D? nextSprite = GameJolt.Emblem.GetPlayerSprite(currentLevel + 1, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender);
        if (nextSprite != null)
        {
            Size frameSize = new Size(nextSprite.Width / 3, nextSprite.Height / 4);
            _spriteBatch.Draw(nextSprite, new Rectangle(570, 310, frameSize.Width, frameSize.Height), new Rectangle(0, frameSize.Height * 2, frameSize.Width, frameSize.Height), Color.White);
        }

        float value = totalNeedPoints > 0 ? Math.Clamp((float)hasPointsThisLevel / totalNeedPoints * 310, 0, 310) : 0;
        if (currentLevel >= 100) { value = 310; needPoints = 0; }

        Canvas.DrawRectangle(_spriteBatch, new Rectangle(260, 312, 310, 32), new Color(0, 0, 0, 180));
        Canvas.DrawRectangle(_spriteBatch, new Rectangle(260, 316, (int)value, 24), new Color(255, 165, 0));
        Canvas.DrawRectangle(_spriteBatch, new Rectangle(260, 316, (int)value, 8), new Color(255, 203, 108));

        int nxtLvl = currentLevel >= 100 ? 100 : currentLevel + 1;
        String rankStr = "Rank: " + nxtLvl;
        _textBatch.DrawString(FontManager.MainFont, rankStr, new Vector2(600 - (int)FontManager.MainFont.MeasureString(rankStr).X, 290), Color.Black);
        _textBatch.DrawString(FontManager.MainFont, "Need " + needPoints + (needPoints == 1 ? " point" : " points"), new Vector2(280, 312), Color.Black);
    }

    public override void Update()
    {
        if (_isIntro == true)
        {
            if (_rotation < 0.12f)
            {
                _rotation = MathHelper.Lerp(_rotation, 0.12f, 0.1f);
                _introY = MathHelper.Lerp(_introY, 0, 0.1f);
                if (_rotation + 0.01f >= 0.12f) _isIntro = false;
            }
        }
        else
        {
            if (Controls.Dismiss() == true)
            {
                SoundManager.PlaySound("select");
                Core.SetScreen(PreScreen!);
            }
        }

        if (Controls.Right(true, false, false, true, true, false) == true) _badgeIndex += 1;
        if (Controls.Left(true, false, false, true, true, false) == true) _badgeIndex -= 1;
        if (Controls.Up(true, false, false, true, true, false) == true) _badgeRegionIndex -= 1;
        if (Controls.Down(true, false, false, true, true, false) == true) _badgeRegionIndex += 1;

        _badgeRegionIndex = _badgeRegionIndex.Clamp(0, Badge.GetRegionCount() - 1);
        _badgeIndex = _badgeIndex.Clamp(0, Badge.GetBadgesCount(Badge.GetRegion(_badgeRegionIndex)) - 1);

        if (Core.Player.IsGameJoltSave == true)
        {
            String emblemName = Core.GameJoltSave.Emblem;
            int newIndex = Core.GameJoltSave.AchievedEmblems.IndexOf(emblemName);
            if (Controls.Up(true, true, false, false, false, true) == true || Controls.Left(true, true, false, false, false, true) == true || ControllerHandler.ButtonPressed(Buttons.LeftShoulder) == true)
                newIndex -= 1;
            if (Controls.Down(true, true, false, false, false, true) == true || Controls.Right(true, true, false, false, false, true) == true || ControllerHandler.ButtonPressed(Buttons.RightShoulder) == true)
                newIndex += 1;
            if (newIndex >= 0 && newIndex < Core.GameJoltSave.AchievedEmblems.Count)
                Core.GameJoltSave.Emblem = Core.GameJoltSave.AchievedEmblems[newIndex];
        }

        if (_badgeAnimation._shakeLeft == true)
        {
            _badgeAnimation._shakeV -= 0.035f;
            if (_badgeAnimation._shakeV <= -0.4f) { _badgeAnimation._shakeCount -= 1; _badgeAnimation._shakeLeft = false; }
        }
        else
        {
            _badgeAnimation._shakeV += 0.035f;
            if (_badgeAnimation._shakeV >= 0.4f) { _badgeAnimation._shakeCount -= 1; _badgeAnimation._shakeLeft = true; }
        }
    }
}
