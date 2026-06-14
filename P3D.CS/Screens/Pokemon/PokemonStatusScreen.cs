using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.BattleSystem;

namespace P3D;

public class PokemonStatusScreen : Screen
{
    private int _pageIndex = 0;
    private int _pokeIndex = 0;
    private int _boxIndex = 0;
    private Pokemon[] _boxPokemon;
    private Texture2D _mainTexture;
    private Pokemon _pokemon;
    private bool _frontView = true;
    private int _attackIndex = 0;
    private bool _attackToggle = false;
    private float _attackPos = 0;
    private int _switchIndex = -1;

    private bool _viewParty = true;

    private List<Color> _evColors = new List<Color>
    {
        new Color(0, 210, 0), new Color(253, 83, 0), new Color(0, 154, 226),
        new Color(253, 183, 97), new Color(100, 201, 226), new Color(178, 108, 204)
    };

    public PokemonStatusScreen(Screen currentScreen, int index, Pokemon[] boxPokemon, Pokemon pokemon, bool viewParty)
    {
        Identification = Identifications.PokemonStatusScreen;
        PreScreen = currentScreen;
        _pokeIndex = index;
        _pokemon = pokemon;
        _boxPokemon = boxPokemon;
        _boxIndex = index;
        _viewParty = viewParty;

        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");

        _pageIndex = Player.Temp.PokemonStatusPageIndex;
    }

    public override void Update()
    {
        if (_attackToggle == false)
        {
            if (_attackPos > 0.0f)
            {
                _attackPos -= 15.0f;
                if (_attackPos <= 0.0f)
                    _attackPos = 0.0f;
            }

            int dummyPokeIndex = _pokeIndex;

            if (Controls.Down(true, false, false) == true)
            {
                _pokeIndex += 1;
                _frontView = true;
                _attackIndex = 0;
            }
            if (Controls.Up(true, false, false) == true)
            {
                _pokeIndex -= 1;
                _frontView = true;
                _attackIndex = 0;
            }

            if (_viewParty == true)
            {
                if (_pokeIndex < 0)
                    _pokeIndex = 0;
                else if (_pokeIndex > Core.Player.Pokemons.Count - 1)
                    _pokeIndex = Core.Player.Pokemons.Count - 1;

                _pokemon = Core.Player.Pokemons[_pokeIndex];
            }
            else
            {
                if (_pokeIndex < 0)
                    _pokeIndex = 0;
                else if (_pokeIndex > _boxPokemon.Length - 1)
                    _pokeIndex = _boxPokemon.Length - 1;

                _pokemon = _boxPokemon[_pokeIndex];
            }

            if (dummyPokeIndex != _pokeIndex)
            {
                if (_pokemon.EggSteps == 0)
                    _pokemon.PlayCry();
            }
        }
        else
        {
            if (_attackPos < 340.0f)
            {
                _attackPos += 15.0f;
                if (_attackPos >= 340.0f)
                    _attackPos = 340.0f;
            }

            if (Controls.Down(true, false, true) == true)
                _attackIndex += 1;
            if (Controls.Up(true, false, true) == true)
                _attackIndex -= 1;

            if (_attackIndex < 0)
                _attackIndex = 0;
            else if (_attackIndex > _pokemon.attacks.Count - 1)
                _attackIndex = _pokemon.attacks.Count - 1;
        }

        if (_switchIndex == -1)
        {
            if (Controls.Right(true, false, true) == true)
            {
                _pageIndex += 1;
                _attackToggle = false;
                _attackIndex = 0;
            }
            if (Controls.Left(true, false, true) == true)
            {
                _pageIndex -= 1;
                _attackToggle = false;
                _attackIndex = 0;
            }
            if (_pageIndex < 0) _pageIndex = 0;
            else if (_pageIndex > 2) _pageIndex = 2;
        }

        Player.Temp.PokemonStatusPageIndex = _pageIndex;
        Player.Temp.PokemonScreenIndex = _pokeIndex;

        if (_pageIndex == 0)
        {
            if (Controls.Accept() == true)
                _frontView = !_frontView;
        }
        else if (_pageIndex == 2)
        {
            if (Controls.Accept() == true && _pokemon.EggSteps == 0)
            {
                if (_attackToggle == false)
                {
                    _attackToggle = true;
                }
                else
                {
                    if (_switchIndex == -1)
                    {
                        _switchIndex = _attackIndex;
                    }
                    else
                    {
                        BattleSystem.Attack a1 = _pokemon.attacks[_switchIndex];
                        BattleSystem.Attack a2 = _pokemon.attacks[_attackIndex];

                        _pokemon.attacks[_attackIndex] = a1;
                        _pokemon.attacks[_switchIndex] = a2;

                        _switchIndex = -1;
                    }
                }
            }
        }

        if (Controls.Dismiss() == true)
        {
            if (_attackToggle == true)
            {
                if (_switchIndex != -1)
                    _switchIndex = -1;
                else
                    _attackToggle = false;
            }
            else
            {
                Core.SetScreen(PreScreen!);
            }
        }
    }

    public override void Draw()
    {
        PreScreen?.Draw();

        Canvas.DrawImageBorder(TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty), 2, new Rectangle(60, 100, 800, 480));
        DrawHeader();

        Vector2 texturePositionPage;

        switch (_pageIndex)
        {
            case 0:
                if (_pokemon.EggSteps == 0)
                    DrawPage1();
                Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_stats_page"), new Vector2(676, 138), Color.Black);
                texturePositionPage = new Vector2(32, 96);
                break;
            case 1:
                DrawPage2();
                Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_details_page"), new Vector2(676, 138), Color.Black);
                texturePositionPage = new Vector2(32, 112);
                break;
            case 2:
                if (_pokemon.EggSteps == 0)
                    DrawPage3();
                Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_moves_page"), new Vector2(676, 138), Color.Black);
                texturePositionPage = new Vector2(80, 96);
                break;
            default:
                texturePositionPage = new Vector2(32, 96);
                break;
        }

        Core.SpriteBatch.Draw(_mainTexture, new Rectangle(574, 132, 96, 32), new Rectangle((int)texturePositionPage.X, (int)texturePositionPage.Y, 48, 16), Color.White);

        if (_attackToggle == false)
            Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_backadvice"), new Vector2(1200 - FontManager.MiniFont.MeasureString(Localization.GetString("poke_status_screen_backadvice")).X - 360, 580), Color.DarkGray);
        else
            Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_closeadvice"), new Vector2(1200 - FontManager.MiniFont.MeasureString(Localization.GetString("poke_status_screen_closeadvice")).X - 360, 580), Color.DarkGray);
    }

    private void DrawHeader()
    {
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        int allEVs = _pokemon.EVHP + _pokemon.EVAttack + _pokemon.EVDefense + _pokemon.EVSpAttack + _pokemon.EVSpDefense + _pokemon.EVSpeed;
        if (allEVs > 0)
        {
            int evMax = 510;
            if (allEVs > evMax)
                evMax = allEVs;

            double blockWidth = 490.0 / evMax;
            int currentWidth = 70;
            int blockY = 192;

            Canvas.DrawRectangle(new Rectangle(currentWidth, blockY, (int)(blockWidth * _pokemon.EVHP), 10), _evColors[0]);
            currentWidth += (int)(blockWidth * _pokemon.EVHP);

            Canvas.DrawRectangle(new Rectangle(currentWidth, blockY, (int)(blockWidth * _pokemon.EVAttack), 10), _evColors[1]);
            currentWidth += (int)(blockWidth * _pokemon.EVAttack);

            Canvas.DrawRectangle(new Rectangle(currentWidth, blockY, (int)(blockWidth * _pokemon.EVDefense), 10), _evColors[2]);
            currentWidth += (int)(blockWidth * _pokemon.EVDefense);

            Canvas.DrawRectangle(new Rectangle(currentWidth, blockY, (int)(blockWidth * _pokemon.EVSpAttack), 10), _evColors[3]);
            currentWidth += (int)(blockWidth * _pokemon.EVSpAttack);

            Canvas.DrawRectangle(new Rectangle(currentWidth, blockY, (int)(blockWidth * _pokemon.EVSpDefense), 10), _evColors[4]);
            currentWidth += (int)(blockWidth * _pokemon.EVSpDefense);

            Canvas.DrawRectangle(new Rectangle(currentWidth, blockY, (int)(blockWidth * _pokemon.EVSpeed), 10), _evColors[5]);
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(60, 100, 480, 64));
        if (_pokemon.GetDisplayName() == _pokemon.GetName() || _pokemon.IsEgg == true)
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _pokemon.GetDisplayName(), new Vector2(158, 132), Color.Black);
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.InGameFont, _pokemon.GetDisplayName(), new Vector2(158, 122), Color.Black);
            Core.SpriteBatch.DrawString(FontManager.MiniFont, _pokemon.GetName(), new Vector2(164, 152), Color.Black);
        }
        Core.SpriteBatch.Draw(_pokemon.GetMenuTexture(), new Rectangle(70, 110, 80, 80), BattleStats.GetStatColor(_pokemon.Status));
        if (_pokemon.Item != null && _pokemon.EggSteps == 0)
            Core.SpriteBatch.Draw(_pokemon.Item.Texture, new Rectangle(118, 150, 28, 28), Color.White);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(60, 196, 128, 128));
        Core.SpriteBatch.Draw(_pokemon.GetTexture(_frontView), new Rectangle(74, 208, 124, 124), Color.White);
        Core.SpriteBatch.Draw(_pokemon.catchBall!.Texture, new Rectangle(74, 318, 24, 24), Color.White);
        if (_pokemon.IsShiny == true && _pokemon.IsEgg == false)
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle(78, 218, 18, 18), new Rectangle(118, 4, 9, 9), Color.White);

        if (_pokemon.EggSteps == 0)
        {
            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(60, 356, 128, 224));
            Core.SpriteBatch.Draw(TextureManager.GetTexture(@"GUI\Menus\Types"), new Rectangle(76, 380, 48, 16), _pokemon.Type1.GetElementImage(), Color.White);
            if (_pokemon.Type2.Type != Element.Types.Blank)
                Core.SpriteBatch.Draw(TextureManager.GetTexture(@"GUI\Menus\Types"), new Rectangle(124, 380, 48, 16), _pokemon.Type2.GetElementImage(), Color.White);

            Rectangle r = new Rectangle(96, 0, 6, 10);
            if (_pokemon.Gender == Pokemon.Genders.Female)
                r = new Rectangle(102, 0, 6, 10);
            if (_pokemon.Gender != Pokemon.Genders.Genderless)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle(180, 376, 12, 20), r, Color.White);

            Core.SpriteBatch.DrawString(FontManager.MiniFont,
                Localization.GetString("Level") + ": " + _pokemon.Level + Environment.NewLine +
                Localization.GetString("poke_status_screen_number") + _pokemon.Number + Environment.NewLine + Environment.NewLine +
                Localization.GetString("poke_status_screen_nature") + ":" + Environment.NewLine +
                _pokemon.Nature.ToString(),
                new Vector2(76, 410), Color.Black);

            Texture2D? statusTexture = BattleStats.GetStatImage(_pokemon.Status);
            if (statusTexture != null)
            {
                int y = 139;
                if (_pokemon.GetDisplayName() != _pokemon.GetName())
                    y = 127;
                Canvas.DrawRectangle(new Rectangle((int)(170 + FontManager.InGameFont.MeasureString(_pokemon.GetDisplayName()).X), y, 61, 22), Color.Gray);
                Core.SpriteBatch.Draw(statusTexture, new Rectangle((int)(172 + FontManager.InGameFont.MeasureString(_pokemon.GetDisplayName()).X), y + 2, 57, 18), Color.White);
            }
        }
    }

    private void DrawPage1()
    {
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Vector2 p = new Vector2(140, 180);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(220, 196, 320, 256));
        int barX = (int)((float)_pokemon.HP / _pokemon.MaxHP * 150);
        int barPercentage = ((int)((float)_pokemon.HP / _pokemon.MaxHP * 100)).Clamp(0, 100);

        Rectangle barRectangle = new Rectangle(115, 0, 1, 4);
        if (barPercentage >= 50)
            barRectangle = new Rectangle(113, 0, 1, 4);
        else if (barPercentage < 50 && barPercentage > 10)
            barRectangle = new Rectangle(116, 0, 1, 4);

        for (int x = 0; x <= barX - 1; x++)
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + x * 2 + 104), (int)(p.Y + 44), 4, 16), barRectangle, Color.White);
        for (int x = barX; x <= 149; x++)
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + x * 2 + 104), (int)(p.Y + 44), 4, 16), new Rectangle(114, 0, 1, 4), Color.White);
        Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 100), (int)(p.Y + 44), 4, 16), new Rectangle(112, 0, 1, 4), Color.White);
        Core.SpriteBatch.Draw(_mainTexture, new Rectangle((int)(p.X + 406), (int)(p.Y + 44), 4, 16), new Rectangle(112, 0, 1, 4), Color.White);

        String redText = Environment.NewLine + Environment.NewLine;
        String blueText = Environment.NewLine + Environment.NewLine;
        String blackText = Localization.GetString("HP") + Environment.NewLine + Environment.NewLine;
        for (int i = 0; i <= 4; i++)
        {
            String statText = String.Empty;
            String stat = String.Empty;
            switch (i)
            {
                case 0: statText = Localization.GetString("Attack"); stat = "Attack"; break;
                case 1: statText = Localization.GetString("Defense"); stat = "Defense"; break;
                case 2: statText = Localization.GetString("Special_Attack"); stat = "SpAttack"; break;
                case 3: statText = Localization.GetString("Special_Defense"); stat = "SpDefense"; break;
                case 4: statText = Localization.GetString("Speed"); stat = "Speed"; break;
            }

            float m = Nature.GetMultiplier(_pokemon.Nature, stat);
            if (m > 1.0f)
            {
                redText += statText + Environment.NewLine;
                blueText += Environment.NewLine + Environment.NewLine;
                blackText += Environment.NewLine + Environment.NewLine;
            }
            else if (m < 1.0f)
            {
                redText += Environment.NewLine + Environment.NewLine;
                blueText += statText + Environment.NewLine;
                blackText += Environment.NewLine + Environment.NewLine;
            }
            else
            {
                redText += Environment.NewLine + Environment.NewLine;
                blueText += Environment.NewLine + Environment.NewLine;
                blackText += statText + Environment.NewLine + Environment.NewLine;
            }
        }

        Core.SpriteBatch.DrawString(FontManager.MiniFont, blackText, new Vector2((int)(p.X + 100), (int)(p.Y + 68)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MiniFont, redText, new Vector2((int)(p.X + 100), (int)(p.Y + 68)), new Color(255, 0, 0, 200));
        Core.SpriteBatch.DrawString(FontManager.MiniFont, blueText, new Vector2((int)(p.X + 100), (int)(p.Y + 68)), Color.Blue);
        Core.SpriteBatch.DrawString(FontManager.MiniFont,
            _pokemon.HP + " / " + _pokemon.MaxHP + Environment.NewLine + Environment.NewLine +
            _pokemon.Attack + Environment.NewLine + Environment.NewLine +
            _pokemon.Defense + Environment.NewLine + Environment.NewLine +
            _pokemon.SpAttack + Environment.NewLine + Environment.NewLine +
            _pokemon.SpDefense + Environment.NewLine + Environment.NewLine +
            _pokemon.Speed,
            new Vector2((int)(p.X + 280), (int)(p.Y + 68)), Color.Black);

        if (_pokemon.Level < int.Parse(GameModeManager.GetGameRuleValue("MaxLevel", "100")))
        {
            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(220, 484, 320, 96));

            int nextLvExp = _pokemon.NeedExperience(_pokemon.Level + 1) - _pokemon.NeedExperience(_pokemon.Level);
            int currentExp = _pokemon.Experience - _pokemon.NeedExperience(_pokemon.Level);
            if (_pokemon.Level == 1)
            {
                nextLvExp = _pokemon.NeedExperience(_pokemon.Level + 1);
                currentExp = _pokemon.Experience;
            }

            if (_pokemon.Level == int.Parse(GameModeManager.GetGameRuleValue("MaxLevel", "100")))
            {
                nextLvExp = 0;
            }
            else
            {
                barPercentage = (int)((float)currentExp / nextLvExp * 100);
                barX = ((int)((float)currentExp / nextLvExp * 150)).Clamp(0, 150);
            }

            Core.SpriteBatch.DrawString(FontManager.MiniFont,
                Localization.GetString("poke_status_screen_all_exp") + ": " + _pokemon.Experience + Environment.NewLine +
                Localization.GetString("poke_status_screen_nxt_lv") + ": " + (nextLvExp - currentExp),
                new Vector2(240, 504), Color.Black);

            int altI = 0;
            for (int x = 0; x <= barX - 1; x++)
            {
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle(x * 2 + 240, 550, 4, 16), new Rectangle(118 + altI, 0, 1, 4), Color.White);
                altI += 1;
                if (altI == 2) altI = 0;
            }
            for (int x = barX; x <= 149; x++)
                Core.SpriteBatch.Draw(_mainTexture, new Rectangle(x * 2 + 240, 550, 4, 16), new Rectangle(114, 0, 1, 4), Color.White);
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle(236, 550, 4, 16), new Rectangle(112, 0, 1, 4), Color.White);
            Core.SpriteBatch.Draw(_mainTexture, new Rectangle(542, 550, 4, 16), new Rectangle(112, 0, 1, 4), Color.White);

            if (barPercentage == 100)
                barPercentage -= 1;

            Core.SpriteBatch.DrawString(FontManager.MiniFont, barPercentage + " %", new Vector2(250, 575), Color.DarkBlue);
        }
    }

    private void DrawPage2()
    {
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(220, 196, 320, 96));
        Core.SpriteBatch.DrawString(FontManager.MiniFont,
            Localization.GetString("poke_status_screen_OT") + ": " + _pokemon.OT + " /" + _pokemon.CatchTrainerName + Environment.NewLine + Environment.NewLine +
            _pokemon.CatchMethod + Environment.NewLine + _pokemon.CatchLocation,
            new Vector2(238, 214), Color.DarkBlue);

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(220, 324, 320, 128));
        if (_pokemon.EggSteps == 0)
        {
            if (_pokemon.Item != null)
            {
                Core.SpriteBatch.Draw(_pokemon.Item.Texture, new Rectangle(232, 338, 24, 24), Color.White);
                Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_Item") + ": " + _pokemon.Item.Name, new Vector2(262, 342), Color.Black);
                Core.SpriteBatch.DrawString(FontManager.MiniFont, _pokemon.Item.Description.CropStringToWidth(FontManager.MiniFont, 300), new Vector2(234, 360), Color.Black);
            }
            else
            {
                Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("poke_status_screen_Item") + ": " + Localization.GetString("poke_status_screen_no_item"), new Vector2(262, 342), Color.Black);
            }
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(220, 484, 320, 96));
        if (_pokemon.EggSteps == 0)
        {
            Core.SpriteBatch.DrawString(FontManager.MiniFont,
                Localization.GetString("poke_status_screen_ability") + ": " + _pokemon.Ability!.Name + Environment.NewLine + Environment.NewLine +
                _pokemon.Ability.Description.CropStringToWidth(FontManager.MiniFont, 300),
                new Vector2(234, 500), Color.Black);
        }
        else
        {
            String s = "\"The Egg Watch\"" + Environment.NewLine;
            int percent = (int)((float)_pokemon.EggSteps / _pokemon.BaseEggSteps * 100);
            if (percent <= 33)
                s += "It looks like this Egg will" + Environment.NewLine + "take a long time to hatch.";
            else if (percent > 33 && percent <= 66)
                s += "It's getting warmer and moves" + Environment.NewLine + "a little. It will hatch soon.";
            else
                s += "There is strong movement" + Environment.NewLine + "noticeable. It will hatch soon!";
            Core.SpriteBatch.DrawString(FontManager.MiniFont, s, new Vector2(234, 500), Color.Black);
        }
    }

    private void DrawPage3()
    {
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);

        if (_pokemon.attacks.Count > 0)
        {
            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(572 - 352 + _attackPos), 196, 288, 384));

            BattleSystem.Attack a = _pokemon.attacks[_attackIndex];

            String fullText = a.Description.Replace("’", "'");
            String t = String.Empty;
            String n = String.Empty;
            for (int i = 0; i < fullText.Length; i++)
            {
                char c = fullText[i];
                if (c == ' ')
                {
                    if (FontManager.MiniFont.MeasureString(n + c).X > 170)
                    {
                        t += Environment.NewLine;
                        n = String.Empty;
                    }
                    else
                    {
                        t += " ";
                        n += " ";
                    }
                }
                else
                {
                    t += c;
                    n += c;
                }
            }

            String power = a.Power.ToString();
            if (power == "0") power = "-";

            String acc = a.Accuracy.ToString();
            if (acc == "0") acc = "-";

            Core.SpriteBatch.DrawString(FontManager.MiniFont,
                Localization.GetString("poke_status_screen_power") + ": " + power + Environment.NewLine +
                Localization.GetString("poke_status_screen_accuracy") + ": " + acc + Environment.NewLine + Environment.NewLine + t,
                new Vector2((int)(552 - 300 + _attackPos), 218), Color.Black);
            Core.SpriteBatch.Draw(a.GetDamageCategoryImage(), new Rectangle((int)(552 - 150 + _attackPos), 222, 56, 28), Color.White);

            Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(220, 196, 320, 384));

            for (int i = 0; i <= _pokemon.attacks.Count - 1; i++)
                DrawAttack(i, _pokemon.attacks[i]);
        }
    }

    private void DrawAttack(int i, BattleSystem.Attack a)
    {
        Vector2 p = new Vector2(240, 210 + i * (64 + 32));

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        if (_attackToggle == true && _attackIndex == i)
        {
            canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(48, 0, 48, 48), String.Empty);
        }
        else
        {
            if (_switchIndex != -1 && i == _switchIndex)
                canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(252, (int)p.Y, 256, 64));

        Core.SpriteBatch.DrawString(FontManager.MiniFont, a.Name, new Vector2(270, (int)(p.Y + 26)), Color.Black);

        Color c = Color.Black;
        int per = (int)((float)a.CurrentPP / a.MaxPP * 100);
        if (per <= 33 && per > 10) c = Color.Orange;
        else if (per <= 10) c = Color.IndianRed;

        Core.SpriteBatch.DrawString(FontManager.MiniFont, Localization.GetString("PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(400, (int)(p.Y + 58)), c);
        Core.SpriteBatch.Draw(TextureManager.GetTexture(@"GUI\Menus\Types", a.Type.GetElementImage(), String.Empty), new Rectangle(270, (int)(p.Y + 54), 48, 16), Color.White);
    }

    public override void ChangeTo()
    {
        if (_pokemon.EggSteps == 0)
            _pokemon.PlayCry();
    }
}
