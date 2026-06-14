using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D;
using P3D.Resources.Blur;

namespace VoltorbFlip;

public class VoltorbFlipScreen : Screen
{
    // Variables & Properties

    public float _screenTransitionY = 0F;
    public static float _interfaceFade = 0F;

    private int _delay = 0;
    private float _memoMenuX = 0F;
    private Size _memoMenuSize = new Size(112, 112);

    public static readonly Size GameSize = new Size(576, 544);
    public static readonly Size BoardSize = new Size(384, 384);
    public static readonly Size TileSize = new Size(64, 64);
    private static readonly int GridSize = 5;

    public static Vector2 GameOrigin = new Vector2(
        (int)(Core.windowSize.Width / 2 - 576 / 2 - 32),
        (int)(Core.windowSize.Height / 2 - 544 / 2));
    public static Vector2 BoardOrigin = new Vector2(GameOrigin.X + 32, GameOrigin.Y + 160);
    public static Rectangle TutorialRectangle = new Rectangle(
        (int)(Core.windowSize.Width / 2 - 512 / 2),
        (int)(Core.windowSize.Height / 2 - 384 / 2), 512, 384);

    private Vector2 _boardCursorPosition = new Vector2(0, 0);
    private Vector2 _boardCursorDestination = new Vector2(0, 0);

    private int _newLevelMenuIndex = 0;
    private int _memoIndex = 0;

    public static States GameState = States.Opening;

    public static Texture2D? Texture_HUD;
    public static Texture2D? Texture_Cursor_Game;
    public static Texture2D? Texture_Cursor_Memo;
    public static Texture2D? Texture_Tile_Front;
    public static Texture2D? Texture_Tile_Back;
    public static Texture2D? Texture_Board;
    public static Texture2D? Texture_Background;
    public static Texture2D? Texture_Button_Quit;

    public static int PreviousLevel { get; set; } = 1;
    public static int CurrentLevel { get; set; } = 1;
    public static readonly int MinLevel = 1;
    public static readonly int MaxLevel = 7;
    public static int CurrentFlips { get; set; } = 0;
    public static int TotalFlips { get; set; } = 0;
    public static int CurrentCoins { get; set; } = 0;
    public static int TotalCoins { get; set; } = -1;
    public static int MaxCoins = 1;
    public static int ConsecutiveWins { get; set; } = 0;

    public List<List<Tile>> Board = null!;
    public List<List<int>> VoltorbSums = null!;
    public List<List<int>> CoinSums = null!;

    public enum States
    {
        Opening,
        Closing,
        QuitQuestion,
        Game,
        Memo,
        GameWon,
        GameLost,
        FlipWon,
        FlipLost,
        NewLevelQuestion,
        NewLevel
    }

    private BlurHandler _blur = null!;
    private RenderTarget2D _preScreenTarget = null!;
    private RenderTarget2D? _preScreenTexture;
    private readonly Identifications[] _blurScreens =
    [
        Identifications.BattleScreen, Identifications.OverworldScreen,
        Identifications.DirectTradeScreen, Identifications.WonderTradeScreen,
        Identifications.GTSSetupScreen, Identifications.GTSTradeScreen,
        Identifications.PVPLobbyScreen
    ];

    public VoltorbFlipScreen(Screen currentScreen)
    {
        Texture_HUD = TextureManager.GetTexture("Textures\\VoltorbFlip\\HUD");
        Texture_Cursor_Game = TextureManager.GetTexture("Textures\\VoltorbFlip\\Cursor_Game");
        Texture_Cursor_Memo = TextureManager.GetTexture("Textures\\VoltorbFlip\\Cursor_Memo");
        Texture_Tile_Front = TextureManager.GetTexture("Textures\\VoltorbFlip\\Tile_Front");
        Texture_Tile_Back = TextureManager.GetTexture("Textures\\VoltorbFlip\\Tile_Back");
        Texture_Board = TextureManager.GetTexture("Textures\\VoltorbFlip\\Board");
        Texture_Background = TextureManager.GetTexture("Textures\\VoltorbFlip\\Background");
        Texture_Button_Quit = TextureManager.GetTexture("Textures\\VoltorbFlip\\Quit_Button");

        GameState = States.Opening;
        GameOrigin = new Vector2((int)(Core.windowSize.Width / 2 - GameSize.Width / 2 - 32), (int)(Core.windowSize.Height / 2 - _screenTransitionY));
        BoardOrigin = new Vector2(GameOrigin.X + 32, GameOrigin.Y + 160);
        TutorialRectangle = new Rectangle((int)(Core.windowSize.Width / 2 - 512 / 2), (int)(Core.windowSize.Height / 2 - 384 / 2), 512, 384);

        _boardCursorDestination = GetCursorOffset(0, 0);
        _boardCursorPosition = GetCursorOffset(0, 0);

        Board = CreateBoard(CurrentLevel);
        TotalCoins = 0;

        _preScreenTarget = new RenderTarget2D(Core.GraphicsDevice, Core.windowSize.Width, Core.windowSize.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        _blur = new BlurHandler(Core.windowSize.Width, Core.windowSize.Height);

        Identification = Identifications.VoltorbFlipScreen;
        PreScreen = currentScreen;

        MouseVisible = true;
        CanChat = PreScreen.CanChat;
        CanBePaused = PreScreen.CanBePaused;
        ChooseBox.readyForResult = false;
        TextBox.ResultFunction = null;
    }

    public override void Draw()
    {
        if (PreScreen != null && _blurScreens.Contains(PreScreen.Identification))
            DrawPrescreen();
        else
            PreScreen?.Draw();

        DrawBackground();
        DrawMemoMenuAndButton();

        if (Board != null)
        {
            DrawBoard();
            DrawCursor();
        }

        DrawHUD();
        DrawQuitButton();
        DrawTutorial();

        ChooseBox.Draw();
        TextBox.Draw();
    }

    private void DrawPrescreen()
    {
        if (_preScreenTexture == null || _preScreenTexture.IsContentLost == true)
        {
            Core.SpriteBatch.EndBatch();

            RenderTarget2D target = _preScreenTarget;
            Core.GraphicsDevice.SetRenderTarget(target);
            Core.GraphicsDevice.Clear(Core.BackgroundColor);

            Core.SpriteBatch.BeginBatch();
            PreScreen?.Draw();
            Core.SpriteBatch.EndBatch();

            Core.GraphicsDevice.SetRenderTarget(null);
            Core.SpriteBatch.BeginBatch();

            _preScreenTexture = target;
        }

        if (_interfaceFade < 1.0F)
            Core.SpriteBatch.Draw(_preScreenTexture, Core.windowSize, Color.White);

        Core.SpriteBatch.Draw(_blur.Perform(_preScreenTexture), Core.windowSize, new Color(255, 255, 255, Math.Clamp((int)(255 * _interfaceFade * 2), 0, 255)));
    }

    private void DrawBackground()
    {
        Color c = (GameState == States.Closing || GameState == States.Opening)
            ? new Color(255, 255, 255, (int)(255 * _interfaceFade))
            : Color.White;
        Canvas.DrawImageBorder(Texture_Background!, 2, new Rectangle((int)GameOrigin.X, (int)GameOrigin.Y, GameSize.Width, GameSize.Height), c, false);
    }

    private void DrawHUD()
    {
        bool fading = GameState == States.Closing || GameState == States.Opening;
        Color bg = fading ? new Color(255, 255, 255, (int)(255 * _interfaceFade)) : Color.White;
        Color fontColor = fading ? new Color(0, 0, 0, (int)(255 * _interfaceFade)) : Color.Black;

        String levelText = Localization.GetString("VoltorbFlip_LV.", "LV.") + " " + CurrentLevel.ToString();
        Canvas.DrawImageBorder(Texture_HUD!, 2, new Rectangle((int)(GameOrigin.X + 32), (int)(GameOrigin.Y + 32), 96, 96), bg, false);
        Core.SpriteBatch.DrawString(FontManager.MainFont, levelText, new Vector2((int)(GameOrigin.X + 80 + 4 - FontManager.MainFont.MeasureString(levelText).X / 2), (int)(GameOrigin.Y + 80 + 4 - FontManager.MainFont.MeasureString(levelText).Y / 2)), fontColor);

        Canvas.DrawImageBorder(Texture_HUD!, 2, new Rectangle((int)(GameOrigin.X + 128 + 24), (int)(GameOrigin.Y + 32), 192, 96), bg, false);
        String cc1 = Localization.GetString("VoltorbFlip_CurrentCoins_Line1", "Coins found");
        String cc2 = Localization.GetString("VoltorbFlip_CurrentCoins_Line2", "in this LV.");
        String cc3 = "[";
        if (CurrentCoins < 10000) cc3 += "0";
        if (CurrentCoins < 1000) cc3 += "0";
        if (CurrentCoins < 100) cc3 += "0";
        if (CurrentCoins < 10) cc3 += "0";
        cc3 += CurrentCoins.ToString() + "]";
        Core.SpriteBatch.DrawString(FontManager.MainFont, cc1, new Vector2((int)(GameOrigin.X + 232 + 24 - FontManager.MainFont.MeasureString(cc1).X / 2), (int)(GameOrigin.Y + 80 + 4 - FontManager.MainFont.MeasureString(cc2).Y / 2 - FontManager.MainFont.MeasureString(cc1).Y)), fontColor);
        Core.SpriteBatch.DrawString(FontManager.MainFont, cc2, new Vector2((int)(GameOrigin.X + 232 + 24 - FontManager.MainFont.MeasureString(cc2).X / 2), (int)(GameOrigin.Y + 80 + 4 - FontManager.MainFont.MeasureString(cc2).Y / 2)), fontColor);
        Core.SpriteBatch.DrawString(FontManager.MainFont, cc3, new Vector2((int)(GameOrigin.X + 232 + 24 - FontManager.MainFont.MeasureString(cc3).X / 2), (int)(GameOrigin.Y + 80 + 4 + FontManager.MainFont.MeasureString(cc2).Y / 2)), fontColor);

        Canvas.DrawImageBorder(Texture_HUD!, 2, new Rectangle((int)(GameOrigin.X + 336 + 32), (int)(GameOrigin.Y + 32), 192, 96), bg, false);
        String tc1 = Localization.GetString("VoltorbFlip_TotalCoins_Line1", "<player.name>'s");
        String tc2 = Localization.GetString("VoltorbFlip_TotalCoins_Line2", "earned Coins");
        String tc3 = "[";
        if (TotalCoins + Core.Player.Coins < 10000) tc3 += "0";
        if (TotalCoins < 1000 + Core.Player.Coins) tc3 += "0";
        if (TotalCoins < 100 + Core.Player.Coins) tc3 += "0";
        if (TotalCoins < 10 + Core.Player.Coins) tc3 += "0";
        tc3 += (TotalCoins + Core.Player.Coins).ToString() + "]";
        Core.SpriteBatch.DrawString(FontManager.MainFont, tc1, new Vector2((int)(GameOrigin.X + 440 + 32 - FontManager.MainFont.MeasureString(tc1).X / 2), (int)(GameOrigin.Y + 80 + 4 - FontManager.MainFont.MeasureString(tc2).Y / 2 - FontManager.MainFont.MeasureString(tc1).Y)), fontColor);
        Core.SpriteBatch.DrawString(FontManager.MainFont, tc2, new Vector2((int)(GameOrigin.X + 440 + 32 - FontManager.MainFont.MeasureString(tc2).X / 2), (int)(GameOrigin.Y + 80 + 4 - FontManager.MainFont.MeasureString(tc2).Y / 2)), fontColor);
        Core.SpriteBatch.DrawString(FontManager.MainFont, tc3, new Vector2((int)(GameOrigin.X + 440 + 32 - FontManager.MainFont.MeasureString(tc3).X / 2), (int)(GameOrigin.Y + 80 + 4 + FontManager.MainFont.MeasureString(tc2).Y / 2)), fontColor);
    }

    private void DrawTutorial()
    {
        bool fading = GameState == States.Closing || GameState == States.Opening;
        Color mainColor = fading ? new Color(255, 255, 255, (int)(255 * _interfaceFade)) : Color.White;
        Color fontColor = fading ? new Color(0, 0, 0, (int)(255 * _interfaceFade)) : Color.Black;

        if (GameState != States.NewLevelQuestion) return;

        switch (_newLevelMenuIndex)
        {
            case 2: // How to Play
            {
                Canvas.DrawRectangle(new Rectangle((int)GameOrigin.X, (int)GameOrigin.Y, GameSize.Width + 64, GameSize.Height + 32), new Color(0, 0, 0, 128));
                String ts1 = Localization.GetString("VoltorbFlip_Tutorial_HowToPlay_Image1", "If you flip the cards in this order, you'll collect: 3 x 1 x 2 x 1 x 3... A total of 18 Coins! And then...").Replace("~", Environment.NewLine);
                String ts2 = Localization.GetString("VoltorbFlip_Tutorial_HowToPlay_Image2", "If you select \"Quit\", you'll keep those 18 Coins.").Replace("~", Environment.NewLine);
                String ts3 = Localization.GetString("VoltorbFlip_Tutorial_HowToPlay_Image3", "But if you find Voltorb, you'll lose all your Coins!").Replace("~", Environment.NewLine);
                Core.SpriteBatch.Draw(TextureManager.GetTexture("Textures\\VoltorbFlip\\Tutorial_HowToPlay"), new Rectangle((int)TutorialRectangle.X, (int)TutorialRectangle.Y, TutorialRectangle.Width, TutorialRectangle.Height), mainColor);
                String ts1c = ts1.CropStringToWidth(FontManager.MainFont, 1, 448);
                String ts2c = ts2.CropStringToWidth(FontManager.MainFont, 1, 304);
                String ts3c = ts3.CropStringToWidth(FontManager.MainFont, 1, 304);
                Core.SpriteBatch.DrawString(FontManager.MainFont, ts1c, new Vector2((int)(TutorialRectangle.X + 256 - FontManager.MainFont.MeasureString(ts1c).X / 2), (int)(TutorialRectangle.Y + 127 - FontManager.MainFont.MeasureString(ts1c).Y / 2)), fontColor);
                Core.SpriteBatch.DrawString(FontManager.MainFont, ts2c, new Vector2((int)(TutorialRectangle.X + 328 - FontManager.MainFont.MeasureString(ts2c).X / 2), (int)(TutorialRectangle.Y + 255 - FontManager.MainFont.MeasureString(ts2c).Y / 2)), fontColor);
                Core.SpriteBatch.DrawString(FontManager.MainFont, ts3c, new Vector2((int)(TutorialRectangle.X + 328 - FontManager.MainFont.MeasureString(ts3c).X / 2), (int)(TutorialRectangle.Y + 335 - FontManager.MainFont.MeasureString(ts3c).Y / 2)), fontColor);
                String qbt = Localization.GetString("VoltorbFlip_QuitButton", "Quit");
                Core.SpriteBatch.DrawString(FontManager.MainFont, qbt, new Vector2((int)(TutorialRectangle.X + 10 + 128 / 2 - FontManager.MainFont.MeasureString(qbt).X / 2), (int)(TutorialRectangle.Y + 228 + 56 / 2 - FontManager.MainFont.MeasureString(qbt).Y / 2)), fontColor);
                Core.SpriteBatch.DrawString(FontManager.MainFont, qbt, new Vector2((int)(TutorialRectangle.X + 10 + 128 / 2 - FontManager.MainFont.MeasureString(qbt).X / 2 - 2), (int)(TutorialRectangle.Y + 228 + 56 / 2 - FontManager.MainFont.MeasureString(qbt).Y / 2 - 2)), mainColor);
                break;
            }
            case 3: // Hint
            {
                Canvas.DrawRectangle(new Rectangle((int)GameOrigin.X, (int)GameOrigin.Y, GameSize.Width + 64, GameSize.Height + 32), new Color(0, 0, 0, 128));
                String ts = Localization.GetString("VoltorbFlip_Tutorial_Hint_Image", "By looking at the numbers on the sides of the cards, you can see the hidden number and Voltorb totals.").Replace("~", Environment.NewLine);
                Core.SpriteBatch.Draw(TextureManager.GetTexture("Textures\\VoltorbFlip\\Tutorial_Hint"), new Rectangle((int)TutorialRectangle.X, (int)TutorialRectangle.Y, TutorialRectangle.Width, TutorialRectangle.Height), mainColor);
                String tsc = ts.CropStringToWidth(FontManager.MainFont, 1, 448);
                Core.SpriteBatch.DrawString(FontManager.MainFont, tsc, new Vector2((int)(TutorialRectangle.X + 256 - FontManager.MainFont.MeasureString(tsc).X / 2), (int)(TutorialRectangle.Y + 319 - FontManager.MainFont.MeasureString(tsc).Y / 2)), fontColor);
                break;
            }
            case 4: // About Memos
            {
                Canvas.DrawRectangle(new Rectangle((int)GameOrigin.X, (int)GameOrigin.Y, GameSize.Width + 64, GameSize.Height + 32), new Color(0, 0, 0, 128));
                String ts = Localization.GetString("VoltorbFlip_Tutorial_AboutMemos_Image", "Select \"Open Memo\" to open the Memo Window. Select the cards and press [<system.button(enter1)>] to add and [<system.button(back1)>] to remove marks.").Replace("~", Environment.NewLine);
                String btnTop = Localization.GetString("VoltorbFlip_MemoButton_Open_Line1", "Open");
                String btnBot = Localization.GetString("VoltorbFlip_MemoButton_Open_Line2", "Memos");
                Core.SpriteBatch.Draw(TextureManager.GetTexture("Textures\\VoltorbFlip\\Tutorial_AboutMemos"), new Rectangle((int)TutorialRectangle.X, (int)TutorialRectangle.Y, TutorialRectangle.Width, TutorialRectangle.Height), mainColor);
                String tsc = ts.CropStringToWidth(FontManager.MainFont, 1, 448);
                Core.SpriteBatch.DrawString(FontManager.MainFont, tsc, new Vector2((int)(TutorialRectangle.X + 256 - FontManager.MainFont.MeasureString(tsc).X / 2), (int)(TutorialRectangle.Y + 303 - FontManager.MainFont.MeasureString(tsc).Y / 2)), fontColor);
                Core.SpriteBatch.DrawString(FontManager.MainFont, btnTop, new Vector2((int)(TutorialRectangle.X + 64 + _memoMenuSize.Width / 2 - FontManager.MainFont.MeasureString(btnTop).X / 2), (int)(TutorialRectangle.Y + 104)), fontColor);
                Core.SpriteBatch.DrawString(FontManager.MainFont, btnBot, new Vector2((int)(TutorialRectangle.X + 64 + _memoMenuSize.Width / 2 - FontManager.MainFont.MeasureString(btnBot).X / 2), (int)(TutorialRectangle.Y + 104 + FontManager.MainFont.MeasureString(btnTop).Y)), fontColor);
                break;
            }
        }
    }

    private void DrawBoard()
    {
        Color bg = (GameState == States.Closing || GameState == States.Opening)
            ? new Color(255, 255, 255, (int)(255 * _interfaceFade))
            : Color.White;
        Core.SpriteBatch.Draw(Texture_Board!, new Rectangle((int)BoardOrigin.X, (int)BoardOrigin.Y, BoardSize.Width, BoardSize.Height), bg);
        DrawTiles();
        DrawSums();
    }

    private void DrawTiles()
    {
        for (int row = 0; row <= GridSize - 1; row++)
            for (int col = 0; col <= GridSize - 1; col++)
                Board[row][col].Draw();
    }

    private void DrawSums()
    {
        bool showSums = GameState == States.Game || GameState == States.Memo || GameState == States.QuitQuestion;
        Color bg = (GameState == States.Closing || GameState == States.Opening)
            ? new Color(255, 255, 255, (int)(255 * _interfaceFade))
            : Color.White;

        for (int rowIndex = 0; rowIndex <= GridSize - 1; rowIndex++)
        {
            String coinStr = "00";
            if (showSums == true)
            {
                int sum = CoinSums[0][rowIndex];
                coinStr = sum < 10 ? "0" + sum.ToString() : sum.ToString();
            }
            Core.SpriteBatch.DrawString(FontManager.VoltorbFlipFont, coinStr, new Vector2((int)(BoardOrigin.X + TileSize.Width * (GridSize + 1) - 8 - FontManager.VoltorbFlipFont.MeasureString(coinStr).X), BoardOrigin.Y + TileSize.Height * rowIndex + 8), bg);
        }
        for (int rowIndex = 0; rowIndex <= GridSize - 1; rowIndex++)
        {
            String vStr = showSums == true ? VoltorbSums[0][rowIndex].ToString() : "0";
            Core.SpriteBatch.DrawString(FontManager.VoltorbFlipFont, vStr, new Vector2((int)(BoardOrigin.X + TileSize.Width * (GridSize + 1) - 8 - FontManager.VoltorbFlipFont.MeasureString(vStr).X), BoardOrigin.Y + TileSize.Height * rowIndex + 34), bg);
        }
        for (int colIndex = 0; colIndex <= GridSize - 1; colIndex++)
        {
            String coinStr = "00";
            if (showSums == true)
            {
                int sum = CoinSums[1][colIndex];
                coinStr = sum < 10 ? "0" + sum.ToString() : sum.ToString();
            }
            Core.SpriteBatch.DrawString(FontManager.VoltorbFlipFont, coinStr, new Vector2((int)(BoardOrigin.X + TileSize.Width * colIndex + TileSize.Width - 8 - FontManager.VoltorbFlipFont.MeasureString(coinStr).X), BoardOrigin.Y + TileSize.Height * GridSize + 8), bg);
        }
        for (int colIndex = 0; colIndex <= GridSize - 1; colIndex++)
        {
            String vStr = showSums == true ? VoltorbSums[1][colIndex].ToString() : "0";
            Core.SpriteBatch.DrawString(FontManager.VoltorbFlipFont, vStr, new Vector2((int)(BoardOrigin.X + TileSize.Width * colIndex + TileSize.Width - 8 - FontManager.VoltorbFlipFont.MeasureString(vStr).X), BoardOrigin.Y + TileSize.Height * GridSize + 34), bg);
        }
    }

    private void DrawMemoMenuAndButton()
    {
        bool fading = GameState == States.Closing || GameState == States.Opening;
        Color bg = fading ? new Color(255, 255, 255, (int)(255 * _interfaceFade)) : Color.White;
        Color fontColor = fading ? new Color(0, 0, 0, (int)(255 * _interfaceFade)) : Color.Black;

        int buttonOriginX = (int)(BoardOrigin.X + BoardSize.Width + TileSize.Width / 4);
        Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Button", new Rectangle(0, 0, 56, 56), String.Empty), new Rectangle(buttonOriginX, (int)BoardOrigin.Y, _memoMenuSize.Width, _memoMenuSize.Height), bg);

        String btnTop = GameState == States.Memo
            ? Localization.GetString("VoltorbFlip_MemoButton_Close_Line1", "Close")
            : Localization.GetString("VoltorbFlip_MemoButton_Open_Line1", "Open");
        String btnBot = GameState == States.Memo
            ? Localization.GetString("VoltorbFlip_MemoButton_Close_Line2", "Memos")
            : Localization.GetString("VoltorbFlip_MemoButton_Open_Line2", "Memos");

        Core.SpriteBatch.DrawString(FontManager.MainFont, btnTop, new Vector2((int)(buttonOriginX + _memoMenuSize.Width / 2 - FontManager.MainFont.MeasureString(btnTop).X / 2), (int)(BoardOrigin.Y + 40)), fontColor);
        Core.SpriteBatch.DrawString(FontManager.MainFont, btnBot, new Vector2((int)(buttonOriginX + _memoMenuSize.Width / 2 - FontManager.MainFont.MeasureString(btnBot).X / 2), (int)(BoardOrigin.Y + 40 + FontManager.MainFont.MeasureString(btnTop).Y)), fontColor);

        if (_memoMenuX > 0)
        {
            Vector2 ct = GetCurrentTile();
            Tile currentTile = Board[(int)ct.Y][(int)ct.X];

            Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Background", new Rectangle(0, 0, 56, 56), String.Empty), new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height), bg);

            if (GameState == States.Memo)
            {
                if (currentTile.GetMemo(0) == true)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Enabled", new Rectangle(0, 0, 56, 56), String.Empty), new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height), bg);
                if (currentTile.GetMemo(1) == true)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Enabled", new Rectangle(56, 0, 56, 56), String.Empty), new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height), bg);
                if (currentTile.GetMemo(2) == true)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Enabled", new Rectangle(112, 0, 56, 56), String.Empty), new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height), bg);
                if (currentTile.GetMemo(3) == true)
                    Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Enabled", new Rectangle(168, 0, 56, 56), String.Empty), new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height), bg);

                Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Memo_Index", new Rectangle(56 * _memoIndex, 0, 56, 56), String.Empty), new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height), bg);
            }
        }
    }

    private void DrawCursor()
    {
        if (GameState != States.Game && GameState != States.Memo) return;

        Color bg = (GameState == States.Closing || GameState == States.Opening)
            ? new Color(255, 255, 255, (int)(255 * _interfaceFade))
            : Color.White;

        Texture2D cursorImg = GameState == States.Memo ? Texture_Cursor_Memo! : Texture_Cursor_Game!;
        Core.SpriteBatch.Draw(cursorImg, new Rectangle((int)(BoardOrigin.X + _boardCursorPosition.X), (int)(BoardOrigin.Y + _boardCursorPosition.Y), TileSize.Width, TileSize.Height), bg);
    }

    private void DrawQuitButton()
    {
        bool fading = GameState == States.Closing || GameState == States.Opening;
        Color mainColor = fading ? new Color(255, 255, 255, (int)(255 * _interfaceFade)) : Color.White;
        Color shadowColor = fading ? new Color(0, 0, 0, (int)(255 * _interfaceFade)) : Color.Black;

        Rectangle qbRect = new Rectangle((int)(GameOrigin.X + 424), (int)(GameOrigin.Y + 448), 128, 56);
        Core.SpriteBatch.Draw(Texture_Button_Quit!, qbRect, mainColor);

        String qbt = Localization.GetString("VoltorbFlip_QuitButton", "Quit");
        Core.SpriteBatch.DrawString(FontManager.MainFont, qbt, new Vector2((int)(qbRect.X + qbRect.Width / 2 - FontManager.MainFont.MeasureString(qbt).X / 2), (int)(qbRect.Y + qbRect.Height / 2 - FontManager.MainFont.MeasureString(qbt).Y / 2)), shadowColor);
        Core.SpriteBatch.DrawString(FontManager.MainFont, qbt, new Vector2((int)(qbRect.X + qbRect.Width / 2 - FontManager.MainFont.MeasureString(qbt).X / 2 - 2), (int)(qbRect.Y + qbRect.Height / 2 - FontManager.MainFont.MeasureString(qbt).Y / 2 - 2)), mainColor);
    }

    private List<List<Tile>> CreateBoard(int level)
    {
        List<List<Tile>> board = CreateGrid();
        List<int> data = GetLevelData(level) ?? [3, 1, 6];
        List<List<int>> spots = [];

        int totalSpots = data[0] + data[1] + data[2];
        for (int i = 0; i < totalSpots; i++)
        {
            if (spots.Count > 0)
            {
                int vx, vy;
                bool isUnique;
                do
                {
                    vx = Core.Random.Next(0, 5);
                    vy = Core.Random.Next(0, 5);
                    isUnique = true;
                    for (int si = 0; si < spots.Count; si++)
                    {
                        if (spots[si][0] == vx && spots[si][1] == vy)
                        {
                            isUnique = false;
                            break;
                        }
                    }
                } while (isUnique == false);
                spots.Add([vx, vy]);
            }
            else
            {
                spots.Add([Core.Random.Next(0, 5), Core.Random.Next(0, 5)]);
            }
        }

        for (int a = 0; a < data[0]; a++)
            board[spots[a][1]][spots[a][0]].Value = (int)Tile.Values.Two;

        for (int b = 0; b < data[1]; b++)
            board[spots[b + data[0]][1]][spots[b + data[0]][0]].Value = (int)Tile.Values.Three;

        for (int c = 0; c < data[2]; c++)
            board[spots[c + data[0] + data[1]][1]][spots[c + data[0] + data[1]][0]].Value = (int)Tile.Values.Voltorb;

        if (data[0] > 0 && data[1] > 0) MaxCoins = (int)(Math.Pow(2, data[0]) * Math.Pow(3, data[1]));
        if (data[0] > 0 && data[1] == 0) MaxCoins = (int)Math.Pow(2, data[0]);
        if (data[0] == 0 && data[1] > 0) MaxCoins = (int)Math.Pow(3, data[1]);

        VoltorbSums = GenerateSums(board, true);
        CoinSums = GenerateSums(board, false);

        return board;
    }

    private List<List<Tile>> CreateGrid()
    {
        List<List<Tile>> grid = [];
        for (int row = 0; row < GridSize; row++)
        {
            List<Tile> column = [];
            for (int col = 0; col < GridSize; col++)
                column.Add(new Tile(row, col, (int)Tile.Values.One, false));
            grid.Add(column);
        }
        return grid;
    }

    private List<List<int>> GenerateSums(List<List<Tile>> board, bool coinsOrVoltorbs)
    {
        List<int> rowSums = [0, 0, 0, 0, 0];
        List<int> colSums = [0, 0, 0, 0, 0];
        List<int> rowBombs = [0, 0, 0, 0, 0];
        List<int> colBombs = [0, 0, 0, 0, 0];

        for (int row = 0; row < GridSize; row++)
            for (int col = 0; col < GridSize; col++)
            {
                if (board[row][col].Value == (int)Tile.Values.Voltorb)
                    rowBombs[row] += 1;
                else
                    rowSums[row] += board[row][col].Value;
            }

        for (int col = 0; col < GridSize; col++)
            for (int row = 0; row < GridSize; row++)
            {
                if (board[row][col].Value == (int)Tile.Values.Voltorb)
                    colBombs[col] += 1;
                else
                    colSums[col] += board[row][col].Value;
            }

        if (coinsOrVoltorbs == false)
            return [rowSums, colSums];
        else
            return [rowBombs, colBombs];
    }

    public Vector2 GetCursorOffset(int column = 0, int row = 0)
    {
        return new Vector2(TileSize.Width * column, TileSize.Height * row);
    }

    public Vector2 GetCurrentTile()
    {
        return new Vector2(
            (float)Math.Clamp(_boardCursorDestination.X / TileSize.Width, 0, GridSize - 1),
            (float)Math.Clamp(_boardCursorDestination.Y / TileSize.Height, 0, GridSize - 1));
    }

    public Vector2 GetTileUnderMouse()
    {
        Vector2 abs = MouseHandler.MousePosition.ToVector2();
        float rx = Math.Clamp(abs.X - BoardOrigin.X, 0, BoardSize.Width);
        float ry = Math.Clamp(abs.Y - BoardOrigin.Y, 0, BoardSize.Height);
        return new Vector2(
            (float)Math.Clamp(Math.Floor(rx / TileSize.Width), 0, GridSize - 1),
            (float)Math.Clamp(Math.Floor(ry / TileSize.Height), 0, GridSize - 1));
    }

    public List<int>? GetLevelData(int levelNumber)
    {
        int chance = Core.Random.Next(0, 5);
        return levelNumber switch
        {
            1 => chance switch { 0 => [3, 1, 6], 1 => [0, 3, 6], 2 => [5, 0, 6], 3 => [2, 2, 6], 4 => [4, 1, 6], _ => null },
            2 => chance switch { 0 => [1, 3, 7], 1 => [6, 0, 7], 2 => [3, 2, 7], 3 => [0, 4, 7], 4 => [5, 1, 7], _ => null },
            3 => chance switch { 0 => [2, 3, 8], 1 => [7, 0, 8], 2 => [4, 2, 8], 3 => [1, 4, 8], 4 => [6, 1, 8], _ => null },
            4 => chance switch { 0 => [3, 3, 8], 1 => [0, 5, 8], 2 => [8, 0, 10], 3 => [5, 2, 10], 4 => [2, 4, 10], _ => null },
            5 => chance switch { 0 => [7, 1, 10], 1 => [4, 3, 10], 2 => [1, 5, 10], 3 => [9, 0, 10], 4 => [6, 2, 10], _ => null },
            6 => chance switch { 0 => [3, 4, 10], 1 => [0, 6, 10], 2 => [8, 1, 10], 3 => [5, 3, 10], 4 => [2, 5, 10], _ => null },
            7 => chance switch { 0 => [7, 2, 10], 1 => [4, 4, 10], 2 => [1, 6, 13], 3 => [9, 1, 13], 4 => [6, 3, 10], _ => null },
            8 => chance switch { 0 => [0, 7, 10], 1 => [8, 2, 10], 2 => [5, 4, 10], 3 => [2, 6, 10], 4 => [7, 3, 10], _ => null },
            _ => null
        };
    }

    protected override SpriteBatch GetFontRenderer()
    {
        if (IsCurrentScreen() == true && _interfaceFade + 0.01F >= 1.0F)
            return Core.FontRenderer;
        else
            return Core.SpriteBatch;
    }

    public override void SizeChanged()
    {
        GameOrigin = new Vector2((int)(Core.windowSize.Width / 2 - GameSize.Width / 2 - 32), (int)(Core.windowSize.Height / 2 - _screenTransitionY));
        BoardOrigin = new Vector2(GameOrigin.X + 32, GameOrigin.Y + 160);
        TutorialRectangle = new Rectangle((int)(Core.windowSize.Width / 2 - 512 / 2), (int)(Core.windowSize.Height / 2 - 384 / 2), 512, 384);
        _boardCursorDestination = GetCursorOffset(0, 0);
        _boardCursorPosition = GetCursorOffset(0, 0);
    }

    public void Minimized()
    {
        GameOrigin = new Vector2((int)(Core.windowSize.Width / 2 - GameSize.Width / 2 - 32), (int)(Core.windowSize.Height / 2 - _screenTransitionY));
        BoardOrigin = new Vector2(GameOrigin.X + 32, GameOrigin.Y + 160);
        TutorialRectangle = new Rectangle((int)(Core.windowSize.Width / 2 - 512 / 2), (int)(Core.windowSize.Height / 2 - 384 / 2), 512, 384);
    }

    public void UpdateTiles()
    {
        for (int row = 0; row <= GridSize - 1; row++)
            for (int col = 0; col <= GridSize - 1; col++)
                Board[row][col].Update();
    }

    public override void Update()
    {
        if ((int)GameOrigin.X != (int)(Core.windowSize.Width / 2 - GameSize.Width / 2 - 32) || (int)GameOrigin.Y != (int)(Core.windowSize.Height / 2 - _screenTransitionY))
            Minimized();

        ChooseBox.Update();
        if (ChooseBox.Showing == false)
            TextBox.Update();

        if (ChooseBox.Showing == false && TextBox.Showing == false)
        {
            if (_delay > 0)
            {
                _delay -= 1;
                if (_delay < 0) _delay = 0;
            }
        }

        if (Board != null)
            UpdateTiles();

        if (_delay == 0)
        {
            if (ChooseBox.Showing == false && TextBox.Showing == false)
            {
                if (GameState == States.Game || GameState == States.Memo)
                {
                    if (Controls.Up(true, true, false) == true)
                    {
                        if (_boardCursorDestination.Y > GetCursorOffset(0, 0).Y)
                            _boardCursorDestination.Y -= GetCursorOffset(0, 1).Y;
                        else
                            _boardCursorDestination.Y = GetCursorOffset(0, 4).Y;
                    }
                    if (Controls.Down(true, true, false) == true)
                    {
                        if (_boardCursorDestination.Y < GetCursorOffset(0, 4).Y)
                            _boardCursorDestination.Y += GetCursorOffset(0, 1).Y;
                        else
                            _boardCursorDestination.Y = GetCursorOffset(0, 0).Y;
                    }
                    if (Controls.Left(true, true, false) == true)
                    {
                        if (_boardCursorDestination.X > GetCursorOffset(0, 0).X)
                            _boardCursorDestination.X -= GetCursorOffset(1, 0).X;
                        else
                            _boardCursorDestination.X = GetCursorOffset(4, 0).X;
                    }
                    if (Controls.Right(true, true, false) == true)
                    {
                        if (_boardCursorDestination.X < GetCursorOffset(4, 0).X)
                            _boardCursorDestination.X += GetCursorOffset(1, 0).X;
                        else
                            _boardCursorDestination.X = GetCursorOffset(0, 0).X;
                    }

                    _boardCursorPosition.X = MathHelper.Lerp(_boardCursorPosition.X, _boardCursorDestination.X, 0.6F);
                    _boardCursorPosition.Y = MathHelper.Lerp(_boardCursorPosition.Y, _boardCursorDestination.Y, 0.6F);
                }
                else
                {
                    _boardCursorDestination = GetCursorOffset(0, 0);
                    _boardCursorPosition = GetCursorOffset(0, 0);
                }

                if (KeyBoardHandler.KeyPressed(KeyBindings.RunKey) == true || ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.X) == true)
                {
                    if (GameState == States.Game) { GameState = States.Memo; SoundManager.PlaySound("select"); }
                    else if (GameState == States.Memo) { GameState = States.Game; SoundManager.PlaySound("select"); }
                }

                Rectangle buttonRect = new Rectangle((int)(BoardOrigin.X + BoardSize.Width + TileSize.Width / 4), (int)BoardOrigin.Y, _memoMenuSize.Width, _memoMenuSize.Height);
                if (Controls.Accept(true, false, false) == true && MouseHandler.IsInRectangle(buttonRect) == true && _delay == 0)
                {
                    if (GameState == States.Game) { GameState = States.Memo; SoundManager.PlaySound("select"); }
                    else if (GameState == States.Memo) { GameState = States.Game; SoundManager.PlaySound("select"); }
                }

                if (GameState == States.Memo)
                {
                    if (_memoMenuX < _memoMenuSize.Width + TileSize.Width / 4)
                    {
                        _memoMenuX = MathHelper.Lerp(_memoMenuSize.Width + TileSize.Width / 4, _memoMenuX, 0.9F);
                        if (_memoMenuX >= _memoMenuSize.Width + TileSize.Width / 4)
                            _memoMenuX = (int)(_memoMenuSize.Width + TileSize.Width / 4);
                    }

                    if (Controls.Left(true, false, true, false, false, false) == true || ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger) == true)
                    {
                        _memoIndex -= 1;
                        if (_memoIndex < 0) _memoIndex = 3;
                        SoundManager.PlaySound("select");
                    }
                    if (Controls.Right(true, false, true, false, false, false) == true || ControllerHandler.ButtonPressed(Microsoft.Xna.Framework.Input.Buttons.RightTrigger) == true)
                    {
                        _memoIndex += 1;
                        if (_memoIndex > 3) _memoIndex = 0;
                        SoundManager.PlaySound("select");
                    }

                    Rectangle memoRect = new Rectangle((int)(BoardOrigin.X + BoardSize.Width - _memoMenuSize.Width + _memoMenuX), (int)(BoardOrigin.Y + _memoMenuSize.Height + TileSize.Height / 2), _memoMenuSize.Width, _memoMenuSize.Height);
                    if (Controls.Accept(true, false, false) == true)
                    {
                        if (MouseHandler.IsInRectangle(new Rectangle(memoRect.X, memoRect.Y, memoRect.Width / 2, memoRect.Height / 2)) == true) { _memoIndex = 0; SoundManager.PlaySound("select"); }
                        if (MouseHandler.IsInRectangle(new Rectangle(memoRect.X + memoRect.Width / 2, memoRect.Y, memoRect.Width / 2, memoRect.Height / 2)) == true) { _memoIndex = 1; SoundManager.PlaySound("select"); }
                        if (MouseHandler.IsInRectangle(new Rectangle(memoRect.X, memoRect.Y + memoRect.Height / 2, memoRect.Width / 2, memoRect.Height / 2)) == true) { _memoIndex = 2; SoundManager.PlaySound("select"); }
                        if (MouseHandler.IsInRectangle(new Rectangle(memoRect.X + memoRect.Width / 2, memoRect.Y + memoRect.Height / 2, memoRect.Width / 2, memoRect.Height / 2)) == true) { _memoIndex = 3; SoundManager.PlaySound("select"); }
                    }
                }
                else
                {
                    if (_memoMenuX > 0F)
                    {
                        _memoMenuX = MathHelper.Lerp(0F, _memoMenuX, 0.9F);
                        if (_memoMenuX <= 0F) _memoMenuX = 0F;
                    }
                }

                String quitText = Localization.GetString("VoltorbFlip_QuitQuestion_Question_1", "If you quit now, you will~receive") + " " + CurrentCoins.ToString() + " " + Localization.GetString("VoltorbFlip_QuitQuestion_Question_2", "Coin(s).*Will you quit?") + "%" + Localization.GetString("global_yes", "Yes") + "|" + Localization.GetString("global_no", "No") + "%";

                if (Controls.Dismiss(false, true, true) == true && (GameState == States.Game || GameState == States.Memo) && _delay == 0)
                {
                    TextBox.Show(quitText);
                    SoundManager.PlaySound("select");
                    GameState = States.QuitQuestion;
                    _memoMenuX = 0;
                }

                Rectangle quitBtnRect = new Rectangle((int)(GameOrigin.X + 424), (int)(GameOrigin.Y + 448), 128, 56);
                if (Controls.Accept(true, false, false) == true && MouseHandler.IsInRectangle(quitBtnRect) == true && (GameState == States.Game || GameState == States.Memo) && _delay == 0)
                {
                    TextBox.Show(quitText);
                    ChooseBox.CancelIndex = 1;
                    SoundManager.PlaySound("select");
                    GameState = States.QuitQuestion;
                    _memoMenuX = 0;
                }

                if (GameState == States.QuitQuestion)
                {
                    if (ChooseBox.readyForResult == true)
                    {
                        if (ChooseBox.result == 0) { Quit(); ChooseBox.CancelIndex = -1; ChooseBox.readyForResult = false; }
                        else { _delay = 15; GameState = States.Game; ChooseBox.readyForResult = false; }
                    }
                }

                if (Controls.Accept(false, true, true) == true && GameState == States.Game && _delay == 0)
                {
                    Vector2 ct = GetCurrentTile();
                    if (Board[(int)ct.Y][(int)ct.X].Flipped == false) SoundManager.PlaySound("select");
                    Board[(int)ct.Y][(int)ct.X].Flip();
                }

                if (Controls.Accept(true, false, false) == true && GameState == States.Game && MouseHandler.IsInRectangle(new Rectangle((int)BoardOrigin.X, (int)BoardOrigin.Y, BoardSize.Width, BoardSize.Height)) == true && _delay == 0)
                {
                    Vector2 tum = GetTileUnderMouse();
                    _boardCursorDestination = GetCursorOffset((int)tum.X, (int)tum.Y);
                    if (Board[(int)tum.Y][(int)tum.X].Flipped == false) SoundManager.PlaySound("select");
                    Board[(int)tum.Y][(int)tum.X].Flip();
                }

                Vector2 ctm = GetCurrentTile();
                if (Controls.Accept(false, true, true) == true && GameState == States.Memo && Board[(int)ctm.Y][(int)ctm.X].Flipped == false && _delay == 0)
                {
                    if (Board[(int)ctm.Y][(int)ctm.X].GetMemo(_memoIndex) == false) SoundManager.PlaySound("select");
                    Board[(int)ctm.Y][(int)ctm.X].SetMemo(_memoIndex, true);
                }

                if (Controls.Accept(true, false, false) == true && GameState == States.Memo && MouseHandler.IsInRectangle(new Rectangle((int)BoardOrigin.X, (int)BoardOrigin.Y, BoardSize.Width, BoardSize.Height)) == true && _delay == 0)
                {
                    Vector2 tum = GetTileUnderMouse();
                    _boardCursorDestination = GetCursorOffset((int)tum.X, (int)tum.Y);
                    if (Board[(int)tum.Y][(int)tum.X].Flipped == false)
                    {
                        if (Board[(int)tum.Y][(int)tum.X].GetMemo(_memoIndex) == false) SoundManager.PlaySound("select");
                        Board[(int)tum.Y][(int)tum.X].SetMemo(_memoIndex, true);
                    }
                }

                ctm = GetCurrentTile();
                if (Controls.Dismiss(false, true, true) == true && GameState == States.Memo && Board[(int)ctm.Y][(int)ctm.X].Flipped == false && _delay == 0)
                {
                    if (Board[(int)ctm.Y][(int)ctm.X].GetMemo(_memoIndex) == true) SoundManager.PlaySound("select");
                    Board[(int)ctm.Y][(int)ctm.X].SetMemo(_memoIndex, false);
                }

                if (Controls.Dismiss(true, false, false) == true && GameState == States.Memo && MouseHandler.IsInRectangle(new Rectangle((int)BoardOrigin.X, (int)BoardOrigin.Y, BoardSize.Width, BoardSize.Height)) == true && _delay == 0)
                {
                    Vector2 tum = GetTileUnderMouse();
                    _boardCursorDestination = GetCursorOffset((int)tum.X, (int)tum.Y);
                    if (Board[(int)tum.Y][(int)tum.X].Flipped == false)
                    {
                        if (Board[(int)tum.Y][(int)tum.X].GetMemo(_memoIndex) == true) SoundManager.PlaySound("select");
                        Board[(int)tum.Y][(int)tum.X].SetMemo(_memoIndex, false);
                    }
                }
            }
        }

        if (CurrentCoins >= MaxCoins && GameState == States.Game)
        {
            String clearText = Localization.GetString("VoltorbFlip_GameWon_1", "Game clear!~You've found all of the~hidden x2 and x3 cards.*<player.name> received~") + CurrentCoins.ToString() + " " + Localization.GetString("VoltorbFlip_GameWon_2", "Coin(s)!");
            SoundManager.PlaySound("VoltorbFlip\\WinGame");
            TextBox.Show(clearText);
            if (_delay == 0)
            {
                PreviousLevel = CurrentLevel;
                if (CurrentFlips >= 8) TotalFlips += 1;
                CurrentFlips = 0;
                ConsecutiveWins += 1;
                if (ConsecutiveWins == 5 && TotalFlips == 5)
                    CurrentLevel = MaxLevel + 1;
                else
                    CurrentLevel = Math.Min(CurrentLevel + 1, MaxLevel);
                GameState = States.GameWon;
                _delay = 5;
            }
        }

        if (GameState == States.GameWon)
        {
            if (int.Parse(GameModeManager.GetGameRuleValue("CoinCaseCap", "0")) > 0 && Core.Player.Coins + TotalCoins > int.Parse(GameModeManager.GetGameRuleValue("CoinCaseCap", "0")))
            {
                TotalCoins = int.Parse(GameModeManager.GetGameRuleValue("CoinCaseCap", "0")) - Core.Player.Coins;
                CurrentCoins = 0;
                TextBox.Show(Localization.GetString("VoltorbFlip_MaxCoins", "Your Coin Case can't fit~any more Coin(s)!"));
            }
            else { TotalCoins += CurrentCoins; CurrentCoins = 0; }

            int readyAmount = 0;
            for (int row = 0; row < GridSize; row++)
                for (int col = 0; col < GridSize; col++)
                {
                    Board[row][col].Reveal();
                    if (Board[row][col].FlipProgress == 0) readyAmount += 1;
                }

            if (Controls.Accept() == true && readyAmount == GridSize * GridSize && TextBox.Showing == false)
            {
                if (_delay == 0) { SoundManager.PlaySound("select"); _delay = 5; }
                if (_delay > 3)
                {
                    if (CurrentLevel > PreviousLevel)
                        TextBox.Show(Localization.GetString("VoltorbFlip_NewLevel_Higher1", "Advanced to Game Lv.") + " " + CurrentLevel + Localization.GetString("VoltorbFlip_NewLevel_Higher2", "!"));
                    GameState = States.FlipWon;
                }
            }
        }

        if (GameState == States.GameLost)
        {
            CurrentCoins = 0;
            int readyAmount = 0;
            for (int row = 0; row < GridSize; row++)
                for (int col = 0; col < GridSize; col++)
                {
                    Board[row][col].Reveal();
                    if (Board[row][col].FlipProgress == 0) readyAmount += 1;
                }

            if (Controls.Accept() == true && readyAmount == GridSize * GridSize && TextBox.Showing == false)
            {
                PreviousLevel = CurrentLevel;
                if (CurrentFlips < CurrentLevel) CurrentLevel = Math.Max(1, CurrentFlips);
                if (_delay == 0) { SoundManager.PlaySound("select"); _delay = 5; }
                if (_delay > 3)
                {
                    if (CurrentLevel < PreviousLevel)
                        TextBox.Show(Localization.GetString("VoltorbFlip_NewLevel_Lower1", "Dropped to Game Lv.") + " " + CurrentLevel + Localization.GetString("VoltorbFlip_NewLevel_Lower2", "!"));
                    GameState = States.FlipLost;
                }
            }
        }

        if (GameState == States.FlipWon)
        {
            int readyAmount = 0;
            for (int row = 0; row < GridSize; row++)
                for (int col = 0; col < GridSize; col++)
                {
                    Board[row][col].Reset();
                    if (Board[row][col].FlipProgress == 0) readyAmount += 1;
                }
            if (readyAmount == GridSize * GridSize) GameState = States.NewLevelQuestion;
        }

        if (GameState == States.FlipLost && TextBox.Showing == false)
        {
            int readyAmount = 0;
            for (int row = 0; row < GridSize; row++)
                for (int col = 0; col < GridSize; col++)
                {
                    Board[row][col].Reset();
                    if (Board[row][col].FlipProgress == 0) readyAmount += 1;
                }
            CurrentFlips = 0;
            if (readyAmount == GridSize * GridSize) GameState = States.NewLevelQuestion;
        }

        if (GameState == States.NewLevelQuestion)
        {
            switch (_newLevelMenuIndex)
            {
                case 0:
                    if (_delay == 0 && TextBox.Showing == false && ChooseBox.Showing == false)
                    {
                        TextBox.Show(Localization.GetString("VoltorbFlip_BeforeNewLevel_Main_Question_1", "Play Voltorb Flip Lv.") + " " + CurrentLevel.ToString() + Localization.GetString("VoltorbFlip_BeforeNewLevel_Main_Question_2", "?") + "%" + Localization.GetString("VoltorbFlip_BeforeNewLevel_Main_Answer_Play", "Play") + "|" + Localization.GetString("VoltorbFlip_BeforeNewLevel_Main_Answer_GameInfo", "Game Info") + "|" + Localization.GetString("VoltorbFlip_BeforeNewLevel_Main_Answer_Quit", "Quit") + "%");
                        ChooseBox.CancelIndex = 2;
                        _delay = 5;
                    }
                    if (ChooseBox.readyForResult == true)
                    {
                        switch (ChooseBox.result)
                        {
                            case 0: GameState = States.NewLevel; ChooseBox.readyForResult = false; ChooseBox.CancelIndex = -1; break;
                            case 1: _newLevelMenuIndex = 1; ChooseBox.readyForResult = false; ChooseBox.CancelIndex = -1; break;
                            case 2: GameState = States.Closing; ChooseBox.readyForResult = false; ChooseBox.CancelIndex = -1; break;
                        }
                    }
                    break;
                case 1:
                    if (_delay == 0 && TextBox.Showing == false && ChooseBox.Showing == false)
                    {
                        TextBox.Show(Localization.GetString("VoltorbFlip_BeforeNewLevel_GameInfo_Question", "Which set of info?") + "%" + Localization.GetString("VoltorbFlip_BeforeNewLevel_GameInfo_Answer_HowToPlay", "How to Play") + "|" + Localization.GetString("VoltorbFlip_BeforeNewLevel_GameInfo_Answer_Hint", "Hint!") + "|" + Localization.GetString("VoltorbFlip_BeforeNewLevel_GameInfo_Answer_AboutMemos", "About Memos") + "|" + Localization.GetString("VoltorbFlip_BeforeNewLevel_GameInfo_Back", "Back") + "%");
                        ChooseBox.CancelIndex = 3;
                        _delay = 5;
                    }
                    if (ChooseBox.readyForResult == true)
                    {
                        switch (ChooseBox.result)
                        {
                            case 0: _newLevelMenuIndex = 2; ChooseBox.CancelIndex = -1; break;
                            case 1: _newLevelMenuIndex = 3; ChooseBox.CancelIndex = -1; break;
                            case 2: _newLevelMenuIndex = 4; ChooseBox.CancelIndex = -1; break;
                            case 3: _newLevelMenuIndex = 0; ChooseBox.CancelIndex = -1; break;
                        }
                    }
                    break;
                case 2:
                    if (_delay == 0)
                    {
                        TextBox.Show(Localization.GetString("VoltorbFlip_Tutorial_HowToPlay_Message", "Voltorb Flip is a game in which~you flip over cards to find~numbers hidden beneath them.*The cards are hiding the~numbers 1 through 3...~and Voltorb as well.*The first number you flip over~will give you that many Coins.*From then on, the next number~you find will multiply the~total amount of Coins you've~collected by that number.*If it's a 2, your total will~be multiplied by \"x2\".*If it's a 3, your total will~be multiplied by \"x3\".*But if you flip over a~Voltorb, it's game over.*When that happens, you'll lose~all the Coins you've collected~in the current level.*If you select \"Quit\", you'll~withdraw from the level.*If you get to a difficult~spot, you might want to end~the game early.*Once you've found all the~hidden 2 and 3 cards,~you've cleared the game.*Once you've flipped over~all these cards, then you'll~advance to the next level.*As you move up in levels,~you will be able to receive~more Coins. Do your best!"));
                        _delay = 5;
                    }
                    if (TextBox.Showing == false && _delay > 3) { ChooseBox.readyForResult = false; _newLevelMenuIndex = 1; }
                    break;
                case 3:
                    if (_delay == 0)
                    {
                        TextBox.Show(Localization.GetString("VoltorbFlip_Tutorial_Hint_Message", "The numbers at the side~of the board give you a clue~about the numbers hidden on~the backs of the cards.*The larger the number, the~more likely it is that there~are many large numbers hidden~in that row or column.*In the same way, you can tell~how many Voltorb are hidden~in the row or column.*Consider the hidden number~totals and the Voltorb~totals carefully as you~flip over cards."));
                        _delay = 5;
                    }
                    if (TextBox.Showing == false && _delay > 3) { ChooseBox.readyForResult = false; _newLevelMenuIndex = 1; }
                    break;
                case 4:
                    if (_delay == 0)
                    {
                        TextBox.Show(Localization.GetString("VoltorbFlip_Tutorial_AboutMemos_Message", "Select \"Open Memo\" or press~[<system.button(run)>] to open the~Memo Window.*You can mark the cards with~the numbers 1 through 3,~but also with a Voltorb mark.*When you have an idea of the~numbers hidden on the back~of the cards, open the Memo~Window, choose the type of~mark you want to use with~the Mouse Wheel or the~Gamepad's Shoulder Buttons~and then press [<system.button(enter1)>]~while highlighting the card~you want to mark.*If you want to remove a mark,~choose the type of mark you~want to remove with the~Mouse Wheel or the Gamepad's~Shoulder Buttons and then~press [<system.button(back1)>] while~highlighting the card you want~to remove the mark from.*You can also use the~mouse to select a~mark type or a card."));
                        _delay = 5;
                    }
                    if (TextBox.Showing == false && _delay > 3) { ChooseBox.readyForResult = false; _newLevelMenuIndex = 1; }
                    break;
            }
        }

        if (GameState == States.NewLevel)
        {
            if (TextBox.Showing == false)
            {
                SoundManager.PlaySound("VoltorbFlip\\StartGame");
                Board = CreateBoard(CurrentLevel);
                if (CurrentLevel == 8) { TotalFlips = 0; ConsecutiveWins = 0; }
                TextBox.Show(Localization.GetString("VoltorbFlip_NewLevel_Ready1", "Ready to play Game Lv.") + " " + CurrentLevel + Localization.GetString("VoltorbFlip_NewLevel_Ready2", "!"));
            }
            else { _delay = 15; GameState = States.Game; }
        }

        if (GameState == States.Closing)
        {
            CurrentCoins = 0;
            if (_interfaceFade > 0F)
            {
                _interfaceFade = MathHelper.Lerp(0, _interfaceFade, 0.8F);
                if (_interfaceFade < 0F) _interfaceFade = 0F;
            }
            if (_screenTransitionY > 0)
            {
                _screenTransitionY = MathHelper.Lerp(0, _screenTransitionY, 0.8F);
                if (_screenTransitionY <= 0) _screenTransitionY = 0;
            }
            GameOrigin.Y = (int)(Core.windowSize.Height / 2 - _screenTransitionY);
            BoardOrigin = new Vector2(GameOrigin.X + 32, GameOrigin.Y + 160);

            if (_screenTransitionY <= 2.0F)
                Core.SetScreen(PreScreen!);
        }
        else
        {
            int maxWindowHeight = GameSize.Height / 2;
            if (_screenTransitionY < maxWindowHeight)
            {
                _screenTransitionY = MathHelper.Lerp(maxWindowHeight, _screenTransitionY, 0.8F);
                if (_screenTransitionY >= maxWindowHeight - 0.8F)
                {
                    if (GameState == States.Opening) GameState = States.NewLevelQuestion;
                    _screenTransitionY = maxWindowHeight;
                }
            }
            GameOrigin.Y = (int)(Core.windowSize.Height / 2 - _screenTransitionY);
            BoardOrigin = new Vector2(GameOrigin.X + 32, GameOrigin.Y + 160);

            if (_interfaceFade < 1.0F)
            {
                _interfaceFade = MathHelper.Lerp(1, _interfaceFade, 0.95F);
                if (_interfaceFade >= 1.0F) _interfaceFade = 1.0F;
            }
        }
    }

    public void Quit()
    {
        SoundManager.PlaySound("VoltorbFlip\\QuitGame", true);
        TextBox.Show(Localization.GetString("VoltorbFlip_QuitGame_1", "<player.name> received~") + CurrentCoins.ToString() + " " + Localization.GetString("VoltorbFlip_QuitGame_2", "Coin(s)!"));

        if (CurrentFlips < CurrentLevel)
            CurrentLevel = Math.Max(1, CurrentFlips);

        if (_delay == 0)
        {
            if (GameState == States.QuitQuestion || GameState == States.GameWon)
            {
                TotalCoins += CurrentCoins;
                CurrentFlips = 0;
                CurrentCoins = 0;
            }
            _delay = 5;
        }
        if (_delay > 3)
        {
            if (int.Parse(GameModeManager.GetGameRuleValue("CoinCaseCap", "0")) > 0 && Core.Player.Coins + TotalCoins > int.Parse(GameModeManager.GetGameRuleValue("CoinCaseCap", "0")))
            {
                if (CurrentLevel < PreviousLevel)
                    TextBox.Show(Localization.GetString("VoltorbFlip_NewLevel_Lower1", "Dropped to Game Lv.") + " " + CurrentLevel + Localization.GetString("VoltorbFlip_NewLevel_Lower2", "!"));
                GameState = States.Closing;
                ChooseBox.readyForResult = false;
            }
            else
            {
                if (CurrentLevel < PreviousLevel)
                    TextBox.Show(Localization.GetString("VoltorbFlip_NewLevel_Lower1", "Dropped to Game Lv.") + " " + CurrentLevel + Localization.GetString("VoltorbFlip_NewLevel_Lower2", "!"));
                ChooseBox.readyForResult = false;
                GameState = States.NewLevelQuestion;
            }
        }
    }
}

public class Tile
{
    public enum Values
    {
        Voltorb = 0,
        One = 1,
        Two = 2,
        Three = 3
    }

    public int Row { get; set; } = 0;
    public int Column { get; set; } = 0;
    public int Value { get; set; } = (int)Values.Voltorb;
    public bool Flipped { get; set; } = false;
    private bool _memoVoltorb = false;
    private bool _memo1 = false;
    private bool _memo2 = false;
    private bool _memo3 = false;

    private float _flipWidth = 1.0F;
    private bool _activated = false;
    public int FlipProgress { get; set; } = 0;

    public void Flip()
    {
        if (Flipped == false)
        {
            FlipProgress = 3;
            if (Value != (int)Values.Voltorb)
                VoltorbFlipScreen.CurrentFlips += 1;
        }
    }

    public void Reveal()
    {
        if (Flipped == false)
            FlipProgress = 1;
    }

    public void Reset()
    {
        if (Flipped == true) { FlipProgress = 1; _activated = false; }
    }

    public void Draw()
    {
        Color bg = (VoltorbFlipScreen.GameState == VoltorbFlipScreen.States.Closing || VoltorbFlipScreen.GameState == VoltorbFlipScreen.States.Opening)
            ? new Color(255, 255, 255, (int)(255 * VoltorbFlipScreen._interfaceFade))
            : Color.White;

        int tileW = VoltorbFlipScreen.TileSize.Width;
        int tileH = VoltorbFlipScreen.TileSize.Height;

        if (FlipProgress == 1 || FlipProgress == 3)
        {
            if (_flipWidth > 0F) _flipWidth -= 0.1F;
            if (_flipWidth <= 0F)
            {
                _flipWidth = 0F;
                if (Flipped == false)
                {
                    SetMemo(0, false); SetMemo(1, false); SetMemo(2, false); SetMemo(3, false);
                    Flipped = true;
                }
                else Flipped = false;
                FlipProgress += 1;
            }
        }
        if (FlipProgress == 2 || FlipProgress == 4)
        {
            if (_flipWidth < 1.0F) _flipWidth += 0.1F;
            if (_flipWidth >= 1.0F) { _flipWidth = 1.0F; FlipProgress = 0; }
        }

        Core.SpriteBatch.Draw(GetImage(), new Rectangle((int)(VoltorbFlipScreen.BoardOrigin.X + tileW * Column + (tileW - _flipWidth * tileW) / 2), (int)(VoltorbFlipScreen.BoardOrigin.Y + tileH * Row), (int)(tileW * _flipWidth), tileH), bg);

        if (GetMemo(0) == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Tile_MemoIcons", new Rectangle(0, 0, 32, 32), String.Empty), new Rectangle((int)(VoltorbFlipScreen.BoardOrigin.X + tileW * Column + (tileW - _flipWidth * tileW)), (int)(VoltorbFlipScreen.BoardOrigin.Y + tileH * Row), (int)(tileW * _flipWidth), tileH), bg);
        if (GetMemo(1) == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Tile_MemoIcons", new Rectangle(32, 0, 32, 32), String.Empty), new Rectangle((int)(VoltorbFlipScreen.BoardOrigin.X + tileW * Column + (tileW - _flipWidth * tileW)), (int)(VoltorbFlipScreen.BoardOrigin.Y + tileH * Row), (int)(tileW * _flipWidth), tileH), bg);
        if (GetMemo(2) == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Tile_MemoIcons", new Rectangle(64, 0, 32, 32), String.Empty), new Rectangle((int)(VoltorbFlipScreen.BoardOrigin.X + tileW * Column + (tileW - _flipWidth * tileW)), (int)(VoltorbFlipScreen.BoardOrigin.Y + tileH * Row), (int)(tileW * _flipWidth), tileH), bg);
        if (GetMemo(3) == true)
            Core.SpriteBatch.Draw(TextureManager.GetTexture("VoltorbFlip\\Tile_MemoIcons", new Rectangle(96, 0, 32, 32), String.Empty), new Rectangle((int)(VoltorbFlipScreen.BoardOrigin.X + tileW * Column + (tileW - _flipWidth * tileW)), (int)(VoltorbFlipScreen.BoardOrigin.Y + tileH * Row), (int)(tileW * _flipWidth), tileH), bg);
    }

    public void Update()
    {
        if (FlipProgress <= 2)
            _activated = false;
        else
        {
            if (Flipped == true && _activated == false)
            {
                if (Value == (int)Values.Voltorb)
                {
                    if (VoltorbFlipScreen.GameState == VoltorbFlipScreen.States.Game)
                        SoundManager.PlaySound("VoltorbFlip\\LoseGame", true);
                    Screen.TextBox.Show(Localization.GetString("VoltorbFlip_GameLost", "Oh no! You get 0 Coins!"));
                    VoltorbFlipScreen.ConsecutiveWins = 0;
                    VoltorbFlipScreen.GameState = VoltorbFlipScreen.States.GameLost;
                }
                else
                {
                    if (VoltorbFlipScreen.CurrentCoins == 0)
                        VoltorbFlipScreen.CurrentCoins = Value;
                    else
                        VoltorbFlipScreen.CurrentCoins *= Value;
                    _activated = true;
                }
            }
        }
    }

    public Texture2D GetImage()
    {
        if (Flipped == true)
            return TextureManager.GetTexture(VoltorbFlipScreen.Texture_Tile_Front!, new Rectangle(Value * 32, 0, 32, 32));
        else
            return TextureManager.GetTexture(VoltorbFlipScreen.Texture_Tile_Back!, new Rectangle(0, 0, 32, 32));
    }

    public bool GetMemo(int memoNumber)
    {
        return memoNumber switch
        {
            0 => _memoVoltorb,
            1 => _memo1,
            2 => _memo2,
            3 => _memo3,
            _ => false
        };
    }

    public void SetMemo(int memoNumber, bool value)
    {
        switch (memoNumber)
        {
            case (int)Values.Voltorb: _memoVoltorb = value; break;
            case (int)Values.One: _memo1 = value; break;
            case (int)Values.Two: _memo2 = value; break;
            case (int)Values.Three: _memo3 = value; break;
        }
    }

    public Tile(int row, int column, int value, bool flipped)
    {
        Row = row;
        Column = column;
        Value = value;
        Flipped = flipped;
    }
}
