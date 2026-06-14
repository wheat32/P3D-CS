using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class JoinServerScreen : Screen
{
    private static int _barAnimationState = 0;

    private List<Server> _serverList = [];

    private int _selectIndex = 0;
    private int _scrollIndex = 0;
    private int _buttonIndex = 0;

    private bool _loadOnlineServers = true;

    public static Server? SelectedServer = null;
    public static bool Online = false;

    public static List<Thread> ClearThreadList = [];

    public JoinServerScreen(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.JoinServerScreen;
        MouseVisible = true;
        CanBePaused = false;
        CanChat = false;
    }

    private void LoadServers()
    {
        _serverList.Clear();

        Server localServer = new Server("Local Server", "127.0.0.1");
        localServer.IsLocal = true;

        _serverList.Add(localServer);

        if (File.Exists(GameController.GamePath + @"\Save\server_list.dat") == false)
        {
            File.WriteAllText(GameController.GamePath + @"\Save\server_list.dat",
                "Official Pokémon3D Server,karp.pokemon3d.net:15134");
        }

        if (_loadOnlineServers == true)
        {
            String[] data = File.ReadAllLines(GameController.GamePath + @"\Save\server_list.dat");
            if (data.Length > 0)
            {
                foreach (String line in data)
                {
                    if (line.CountSeperators(",") == 1)
                    {
                        String name = line.Split(',')[0];
                        String address = line.Split(',')[1];
                        _serverList.Add(new Server(name, address));
                    }
                }
            }
        }

        foreach (Server s in _serverList)
        {
            s.Ping();
        }
    }

    public override void Draw()
    {
        int tx = (int)World.CurrentSeason;
        int ty = 0;
        if (tx > 1)
        {
            tx -= 2;
            ty += 1;
        }

        int serversToDisplay = GetServersToDisplay();

        Texture2D pattern = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(160 + tx * 16, ty * 16, 16, 16), String.Empty);
        for (int dx = 0; dx <= Core.ScreenSize.Width; dx += 128)
        {
            for (int dy = 0; dy <= Core.ScreenSize.Height; dy += 128)
            {
                Core.SpriteBatch.DrawInterface(pattern, new Rectangle(dx, dy, 128, 128), Color.White);
            }
        }

        Canvas.DrawRectangle(new Rectangle(0, 72, Core.ScreenSize.Width, Core.ScreenSize.Height - 240), new Color(0, 0, 0, 128), true);

        String title = Localization.GetString("join_server_screen_title", "Join a Server:");
        String titleMeasure = Localization.GetString("join_server_screen_title", "Join a server:");
        Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, title,
            new Vector2((int)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(titleMeasure).X) + 4, 14 + 4),
            Color.Black, 0.0f, new Vector2(0), 2.0f, SpriteEffects.None, 0.0f);
        Core.SpriteBatch.DrawInterfaceString(FontManager.MainFont, title,
            new Vector2((int)(Core.ScreenSize.Width / 2 - FontManager.MainFont.MeasureString(titleMeasure).X), 14),
            Color.White, 0.0f, new Vector2(0), 2.0f, SpriteEffects.None, 0.0f);

        int endX = _serverList.Count - 1;
        endX = (int)MathHelper.Clamp(endX, 0, serversToDisplay - 1);

        if (_serverList.Count > serversToDisplay)
        {
            Canvas.DrawScrollBar(new Vector2((float)(Core.ScreenSize.Width / 2 + 320), 100),
                _serverList.Count, 1, _selectIndex,
                new Size(8, Core.ScreenSize.Height - 300), false, Color.Black, Color.Gray, true);
        }

        for (int i = 0; i <= endX; i++)
        {
            int index = i + _scrollIndex;
            if (_serverList.Count - 1 >= index)
            {
                _serverList[index].Draw(new Vector2(0, i * 100 + 100), index == _selectIndex);
            }
        }

        Color fontShadow = new Color(0, 0, 0, 0);
        for (int i = 0; i <= 5; i++)
        {
            Texture2D canvasTexture;
            Color fontColor;
            if (i == _buttonIndex)
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
                fontColor = Color.White;
                fontShadow.A = 255;
            }
            else
            {
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
                fontColor = Color.Black;
                fontShadow.A = 0;
            }

            String text = String.Empty;
            switch (i)
            {
                case 0:
                    text = Localization.GetString("join_server_screen_button_join", "Join");
                    Server s0 = _serverList[_selectIndex];
                    if (s0.IsLocal == true)
                    {
                        text = Localization.GetString("join_server_screen_button_play", "Play");
                    }
                    else
                    {
                        if (s0.CanJoin() == false)
                        {
                            canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
                        }
                    }
                    break;
                case 1:
                    text = Localization.GetString("join_server_screen_button_refresh", "Refresh");
                    break;
                case 2:
                    text = Localization.GetString("join_server_screen_button_add", "Add");
                    break;
                case 3:
                    text = Localization.GetString("join_server_screen_button_edit", "Edit");
                    Server s3 = _serverList[_selectIndex];
                    if (s3.IsLocal == true)
                    {
                        canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
                    }
                    break;
                case 4:
                    text = Localization.GetString("join_server_screen_button_remove", "Remove");
                    Server s4 = _serverList[_selectIndex];
                    if (s4.IsLocal == true)
                    {
                        canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
                    }
                    break;
                case 5:
                    text = Localization.GetString("global_back", "Back");
                    break;
            }

            Canvas.DrawImageBorder(canvasTexture, 2,
                new Rectangle((int)(Core.ScreenSize.Width / 2) - 560 + i * 192, Core.ScreenSize.Height - 136, 128, 64), true);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                new Vector2((int)(Core.ScreenSize.Width / 2) - 542 + i * 192 + 2, Core.ScreenSize.Height - 104 + 2),
                fontShadow);
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, text,
                new Vector2((int)(Core.ScreenSize.Width / 2) - 542 + i * 192, Core.ScreenSize.Height - 104),
                fontColor);
        }

        String vS = "Protocol version: " + Servers.ServersManager.PROTOCOLVERSION;
        Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, vS,
            new Vector2(Core.ScreenSize.Width - FontManager.MiniFont.MeasureString(vS).X - 4,
                Core.ScreenSize.Height - FontManager.MiniFont.MeasureString(vS).Y - 1),
            Color.White);

        for (int i = 0; i <= endX; i++)
        {
            int index = i + _scrollIndex;
            if (_serverList.Count - 1 >= index)
            {
                _serverList[index].DrawPlayerListToolTip(new Vector2(0, i * 100 + 100));
            }
        }
    }

    public override void Update()
    {
        if (_loadOnlineServers == false)
        {
            JoinButton();
        }

        int serversToDisplay = GetServersToDisplay();

        if (Controls.Up(true, true, true) == true)
        {
            _selectIndex -= 1;
            if (_selectIndex - _scrollIndex < 0)
            {
                _scrollIndex -= 1;
            }
        }
        if (Controls.Down(true, true, true) == true)
        {
            _selectIndex += 1;
            if (_selectIndex + _scrollIndex > serversToDisplay - 1)
            {
                _scrollIndex += 1;
            }
        }

        if (Core.GameInstance.IsMouseVisible == true)
        {
            for (int i = 0; i <= 5; i++)
            {
                if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 560 + i * 192,
                    Core.ScreenSize.Height - 138, 128 + 32, 64 + 32)).Contains(MouseHandler.MousePosition) == true)
                {
                    _buttonIndex = i;

                    if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true &&
                        MouseHandler.ButtonPressed(MouseHandler.MouseButtons.RightButton) == false)
                    {
                        switch (_buttonIndex)
                        {
                            case 0:
                                SoundManager.PlaySound("select");
                                JoinButton();
                                break;
                            case 1:
                                SoundManager.PlaySound("select");
                                RefreshButton();
                                break;
                            case 2:
                                SoundManager.PlaySound("select");
                                AddServerButton();
                                break;
                            case 3:
                                SoundManager.PlaySound("select");
                                EditServerButton();
                                break;
                            case 4:
                                SoundManager.PlaySound("select");
                                RemoveServerButton();
                                break;
                            case 5:
                                SoundManager.PlaySound("select");
                                CancelButton();
                                break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i <= serversToDisplay - 1; i++)
        {
            if (Core.ScaleScreenRec(new Rectangle((int)(Core.ScreenSize.Width / 2) - 354,
                i * 100 + 100, 500, 80)).Contains(MouseHandler.MousePosition) == true)
            {
                if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                {
                    _selectIndex = i + _scrollIndex;
                }
            }
        }

        if (Controls.Right(true, true, false) == true)
        {
            _buttonIndex += 1;
        }
        if (Controls.Left(true, true, false) == true)
        {
            _buttonIndex -= 1;
        }

        _buttonIndex = (int)MathHelper.Clamp(_buttonIndex, 0, 5);

        if (Controls.Accept(false, true) == true)
        {
            switch (_buttonIndex)
            {
                case 0:
                    SoundManager.PlaySound("select");
                    JoinButton();
                    break;
                case 1:
                    SoundManager.PlaySound("select");
                    RefreshButton();
                    break;
                case 2:
                    SoundManager.PlaySound("select");
                    AddServerButton();
                    break;
                case 3:
                    SoundManager.PlaySound("select");
                    EditServerButton();
                    break;
                case 4:
                    SoundManager.PlaySound("select");
                    RemoveServerButton();
                    break;
                case 5:
                    SoundManager.PlaySound("select");
                    CancelButton();
                    break;
            }
        }

        if (Controls.Dismiss() == true)
        {
            Core.Player.Unload();
            Core.SetScreen(PreScreen);
            SoundManager.PlaySound("select");
        }

        _barAnimationState += 1;
        if (_barAnimationState > 49)
        {
            _barAnimationState = 0;
        }

        _selectIndex = (int)MathHelper.Clamp(_selectIndex, 0, _serverList.Count - 1);
        _scrollIndex = (int)MathHelper.Clamp(_scrollIndex, 0, _serverList.Count - serversToDisplay);
    }

    private void JoinButton()
    {
        if (_selectIndex == 0)
        {
            Online = false;
            SelectedServer = null;
            Core.SetScreen(new OverworldScreen());
        }
        else
        {
            if (_serverList[_selectIndex].CanJoin() == true)
            {
                SelectedServer = _serverList[_selectIndex];
                _serverList.Move(_selectIndex, 1);
                SaveServerList();
                Core.SetScreen(new OverworldScreen());
                Core.SetScreen(new ConnectScreen(ConnectScreen.Modes.Connect,
                    "Connecting to server",
                    Localization.GetString("global_please_wait", "Please wait") + "...",
                    Core.CurrentScreen));
            }
        }
    }

    private void AddServerButton()
    {
        Core.SetScreen(new AddServerScreen(this, new List<P3D.Server>(), true, null!));
    }

    private void EditServerButton()
    {
        Server s = _serverList[_selectIndex];
        if (s.IsLocal == false)
        {
            _serverList.RemoveAt(_selectIndex);
            SaveServerList();
            Core.SetScreen(new AddServerScreen(this, new List<P3D.Server>(), false, null!));
        }
    }

    private void RemoveServerButton()
    {
        Server s = _serverList[_selectIndex];
        if (s.IsLocal == false)
        {
            _serverList.RemoveAt(_selectIndex);
            SaveServerList();
            LoadServers();
        }
    }

    private void CancelButton()
    {
        Core.SetScreen(PreScreen);
    }

    private void RefreshButton()
    {
        foreach (Server server in _serverList)
        {
            server.Refresh();
        }
    }

    private int GetServersToDisplay()
    {
        int contentHeight = Core.ScreenSize.Height - 300;
        int serverHeight = 80;
        int serverCount = 1;

        while (contentHeight > serverHeight + 80)
        {
            serverHeight += 100;
            serverCount += 1;
        }

        return serverCount;
    }

    public override void ChangeTo()
    {
        _loadOnlineServers = Controls.ShiftDown() == false;
        LoadServers();
    }

    public override void ChangeFrom()
    {
        foreach (Thread t in ClearThreadList)
        {
            // Thread.Abort() is not supported in .NET Core; threads are background threads and will stop naturally.
        }
        ClearThreadList.Clear();
    }

    private void SaveServerList()
    {
        String data = String.Empty;
        foreach (Server s in _serverList)
        {
            if (s.IsLocal == false)
            {
                if (data != String.Empty)
                {
                    data += Environment.NewLine;
                }
                data += s.ToString();
            }
        }
        File.WriteAllText(GameController.GamePath + @"\Save\server_list.dat", data);
    }

    public static void AddServerMessage(String m, String serverName)
    {
        if (File.Exists(GameController.GamePath + @"\Save\server_list.dat") == false)
        {
            File.WriteAllText(GameController.GamePath + @"\Save\server_list.dat", String.Empty);
        }

        String newData = String.Empty;

        String[] data = File.ReadAllLines(GameController.GamePath + @"\Save\server_list.dat");
        foreach (String line in data)
        {
            if (newData != String.Empty)
            {
                newData += Environment.NewLine;
            }
            if (line.StartsWith(serverName + ",") == true)
            {
                String[] lineData = line.Split(',');
                newData += lineData[0] + "," + lineData[1] + "," + m;
            }
            else
            {
                newData += line;
            }
        }

        File.WriteAllText(GameController.GamePath + @"\Save\server_list.dat", newData);
    }

    public class Server : Servers.Server
    {
        public bool IsLocal = false;
        public String IdentifierName = String.Empty;
        private String _name = String.Empty;
        public int PingResult = 0;
        public bool Pinged = false;
        public bool StartedPing = false;
        public String ServerMessage = String.Empty;
        public List<String> PlayerList = [];
        public int CurrentPlayersOnline = 0;
        public int MaxPlayersOnline = 0;
        public String ServerProtocolVersion = String.Empty;
        private bool _receivedError = false;

        public Server(String name, String address) : base(address)
        {
            IdentifierName = name;
            Pinged = false;
            StartedPing = false;
        }

        public void Refresh()
        {
            _name = IdentifierName;
            Pinged = false;
            StartedPing = false;
            _receivedError = false;
            CurrentPlayersOnline = 0;
            MaxPlayersOnline = 0;
            ServerProtocolVersion = String.Empty;
            ServerMessage = String.Empty;
            Ping();
        }

        public String GetName()
        {
            if (_name == String.Empty)
            {
                return IdentifierName;
            }
            return _name;
        }

        public void Ping()
        {
            if (IsLocal == true)
            {
                _name = Localization.GetString("join_server_screen_local_server_title", "Local");
                PingResult = 0;
                StartedPing = true;
                Pinged = true;
                CurrentPlayersOnline = 0;
                MaxPlayersOnline = 1;
                IP = "127.0.0.1";
                Port = "15124";
                ServerMessage = Localization.GetString("join_server_screen_local_server_description",
                    "Single-player: Play on your local computer.");
                ServerProtocolVersion = Servers.ServersManager.PROTOCOLVERSION;
            }
            else
            {
                Thread t = new Thread(StartPing);
                t.IsBackground = true;
                t.Start();
                ClearThreadList.Add(t);
                StartedPing = true;
            }
        }

        public bool CanJoin()
        {
            if (IsLocal == true)
            {
                return true;
            }
            if (StartedPing == true && Pinged == true)
            {
                if (ServerProtocolVersion == Servers.ServersManager.PROTOCOLVERSION)
                {
                    if (CurrentPlayersOnline < MaxPlayersOnline)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void StartPing()
        {
            // TODO Phase 9: TCP ping to server for player count / version info
            _receivedError = true;
            Pinged = true;
        }

        public int GetPingTime()
        {
            if (Pinged == true && StartedPing == true)
            {
                if (_receivedError == true)
                {
                    return 0;
                }
                else
                {
                    return PingResult;
                }
            }
            else
            {
                return 0;
            }
        }

        public String GetServerStatus()
        {
            if (_receivedError == true)
            {
                return Localization.GetString("join_server_screen_server_unavailable", "Cannot reach server.");
            }
            if (Pinged == true)
            {
                return Localization.GetString("join_server_screen_server_online", "Server online.");
            }
            else
            {
                return Localization.GetString("join_server_screen_server_polling", "Polling") + LoadingDots.Dots;
            }
        }

        public void Draw(Vector2 startPos, bool selected)
        {
            int width = 608;
            startPos.X = (int)(Core.ScreenSize.Width / 2 - width / 2);
            if (selected == true)
            {
                Canvas.DrawRectangle(new Rectangle((int)startPos.X, (int)startPos.Y, width, 80),
                    new Color(0, 0, 0, 200), true);
                Canvas.DrawBorder(2, new Rectangle((int)startPos.X, (int)startPos.Y, width, 80),
                    Color.LightGray, true);
            }
            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, GetName(),
                new Vector2((int)startPos.X + 4, (int)startPos.Y + 3),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            if (_receivedError == true)
            {
                Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, GetServerStatus(),
                    new Vector2((int)startPos.X + 4, (int)startPos.Y + 30),
                    new Color(190, 0, 0, 255), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                Core.SpriteBatch.DrawInterface(
                    TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(150, 224, 14, 14), String.Empty),
                    new Rectangle((int)startPos.X + width - 32, (int)startPos.Y + 3, 28, 28),
                    Color.White);

                if (new Rectangle((int)startPos.X + width - 32, (int)startPos.Y + 3, 28, 28)
                    .Contains(MouseHandler.MousePosition) == true)
                {
                    Canvas.DrawRectangle(new Rectangle(MouseHandler.MousePosition.X + 10,
                        MouseHandler.MousePosition.Y + 10, 160, 32), Color.Black);
                    Canvas.DrawBorder(3, new Rectangle(MouseHandler.MousePosition.X + 10,
                        MouseHandler.MousePosition.Y + 10, 160, 32), Color.Gray);
                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont,
                        Localization.GetString("join_server_screen_no_connection", "(no connection)"),
                        new Vector2(MouseHandler.MousePosition.X + 14, MouseHandler.MousePosition.Y + 16),
                        Color.White);
                }
            }
            else
            {
                if (Pinged == true)
                {
                    String message = ServerMessage;
                    Color color = new Color(180, 180, 180, 255);

                    if (CanJoin() == false)
                    {
                        if (CurrentPlayersOnline >= MaxPlayersOnline)
                        {
                            message = Localization.GetString("join_server_screen_server_full",
                                "The server is full.");
                        }
                        if (ServerProtocolVersion != Servers.ServersManager.PROTOCOLVERSION)
                        {
                            message = Localization.GetString("join_server_screen_version_mismatch",
                                "Version doesn't match the server's version.");
                        }

                        color = new Color(190, 0, 0, 255);
                    }

                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, message,
                        new Vector2((int)startPos.X + 4, (int)startPos.Y + 30),
                        color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont,
                        CurrentPlayersOnline + "/" + MaxPlayersOnline,
                        new Vector2((int)startPos.X + width - 36 -
                            FontManager.InGameFont.MeasureString(CurrentPlayersOnline + "/" + MaxPlayersOnline).X,
                            (int)startPos.Y + 9),
                        Color.LightGray);
                    Core.SpriteBatch.DrawInterface(
                        TextureManager.GetTexture(@"GUI\Menus\Menu",
                            new Rectangle(80 + 14 * (4 - GetPingLevel()), 238, 14, 14), String.Empty),
                        new Rectangle((int)startPos.X + width - 30, (int)startPos.Y + 3, 28, 28),
                        Color.White);

                    if (new Rectangle((int)startPos.X + width - 32, (int)startPos.Y + 3, 28, 28)
                        .Contains(MouseHandler.MousePosition) == true)
                    {
                        Canvas.DrawRectangle(new Rectangle(MouseHandler.MousePosition.X + 10,
                            MouseHandler.MousePosition.Y + 10, 160, 32), Color.Black);
                        Canvas.DrawBorder(3, new Rectangle(MouseHandler.MousePosition.X + 10,
                            MouseHandler.MousePosition.Y + 10, 160, 32), Color.Gray);
                        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont,
                            Localization.GetString("join_server_screen_ping", "Ping:") + " " + PingResult + " ms",
                            new Vector2(MouseHandler.MousePosition.X + 14, MouseHandler.MousePosition.Y + 16),
                            Color.White);
                    }
                }
                else
                {
                    Core.SpriteBatch.DrawInterface(
                        TextureManager.GetTexture(@"GUI\Menus\Menu",
                            new Rectangle(80 + 14 * (int)Math.Floor(_barAnimationState / 10.0), 224, 14, 14),
                            String.Empty),
                        new Rectangle((int)startPos.X + width - 32, (int)startPos.Y + 3, 28, 28),
                        Color.White);
                    if (new Rectangle((int)startPos.X + width - 32, (int)startPos.Y + 3, 28, 28)
                        .Contains(MouseHandler.MousePosition) == true)
                    {
                        Canvas.DrawRectangle(new Rectangle(MouseHandler.MousePosition.X + 10,
                            MouseHandler.MousePosition.Y + 10, 160, 32), Color.Black);
                        Canvas.DrawBorder(3, new Rectangle(MouseHandler.MousePosition.X + 10,
                            MouseHandler.MousePosition.Y + 10, 160, 32), Color.Gray);
                        Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont,
                            Localization.GetString("join_server_screen_server_polling", "Polling") + LoadingDots.Dots,
                            new Vector2(MouseHandler.MousePosition.X + 14, MouseHandler.MousePosition.Y + 16),
                            Color.White);
                    }

                    Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont,
                        Localization.GetString("join_server_screen_server_polling", "Polling") + LoadingDots.Dots,
                        new Vector2((int)startPos.X + 4, (int)startPos.Y + 30),
                        new Color(180, 180, 180, 255), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            Core.SpriteBatch.DrawInterfaceString(FontManager.InGameFont, GetAddressString(),
                new Vector2((int)startPos.X + 4, (int)startPos.Y + 53),
                new Color(180, 180, 180, 255), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }

        public void DrawPlayerListToolTip(Vector2 startPos)
        {
            if (_receivedError == false && Pinged == true && IsLocal == false)
            {
                int width = 500;
                startPos.X = (int)(Core.ScreenSize.Width / 2 - width / 2);

                int playerCountWidth = (int)FontManager.MiniFont.MeasureString(CurrentPlayersOnline + "/" + MaxPlayersOnline).X;
                if (Core.ScaleScreenRec(new Rectangle(
                    (int)startPos.X + width - 36 - playerCountWidth,
                    (int)startPos.Y + 3, playerCountWidth, 28)).Contains(MouseHandler.MousePosition) == true)
                {
                    String tooltipText = Localization.GetString("join_server_screen_tooltip_no_players",
                        "No players on the server.");

                    if (PlayerList.Count > 0)
                    {
                        tooltipText = PlayerList.ToArray().ArrayToString(true);
                    }

                    Vector2 v = FontManager.MiniFont.MeasureString(
                        Localization.GetString("join_server_screen_tooltip_player_list", "Player List") +
                        Environment.NewLine + tooltipText);

                    int drawY = MouseHandler.MousePosition.Y + 10;
                    if (drawY + v.Y + 12 > Core.windowSize.Height)
                    {
                        drawY = (int)(Core.windowSize.Height - v.Y - 22);
                    }
                    if (drawY < 0)
                    {
                        drawY = 0;
                    }

                    Canvas.DrawRectangle(new Rectangle(MouseHandler.MousePosition.X + 10, drawY,
                        (int)(v.X + 10), (int)(v.Y + 22)), Color.Black, true);
                    Canvas.DrawBorder(3, new Rectangle(MouseHandler.MousePosition.X + 10, drawY,
                        (int)(v.X + 10), (int)(v.Y + 22)), Color.Gray, true);

                    Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont,
                        Localization.GetString("join_server_screen_tooltip_player_list", "Player List"),
                        new Vector2(MouseHandler.MousePosition.X + 14, drawY + 6),
                        Color.LightBlue);
                    Core.SpriteBatch.DrawInterfaceString(FontManager.MiniFont, tooltipText,
                        new Vector2(MouseHandler.MousePosition.X + 14, drawY + 6 + 34),
                        Color.White);
                }
            }
        }

        public String GetAddressString() => IP + ":" + Port;

        private int GetPingLevel()
        {
            if (PingResult < 500)
            {
                return 0;
            }
            else if (PingResult >= 500 && PingResult < 1000)
            {
                return 1;
            }
            else if (PingResult >= 1000 && PingResult < 2000)
            {
                return 2;
            }
            else if (PingResult >= 2000 && PingResult < 5000)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public override String ToString() => IdentifierName + "," + GetAddressString();
    }
}
