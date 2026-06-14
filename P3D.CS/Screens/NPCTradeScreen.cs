using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class NPCTradeScreen : Screen
{
    private Pokemon _playerPokemon = null!;
    private Pokemon _npcPokemon = null!;
    private String _npcTrainerName = "Trainer";
    private String _textMessage = String.Empty;
    private int _ownPokemonPosition = 0;
    private int _oppPokemonPosition = 0;
    private int _tState = 0;
    private int _messageDelay = 220;

    public NPCTradeScreen(Screen currentScreen, Pokemon playerPokemon, Pokemon npcPokemon,
                          String trainerName, String afterTradeMessage = "")
    {
        Identification = Identifications.DirectTradeScreen;
        PreScreen = currentScreen;

        _ownPokemonPosition = Core.windowSize.Height;
        _tState = 0;
        _messageDelay = 220;
        CanChat = false;
        MouseVisible = false;
        CanBePaused = false;

        _playerPokemon = playerPokemon;
        _npcPokemon = npcPokemon;
        _npcTrainerName = trainerName;
        _textMessage = afterTradeMessage;

        MusicManager.Play("evolution", true);
    }

    public override void Draw()
    {
        Texture2D background = TextureManager.GetTexture("GUI\\Menus\\GTSBackground");

        Size backSize = new Size(Core.windowSize.Width, Core.windowSize.Height);
        Size origSize = new Size(background.Width, background.Height);
        float aspectRatio = (float)origSize.Width / origSize.Height;

        backSize.Width = (int)(Core.windowSize.Width * aspectRatio);
        backSize.Height = (int)(backSize.Width / aspectRatio);

        if (backSize.Width > backSize.Height)
        {
            backSize.Width = Core.windowSize.Width;
            backSize.Height = (int)(Core.windowSize.Width / aspectRatio);
        }
        else
        {
            backSize.Height = Core.windowSize.Height;
            backSize.Width = (int)(Core.windowSize.Height / aspectRatio);
        }
        if (backSize.Height < Core.windowSize.Height)
        {
            backSize.Height = Core.windowSize.Height;
            backSize.Width = (int)((float)Core.windowSize.Height / origSize.Height * origSize.Width);
        }

        int xOffset = 0;
        if (Core.windowSize.Width < backSize.Width)
        {
            float xAspectRatio = (float)origSize.Width / backSize.Width;
            xOffset = (int)(Math.Floor((backSize.Width - Core.windowSize.Width) * xAspectRatio) / 2);
        }

        Core.SpriteBatch.Draw(background, new Rectangle(0, 0, backSize.Width, backSize.Height), new Rectangle(xOffset, 0, origSize.Width, origSize.Height), Color.White);

        if (TextBox.Showing == false)
        {
            switch (_tState)
            {
                case 0:
                {
                    Texture2D tex = _playerPokemon.GetTexture(true);
                    Core.SpriteBatch.Draw(tex, new Rectangle(Core.windowSize.Width / 2 - Math.Min((int)(tex.Width * 3 / 2), 144), _ownPokemonPosition, Math.Min(tex.Width * 3, 288), Math.Min(tex.Height * 3, 288)), Color.White);
                    break;
                }
                case 1:
                {
                    Texture2D tex = _playerPokemon.GetTexture(false);
                    Core.SpriteBatch.Draw(tex, new Rectangle(Core.windowSize.Width / 2 - Math.Min((int)(tex.Width * 3 / 2), 144), Core.windowSize.Height / 2 - 128, Math.Min(tex.Width * 3, 288), Math.Min(tex.Height * 3, 288)), Color.White);
                    String t = Localization.GetString("trade_screen_trade_SendingPokemon", "Sending [YOURPOKEMON] to [OTHERPLAYER].~Good-bye, [YOURPOKEMON]!").Replace("[YOURPOKEMON]", _playerPokemon.GetDisplayName()).Replace("[OTHERPLAYER]", _npcTrainerName).Replace("~", Environment.NewLine).Replace("*", Environment.NewLine);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.MainFont.MeasureString(t).X / 2) + 2, Core.windowSize.Height / 2 + 194), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.MainFont.MeasureString(t).X / 2), Core.windowSize.Height / 2 + 192), Color.White);
                    break;
                }
                case 2:
                {
                    Texture2D tex = _playerPokemon.GetTexture(false);
                    Core.SpriteBatch.Draw(tex, new Rectangle(Core.windowSize.Width / 2 - Math.Min((int)(tex.Width * 3 / 2), 144), _ownPokemonPosition, Math.Min(tex.Width * 3, 288), Math.Min(tex.Height * 3, 288)), Color.White);
                    break;
                }
                case 3:
                {
                    Texture2D tex = _npcPokemon.GetTexture(true);
                    Core.SpriteBatch.Draw(tex, new Rectangle(Core.windowSize.Width / 2 - Math.Min((int)(tex.Width * 3 / 2), 144), _oppPokemonPosition, Math.Min(tex.Width * 3, 288), Math.Min(tex.Height * 3, 288)), Color.White);
                    break;
                }
                case 4:
                {
                    Texture2D tex = _npcPokemon.GetTexture(true);
                    Core.SpriteBatch.Draw(tex, new Rectangle(Core.windowSize.Width / 2 - Math.Min((int)(tex.Width * 3 / 2), 144), Core.windowSize.Height / 2 - 128, Math.Min(tex.Width * 3, 288), Math.Min(tex.Height * 3, 288)), Color.White);
                    String t = Localization.GetString("trade_screen_trade_ReceivedPokemon", "[OTHERPLAYER] sent over [THEIRPOKEMON].").Replace("[OTHERPLAYER]", _npcTrainerName).Replace("[THEIRPOKEMON]", _npcPokemon.GetDisplayName());
                    Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.MainFont.MeasureString(t).X / 2) + 2, Core.windowSize.Height / 2 + 194), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.MainFont.MeasureString(t).X / 2), Core.windowSize.Height / 2 + 192), Color.White);
                    break;
                }
            }
        }
        TextBox.Draw();
    }

    public override void Update()
    {
        TextBox.Update();
        if (TextBox.Showing == false)
        {
            switch (_tState)
            {
                case 0:
                    if (_ownPokemonPosition > Core.windowSize.Height / 2 - 128)
                    {
                        _ownPokemonPosition -= 4;
                        if (_ownPokemonPosition <= Core.windowSize.Height / 2 - 128)
                        {
                            _ownPokemonPosition = Core.windowSize.Height / 2 - 128;
                            _tState = 1;
                            SoundManager.PlayPokemonCry(_playerPokemon.Number, PokemonForms.GetCrySuffix(_playerPokemon));
                        }
                    }
                    break;
                case 1:
                    if (_messageDelay > 0)
                    {
                        _messageDelay -= 1;
                        if (_messageDelay <= 0) { _messageDelay = 220; _tState = 2; }
                    }
                    break;
                case 2:
                    if (_ownPokemonPosition > -288)
                    {
                        _ownPokemonPosition -= 4;
                        if (_ownPokemonPosition <= -288) { _ownPokemonPosition = -288; _tState = 3; _oppPokemonPosition = -288; }
                    }
                    break;
                case 3:
                    if (_oppPokemonPosition < Core.windowSize.Height / 2 - 128)
                    {
                        _oppPokemonPosition += 4;
                        if (_oppPokemonPosition >= Core.windowSize.Height / 2 - 128)
                        {
                            _oppPokemonPosition = Core.windowSize.Height / 2 - 128;
                            _tState = 4;
                            SoundManager.PlayPokemonCry(_npcPokemon.Number, PokemonForms.GetCrySuffix(_npcPokemon));
                        }
                    }
                    break;
                case 4:
                    if (_messageDelay > 0)
                    {
                        _messageDelay -= 1;
                        if (_messageDelay == 180) SoundManager.PlaySound("success", true);
                        if (_messageDelay <= 0) { _messageDelay = 220; EndTrade(); }
                    }
                    break;
                case 5:
                    if (_npcPokemon.Number != Core.Player.Pokemons[Core.Player.Pokemons.Count - 1].Number || _npcPokemon.AdditionalData != Core.Player.Pokemons[Core.Player.Pokemons.Count - 1].AdditionalData)
                        _npcPokemon = Core.Player.Pokemons[Core.Player.Pokemons.Count - 1];
                    if (Core.CurrentScreen.Identification == Identifications.DirectTradeScreen)
                        EndTradeAfterEvolve();
                    break;
            }
        }
    }

    private void EndTrade()
    {
        String text = _textMessage;
        if (text == String.Empty)
            text = Localization.GetString("trade_screen_trade_TradedWithNPC", "<Player.Name> traded~[THEIRPOKEMON] for~[YOURPOKEMON]!").Replace("[THEIRPOKEMON]", _npcPokemon.GetName()).Replace("[YOURPOKEMON]", _playerPokemon.GetName());

        String s = "version=2" + Environment.NewLine +
                   "@sound.play(success_small)" + Environment.NewLine +
                   "@text.show(" + text + ")" + Environment.NewLine +
                   ":end";

        Pokemon p = Core.Player.Pokemons[Core.Player.Pokemons.Count - 1];
        if (p.CanEvolve(EvolutionCondition.EvolutionTrigger.Trading, PokemonForms.GetPokemonDataFileName(_npcPokemon.Number, _npcPokemon.AdditionalData)) == true)
        {
            Core.SetScreen(new EvolutionScreen(this, new List<int> { Core.Player.Pokemons.Count - 1 }, PokemonForms.GetPokemonDataFileName(_npcPokemon.Number, _npcPokemon.AdditionalData), EvolutionCondition.EvolutionTrigger.Trading));
            _tState = 5;
        }
        else
        {
            ((OverworldScreen)PreScreen!).ActionScript.StartScript(s, 2, false);
            Core.SetScreen(new TransitionScreen(Core.CurrentScreen, PreScreen!, Color.Black, false, 15));
        }
    }

    private void EndTradeAfterEvolve()
    {
        String text = _textMessage;
        if (text == String.Empty)
            text = Localization.GetString("trade_screen_trade_TradedWithNPC", "<Player.Name> traded~[THEIRPOKEMON] for~[YOURPOKEMON]!").Replace("[THEIRPOKEMON]", _npcPokemon.GetName()).Replace("[YOURPOKEMON]", _playerPokemon.GetName());

        String s = "version=2" + Environment.NewLine +
                   "@sound.play(success_small)" + Environment.NewLine +
                   "@text.show(" + text + ")" + Environment.NewLine +
                   ":end";

        ((OverworldScreen)PreScreen!).ActionScript.StartScript(s, 2, false);
        Core.SetScreen(new TransitionScreen(Core.CurrentScreen, PreScreen!, Color.Black, false, 20));
    }
}
