using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class LearnAttackScreen : Screen
{
    private Pokemon _pokemon;
    private List<BattleSystem.Attack> _newAttacks;
    private Texture2D _mainTexture;

    private bool _chosen = false;
    private int _index = 0;

    private int _attackIndex = 0;
    private float _attackPos = 320.0f;

    private bool _canForget = true;
    private String _machineItemID = "-1";

    private int _currentCharIndex = 0;

    public LearnAttackScreen(Screen currentScreen, Pokemon pokemon, List<BattleSystem.Attack> newAttacks)
        : this(currentScreen, pokemon, newAttacks, "-1") { }

    public LearnAttackScreen(Screen currentScreen, Pokemon pokemon, BattleSystem.Attack newAttack)
        : this(currentScreen, pokemon, new List<BattleSystem.Attack> { newAttack }, "-1") { }

    public LearnAttackScreen(Screen currentScreen, Pokemon pokemon, BattleSystem.Attack newAttack, String machineItemID)
        : this(currentScreen, pokemon, new List<BattleSystem.Attack> { newAttack }, machineItemID) { }

    public LearnAttackScreen(Screen currentScreen, Pokemon pokemon, List<BattleSystem.Attack> newAttacks, String machineItemID)
    {
        Identification = Identifications.LearnAttackScreen;

        PreScreen = currentScreen;
        _pokemon = pokemon;
        _newAttacks = newAttacks;
        _machineItemID = machineItemID;

        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
    }

    public override void Update()
    {
        if (TextBox.Showing == false)
        {
            if (_currentCharIndex < GetText().Length)
            {
                _currentCharIndex += 1;
                return;
            }

            if (_chosen == false)
            {
                Core.GameInstance.IsMouseVisible = false;
                if (Controls.Up(true, true, true, true) == true)
                    _attackIndex -= 1;
                if (Controls.Down(true, true, true, true) == true)
                    _attackIndex += 1;

                _attackIndex = (int)MathHelper.Clamp(_attackIndex, 0, 4);

                if (_attackIndex < 4)
                {
                    if (bool.Parse(GameModeManager.GetGameRuleValue("CanForgetHM", "0")) == true)
                    {
                        _canForget = true;
                    }
                    else
                    {
                        _canForget = _pokemon.attacks[_attackIndex].isHMMove == false;
                    }
                }
                else
                {
                    _canForget = true;
                }

                if (Controls.Dismiss() == true)
                {
                    _attackIndex = 4;
                    _chosen = true;
                }

                if (Controls.Accept() == true)
                    _chosen = true;
            }
            else
            {
                Core.GameInstance.IsMouseVisible = true;

                if (Controls.Right(true, true, true) == true && _canForget == true)
                    _index = 1;
                if (Controls.Left(true, true, true) == true)
                    _index = 0;

                bool accepted = false;
                for (int i = 0; i <= 1; i++)
                {
                    if (new Rectangle(Core.windowSize.Width / 2 - 182 + i * 192, 550, 128 + 32, 64 + 32).Contains(MouseHandler.MousePosition) == true)
                    {
                        _index = i;

                        if (MouseHandler.ButtonPressed(MouseHandler.MouseButtons.LeftButton) == true)
                        {
                            switch (_index)
                            {
                                case 0:
                                    accepted = true;
                                    ClickNo();
                                    break;
                                case 1:
                                    accepted = true;
                                    ClickYes();
                                    break;
                            }
                        }
                    }
                }

                if (Controls.Accept(false, true) == true || (accepted == false && Controls.Accept(true, false, false) == true))
                {
                    switch (_index)
                    {
                        case 0:
                            ClickNo();
                            break;
                        case 1:
                            ClickYes();
                            break;
                    }
                }
                if (Controls.Dismiss() == true)
                    ClickNo();
            }
        }
        else
        {
            TextBox.Update();
        }
    }

    public override void Draw()
    {
        PreScreen?.Draw();
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0, 150));
        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        DrawText();

        if (_currentCharIndex < GetText().Length)
        {
            if (TextBox.Showing == true)
                TextBox.Draw();
        }
        else
        {
            Vector2 p = new Vector2(96, 96);

            if (_pokemon.attacks.Count > 0)
            {
                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X, (int)p.Y, 672, 416));

                BattleSystem.Attack a;
                if (_attackIndex == 4)
                    a = _newAttacks[0];
                else
                    a = _pokemon.attacks[_attackIndex];

                String description = a.Description.Replace("'", "'").CropStringToWidth(FontManager.MainFont, 240);

                String power = a.Power.ToString();
                if (power == "0") power = "-";

                String acc = a.Accuracy.ToString();
                if (acc == "0") acc = "-";

                Core.SpriteBatch.DrawString(FontManager.MainFont,
                    Localization.GetString("property_Power", "Power") + ": " + power + Environment.NewLine +
                    Localization.GetString("property_Accuracy", "Accuracy") + ": " + acc + Environment.NewLine + Environment.NewLine + description,
                    new Vector2((int)(p.X + 352 + 48), p.Y + 48), Color.Black);
                Core.SpriteBatch.Draw(a.GetDamageCategoryImage(),
                    new Rectangle((int)(p.X + 672 - 16 - 56), (int)(p.Y + 44), 56, 28), Color.White);

                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X, (int)p.Y, 352, 416));
                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X, (int)(p.Y + 416 + 48), 352, 128));

                for (int i = 0; i <= _pokemon.attacks.Count - 1; i++)
                    DrawAttack(p, i, _pokemon.attacks[i]);
                DrawAttack(p, 4, _newAttacks[0]);
            }

            if (_chosen == true)
            {
                Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0, 150));

                Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(Core.windowSize.Width / 2 - 352, 172, 704, 96));

                String drawText = String.Empty;
                if (_attackIndex == 4)
                {
                    drawText = Localization.GetString("learn_move_GiveUpOnLearning", "Give up on learning \"[MOVENAME]\"").Replace("[MOVENAME]", _newAttacks[0].Name);
                }
                else
                {
                    drawText = Localization.GetString("learn_move_ForgetMoveToLearn", "Forget \"[OLDMOVENAME]\" to learn \"[NEWMOVENAME]\"?").Replace("[OLDMOVENAME]", _pokemon.attacks[_attackIndex].Name).Replace("[NEWMOVENAME]", _newAttacks[0].Name);
                    if (_canForget == false)
                        drawText = Localization.GetString("learn_move_CannotForgetMove", "Cannot forget the move \"[MOVENAME]\" because~it's a Hidden Machine move.").Replace("[MOVENAME]", _pokemon.attacks[_attackIndex].Name).Replace("~", Environment.NewLine).Replace("*", Environment.NewLine);
                }

                Core.SpriteBatch.DrawString(FontManager.InGameFont, drawText,
                    new Vector2(Core.windowSize.Width / 2 - (int)(FontManager.InGameFont.MeasureString(drawText).X / 2), 200), Color.Black);

                int endIndex = 1;
                if (_canForget == false)
                    endIndex = 0;

                for (int i = 0; i <= endIndex; i++)
                {
                    Color fontColor = Color.Black;
                    String text = Localization.GetString("global_learn", "Learn");
                    if (_attackIndex == 4)
                        text = Localization.GetString("global_ok", "OK");
                    if (i == 0)
                        text = Localization.GetString("global_cancel", "Cancel");

                    if (i == _index)
                    {
                        canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
                        fontColor = Color.White;
                    }
                    else
                    {
                        canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
                    }

                    Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle(Core.windowSize.Width / 2 - 182 + i * 192 + 22, 550, 128, 64));

                    if (fontColor != Color.Black)
                        Core.SpriteBatch.DrawString(FontManager.InGameFont, text, new Vector2(Core.windowSize.Width / 2 - 164 + i * 192 + 22 + 2, 404 + 180 + 2), Color.Black);
                    Core.SpriteBatch.DrawString(FontManager.InGameFont, text, new Vector2(Core.windowSize.Width / 2 - 164 + i * 192 + 22, 404 + 180), fontColor);
                }
            }

            TextBox.Draw();
        }
    }

    private void DrawText()
    {
        String namePrefix = _pokemon.GetDisplayName() + " ";
        if (_currentCharIndex < namePrefix.Length)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, namePrefix.Remove(_currentCharIndex), new Vector2(120, 20), Color.White);
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont, namePrefix, new Vector2(120, 20), Color.White);
        }
        if (_currentCharIndex > namePrefix.Length)
        {
            Texture2D pokeTexture = _pokemon.GetMenuTexture();
            Vector2 pokeTextureScale = new Vector2((float)(32.0 / pokeTexture.Width), (float)(32.0 / pokeTexture.Height));
            Core.SpriteBatch.Draw(pokeTexture, new Rectangle(
                (int)(FontManager.MainFont.MeasureString(_pokemon.GetDisplayName()).X + 120 + (int)(FontManager.MainFont.MeasureString(" ").X / 2)),
                12,
                (int)(pokeTexture.Width * pokeTextureScale.X),
                (int)(pokeTexture.Height * pokeTextureScale.Y)), Color.White);
        }
        if (_currentCharIndex > namePrefix.Length + 1)
        {
            String t = " " + (Localization.GetString("learn_move_AlreadyKnowsFourMoves1", "wants to learn \"{MOVENAME}\". But [POKEMONNAME] can only learn 4 moves.") + Environment.NewLine + " " +
                Localization.GetString("learn_move_AlreadyKnowsFourMoves2", "Do you want [POKEMONNAME] to forget a move to learn \"[MOVENAME]\"?"))
                .Replace("[MOVENAME]", _newAttacks[0].Name).Replace("[POKEMONNAME]", _pokemon.GetDisplayName());
            if (_currentCharIndex < GetText().Length)
                Core.SpriteBatch.DrawString(FontManager.MainFont, t.Remove(_currentCharIndex - namePrefix.Length), new Vector2(FontManager.MainFont.MeasureString(_pokemon.GetDisplayName()).X + 152, 20), Color.White);
            else
                Core.SpriteBatch.DrawString(FontManager.MainFont, t, new Vector2(FontManager.MainFont.MeasureString(_pokemon.GetDisplayName()).X + 152, 20), Color.White);
        }
    }

    private String GetText()
    {
        return _pokemon.GetDisplayName() + " " +
            (Localization.GetString("learn_move_AlreadyKnowsFourMoves1", "wants to learn \"[MOVENAME]\". But [POKEMONNAME] can only learn 4 moves.") + Environment.NewLine + " " +
            Localization.GetString("learn_move_AlreadyKnowsFourMoves2", "Do you want [POKEMONNAME] to forget a move to learn \"[MOVENAME]\"?"))
            .Replace("[MOVENAME]", _newAttacks[0].Name).Replace("[POKEMONNAME]", _pokemon.GetDisplayName());
    }

    private void DrawAttack(Vector2 startPosition, int i, BattleSystem.Attack a)
    {
        Vector2 p = new Vector2(startPosition.X + 16, startPosition.Y + 32 + i * (64 + 32));

        if (i == 4)
            p.Y += 80;

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Color fontColor = Color.Black;
        if (_attackIndex == i)
        {
            canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            fontColor = Color.White;
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)p.X + 16, (int)p.Y, 288, 64));

        if (fontColor != Color.Black)
            Core.SpriteBatch.DrawString(FontManager.MainFont, a.Name, new Vector2((int)p.X + 34 + 2, (int)(p.Y + 26 + 2)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, a.Name, new Vector2((int)p.X + 34, (int)(p.Y + 26)), fontColor);

        Color c = fontColor;
        int per = (int)((float)a.CurrentPP / a.MaxPP * 100);
        if (per <= 33 && per > 10) c = Color.Orange;
        else if (per <= 10) c = Color.IndianRed;

        if (c != Color.Black)
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(p.X + 96 + 2, (int)(p.Y + 56 + 2)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(p.X + 96, (int)(p.Y + 56)), c);
        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath(), a.Type.GetElementImage(), String.Empty),
            new Rectangle((int)(p.X + 34), (int)(p.Y + 56), 48, 16), Color.White);
    }

    private void ClickYes()
    {
        if (_canForget == true)
        {
            String text = Localization.GetString("learn_move_DidNotLearnMove", "[POKEMONNAME] didn't~learn [MOVENAME]!").Replace("[POKEMONNAME]", _pokemon.GetDisplayName()).Replace("[MOVENAME]", _newAttacks[0].Name);

            if (_attackIndex != 4)
            {
                TeachMovesScreen.LearnedMove = true;
                text = Localization.GetString("learn_move_PokemonForgotMove", "1... 2... 3... and...*Ta-da!*[POKEMONNAME] forgot~[OLDMOVENAME] and...").Replace("[POKEMONNAME]", _pokemon.GetDisplayName()).Replace("[OLDMOVENAME]", _pokemon.attacks[_attackIndex].Name);
                _pokemon.attacks.RemoveAt(_attackIndex);
                _pokemon.attacks.Insert(_attackIndex, _newAttacks[0]);

                if (_machineItemID != "-1")
                {
                    PlayerStatistics.Track("TMs/HMs used", 1);
                    if (bool.Parse(GameModeManager.GetGameRuleValue("SingleUseTM", "0")) == true)
                    {
                        Items.Item techMachine = Items.Item.GetItemByID(_machineItemID);
                        if (techMachine.ItemType == Items.ItemTypes.Machines)
                        {
                            if (techMachine.IsGameModeItem == true)
                            {
                                if (((Items.GameModeItem)techMachine).gmIsHM == false)
                                    techMachine.RemoveItem();
                            }
                            else
                            {
                                if (((Items.TechMachine)techMachine).IsTM == true)
                                    techMachine.RemoveItem();
                            }
                        }
                    }
                }
                PlayerStatistics.Track("Moves learned", 1);
                TextBox.FollowUp = FollowUpText;
            }

            TextBox.Show(text, [], false, false);
            Core.GameInstance.IsMouseVisible = false;
            if (_newAttacks.Count > 1)
            {
                _newAttacks.RemoveAt(0);
                Core.SetScreen(new LearnAttackScreen(PreScreen!, _pokemon, _newAttacks));
            }
            else
            {
                Core.SetScreen(PreScreen!);
            }
        }
    }

    private void ClickNo()
    {
        _chosen = false;
        Core.GameInstance.IsMouseVisible = false;
    }

    private void FollowUpText()
    {
        TextBox.Show(Localization.GetString("learn_move_PokemonLearnedMove_WithDots", "... [POKEMONNAME] learned~[MOVENAME]!").Replace("[POKEMONNAME]", _pokemon.GetDisplayName()).Replace("[MOVENAME]", _newAttacks[0].Name));
        SoundManager.PlaySound("success_small", true);
    }
}
