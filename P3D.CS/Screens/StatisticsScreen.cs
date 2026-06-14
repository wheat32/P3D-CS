using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Items;

namespace P3D;

public class StatisticsScreen : Screen
{
    private Dictionary<String, int> _statistics = [];
    private Texture2D _texture = null!;
    private int _tileOffset = 0;
    private int _scroll = 0;

    private Dictionary<String, decimal> _gjStatistics = [];
    private int _gjGrabIndex = 0;
    private bool _gjCanGrabNewScore = false;
    private float _gjGrabDelay = 0.0F;
    private int _statisticsStartIndex = 0;

    public StatisticsScreen(Screen currentScreen)
    {
        PreScreen = currentScreen;
        Identification = Identifications.StatisticsScreen;

        _texture = TextureManager.GetTexture("GUI\\Menus\\General");

        CanBePaused = true;
        CanMuteAudio = true;

        if (Core.Player.IsGameJoltSave == true)
            _statisticsStartIndex = 2;

        LoadStatistics();
    }

    private void LoadStatistics()
    {
        if (Core.Player.IsGameJoltSave == true)
        {
            _statistics.Add("Level", GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points));
            _statistics.Add("Points", Core.GameJoltSave.Points);
        }

        String data = BattleSystem.PlayerStatistics.GetData();
        foreach (String line in data.SplitAtNewline())
        {
            if (line.Contains(",") == false) continue;
            String statName = line.Remove(line.IndexOf(","));
            int statValue = int.Parse(line.Remove(0, line.IndexOf(",") + 1));
            if (_statistics.ContainsKey(statName) == true) _statistics.Remove(statName);
            _statistics.Add(statName, statValue);
        }

        if (_statistics.Count > _statisticsStartIndex)
        {
            _gjGrabIndex = _statisticsStartIndex;
            _gjCanGrabNewScore = true;
        }
    }

    public override void Draw()
    {
        Canvas.DrawRectangle(Core.windowSize, new Color(84, 198, 216));

        for (int i = 0; i <= _statistics.Count - 1; i++)
        {
            String itemID = String.Empty;
            int itemIDX = 0;
            String name = _statistics.Keys.ElementAt(i);
            if (name.StartsWith("[") == true && name.Contains("]") == true)
            {
                itemID = name.Remove(name.IndexOf("]")).Remove(0, 1);
                itemIDX = 44;
                name = name.Remove(0, name.IndexOf("]") + 1);
            }

            int value = _statistics.Values.ElementAt(i);

            if (itemID != String.Empty)
            {
                Item? item = Item.GetItemByID(itemID);
                if (item != null) Core.SpriteBatch.Draw(item.Texture, new Rectangle(140, 152 + i * 50 + _scroll, 48, 48), Color.White);
            }

            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("statistics_screen_" + name, name), new Vector2(144 + itemIDX, 160 + i * 50 + _scroll), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);

            String statKey = _statistics.Keys.ElementAt(i);
            if (_gjStatistics.ContainsKey(statKey) == true)
            {
                String gjLabel = _gjStatistics[statKey].ToString();
                Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("statistics_screen_" + gjLabel, gjLabel), new Vector2(Core.windowSize.Width - 418, 180 + i * 50 + _scroll), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
                Core.SpriteBatch.DrawString(FontManager.MainFont, value.ToString(), new Vector2(Core.windowSize.Width - 420, 150 + i * 50 + _scroll), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
            }
            else
                Core.SpriteBatch.DrawString(FontManager.MainFont, value.ToString(), new Vector2(Core.windowSize.Width - 420, 160 + i * 50 + _scroll), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);

            Canvas.DrawRectangle(new Rectangle(130, 200 + i * 50 + _scroll, Core.windowSize.Width - 360, 1), Color.White);
        }

        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, 150), new Color(84, 198, 216));
        Canvas.DrawRectangle(new Rectangle(0, Core.windowSize.Height - 100, Core.windowSize.Width, 100), new Color(84, 198, 216));

        Canvas.DrawGradient(new Rectangle(50, 150, 50, 2), new Color(255, 255, 255, 0), Color.White, true, -1);
        Canvas.DrawRectangle(new Rectangle(100, 150, Core.windowSize.Width - 300, 2), Color.White);
        Canvas.DrawGradient(new Rectangle(Core.windowSize.Width - 200, 150, 50, 2), Color.White, new Color(255, 255, 255, 0), true, -1);

        Canvas.DrawGradient(new Rectangle(Core.windowSize.Width - 450, 100, 2, 50), new Color(255, 255, 255, 0), Color.White, false, -1);
        Canvas.DrawRectangle(new Rectangle(Core.windowSize.Width - 450, 150, 2, Core.windowSize.Height - 250), Color.White);
        Canvas.DrawGradient(new Rectangle(Core.windowSize.Width - 450, Core.windowSize.Height - 100, 2, 50), Color.White, new Color(255, 255, 255, 0), false, -1);

        Canvas.DrawGradient(new Rectangle(50, Core.windowSize.Height - 100, 50, 2), new Color(255, 255, 255, 0), Color.White, true, -1);
        Canvas.DrawRectangle(new Rectangle(100, Core.windowSize.Height - 100, Core.windowSize.Width - 300, 2), Color.White);
        Canvas.DrawGradient(new Rectangle(Core.windowSize.Width - 200, Core.windowSize.Height - 100, 50, 2), Color.White, new Color(255, 255, 255, 0), true, -1);

        for (int y = -64; y <= Core.windowSize.Height; y += 64)
            Core.SpriteBatch.Draw(_texture, new Rectangle(Core.windowSize.Width - 128, y + _tileOffset, 128, 64), new Rectangle(48, 0, 16, 16), Color.White);

        Canvas.DrawGradient(new Rectangle(0, 0, Core.windowSize.Width, 100), new Color(42, 167, 198), new Color(42, 167, 198, 0), false, -1);
        Canvas.DrawGradient(new Rectangle(0, Core.windowSize.Height - 100, Core.windowSize.Width, 100), new Color(42, 167, 198, 0), new Color(42, 167, 198), false, -1);

        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("statistics_screen_header_Name", "Name"), new Vector2(150, 110), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("statistics_screen_header_Value", "Value"), new Vector2(Core.windowSize.Width - 420, 110), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("pokegear_screen_main_function_Statistics", "Statistics"), new Vector2(100, 24), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F);
    }

    public override void Update()
    {
        if (Controls.Up(false, true, false, true, true, true) == true)
            _scroll += Controls.ShiftDown() == true ? 14 : 7;
        if (Controls.Down(false, true, false, true, true, true) == true)
            _scroll -= Controls.ShiftDown() == true ? 14 : 7;
        if (Controls.Up(true, false, true, false, false, false) == true)
            _scroll += Controls.ShiftDown() == true ? 70 : 35;
        if (Controls.Down(true, false, true, false, false, false) == true)
            _scroll -= Controls.ShiftDown() == true ? 70 : 35;

        int minScroll = -_statistics.Count * 50 + Core.windowSize.Height - 250;
        if (minScroll >= 0) _scroll = 0;
        else _scroll = Math.Clamp(_scroll, minScroll, 0);

        if (_gjCanGrabNewScore == true)
        {
            if (_gjGrabDelay <= 0.0F)
            {
                _gjCanGrabNewScore = false;
                GameJolt.GameJoltStatistics.GetStatisticValue(_statistics.Keys.ElementAt(_gjGrabIndex), GetGJStatistic);
            }
            else
                _gjGrabDelay -= 0.1F;
        }

        if (Controls.Dismiss() == true)
        {
            SoundManager.PlaySound("select");
            Core.SetScreen(new TransitionScreen(this, PreScreen!, Color.White, false));
        }

        _tileOffset += 1;
        if (_tileOffset >= 64) _tileOffset = 0;
    }

    private void GetGJStatistic(String result)
    {
        String statName = _statistics.Keys.ElementAt(_gjGrabIndex);
        String statValue = "0";

        List<GameJolt.API.JoltValue> list = GameJolt.API.HandleData(result);
        if (list.Count > 1 && bool.Parse(list[0].Value) == true)
        {
            statValue = list[1].Value;
            if (_gjStatistics.ContainsKey(statName) == true) _gjStatistics[statName] = decimal.Parse(statValue);
            else _gjStatistics.Add(statName, decimal.Parse(statValue));
        }

        _gjGrabIndex += 1;
        if (_gjGrabIndex > _statistics.Count - 1)
        {
            _gjGrabIndex = _statisticsStartIndex;
            _gjGrabDelay = 25.0F + (float)_statistics.Count;
        }

        _gjCanGrabNewScore = true;
    }
}
