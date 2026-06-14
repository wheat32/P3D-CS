using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D;

public class TeachMovesScreen : Screen
{
    private List<BattleSystem.Attack> _movesList = [];
    private Pokemon _pokemon;
    private Texture2D _mainTexture;
    private int _index = 0;
    private int _scrollIndex = 0;

    public static bool LearnedMove = false;

    public TeachMovesScreen(Screen currentScreen, int pokemonIndex)
    {
        PreScreen = currentScreen;
        _pokemon = Core.Player.Pokemons[pokemonIndex];
        Identification = Identifications.TeachMovesScreen;

        for (int i = 0; i <= _pokemon.attackLearns.Count - 1; i++)
        {
            List<BattleSystem.Attack> aList = _pokemon.attackLearns.Values.ElementAt(i);
            for (int a = 0; a <= aList.Count - 1; a++)
            {
                BattleSystem.Attack tutorMove = aList[a];
                int learnLevel = _pokemon.attackLearns.Keys.ElementAt(i);

                if (learnLevel <= _pokemon.Level)
                {
                    bool canLearnMove = true;

                    foreach (BattleSystem.Attack learnedAttack in _pokemon.attacks)
                        if (learnedAttack.ID == tutorMove.ID) canLearnMove = false;

                    foreach (BattleSystem.Attack move in _movesList)
                        if (move.ID == tutorMove.ID) canLearnMove = false;

                    if (canLearnMove == true)
                        _movesList.Add(tutorMove);
                }
            }
        }

        MouseVisible = false;
        CanBePaused = true;
        CanMuteAudio = true;
        LearnedMove = false;
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
    }

    public TeachMovesScreen(Screen currentScreen, int pokemonIndex, BattleSystem.Attack[] movesList)
    {
        PreScreen = currentScreen;
        _pokemon = Core.Player.Pokemons[pokemonIndex];
        Identification = Identifications.TeachMovesScreen;

        foreach (BattleSystem.Attack a in movesList)
        {
            bool canLearnMove = false;

            foreach (BattleSystem.Attack tutorMove in _pokemon.tutorAttacks)
                if (a.ID == tutorMove.ID) canLearnMove = true;

            for (int i = 0; i <= _pokemon.attackLearns.Count - 1; i++)
            {
                List<BattleSystem.Attack> aList = _pokemon.attackLearns.Values.ElementAt(i);
                for (int lA = 0; lA <= aList.Count - 1; lA++)
                {
                    BattleSystem.Attack learnAttack = aList[lA];
                    if (learnAttack.ID == a.ID) canLearnMove = true;
                }
            }

            foreach (int eggMoveID in _pokemon.eggMoves)
                if (eggMoveID == a.ID) canLearnMove = true;

            foreach (int tMMoveID in _pokemon.machines)
                if (tMMoveID == a.ID) canLearnMove = true;

            foreach (BattleSystem.Attack learnedAttack in _pokemon.attacks)
                if (learnedAttack.ID == a.ID) canLearnMove = false;

            foreach (BattleSystem.Attack move in _movesList)
                if (move.ID == a.ID) canLearnMove = false;

            if (canLearnMove == true)
                _movesList.Add(a);
        }

        MouseVisible = false;
        CanBePaused = true;
        CanMuteAudio = true;
        LearnedMove = false;
        _mainTexture = TextureManager.GetTexture(@"GUI\Menus\Menu");
    }

    public override void Draw()
    {
        PreScreen?.Draw();
        Canvas.DrawRectangle(new Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), new Color(0, 0, 0, 150));

        Texture2D borderTexture = TextureManager.GetTexture(_mainTexture, new Rectangle(0, 0, 48, 48));

        if (_movesList.Count > 0)
        {
            Canvas.DrawImageBorder(borderTexture, 2, new Rectangle(60, 100, 864 + 320 + 64, 480));

            BattleSystem.Attack a = _movesList[_index];
            String description = a.Description.Replace("'", "'").CropStringToWidth(FontManager.MainFont, 188);

            String power = a.Power.ToString();
            if (power == "0") power = "-";

            String acc = a.Accuracy.ToString();
            if (acc == "0") acc = "-";

            Core.SpriteBatch.DrawString(FontManager.MainFont,
                Localization.GetString("property_Power", "Power") + ": " + power + Environment.NewLine +
                Localization.GetString("property_Accuracy", "Accuracy") + ": " + acc + Environment.NewLine + Environment.NewLine + description,
                new Vector2(60 + 864 + 48 + 64, 128), Color.Black);
            Core.SpriteBatch.Draw(a.GetDamageCategoryImage(),
                new Rectangle(60 + 864 + 320 + 64 - 16 - 56, 124, 56, 28), Color.White);
        }

        Canvas.DrawImageBorder(borderTexture, 2, new Rectangle(60, 100, 864 + 64, 480));

        Texture2D pokeTexture = _pokemon.GetTexture(true);
        Core.SpriteBatch.Draw(pokeTexture, new Rectangle(
            176 - MathHelper.Min(pokeTexture.Width, 128),
            208 - MathHelper.Min(pokeTexture.Height, 128),
            MathHelper.Min(pokeTexture.Width * 2, 256),
            MathHelper.Min(pokeTexture.Height * 2, 256)), Color.White);

        Core.SpriteBatch.DrawString(FontManager.MainFont,
            _pokemon.GetDisplayName() + Environment.NewLine +
            Localization.GetString("property_Level", "Level") + ": " + _pokemon.Level,
            new Vector2(80, 304), Color.Black);

        Core.SpriteBatch.DrawString(FontManager.MainFont,
            Localization.GetString("move_tutor_screen_CurrentMoves", "Pokémon's moves") + ":",
            new Vector2(312 - 16, 128), Color.Black);

        for (int i = 0; i <= _pokemon.attacks.Count - 1; i++)
            DrawAttack(312, i, _pokemon.attacks[i], false);

        if (_movesList.Count == 0)
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont,
                Localization.GetString("move_tutor_screen_CannotLearnNewMove", "The Pokémon cannot learn~a new move here.")
                    .Replace("~", Environment.NewLine).Replace("*", Environment.NewLine),
                new Vector2(588 + 32, 128), Color.Black);
        }
        else
        {
            Core.SpriteBatch.DrawString(FontManager.MainFont,
                Localization.GetString("move_tutor_screen_TutorMoves", "Tutor moves") + " (" + _movesList.Count + "):",
                new Vector2(644 + 32 - 16, 128), Color.Black);

            for (int i = _scrollIndex; i <= _scrollIndex + 3; i++)
            {
                if (i <= _movesList.Count - 1)
                    DrawAttack(644 + 32, i, _movesList[i], true);
            }
        }

        Dictionary<Buttons, String> d = [];
        d.Add(Buttons.A, Localization.GetString("game_interaction_learn", "Learn"));
        d.Add(Buttons.B, Localization.GetString("game_interaction_close", "Close"));
        DrawGamePadControls(d);
    }

    private void DrawAttack(int x, int i, BattleSystem.Attack a, bool isLearnMove)
    {
        int y = isLearnMove == false ? i : i - _scrollIndex;
        Vector2 p = new Vector2(x, 160 + y * (64 + 32));

        Texture2D canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty);
        Color fontColor = Color.Black;
        if (_index == i && isLearnMove == true)
        {
            canvasTexture = TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 48, 48, 48), String.Empty);
            fontColor = Color.White;
        }

        Canvas.DrawImageBorder(canvasTexture, 2, new Rectangle((int)(p.X - 18), (int)p.Y, 256 + 32, 64));

        if (fontColor != Color.Black)
            Core.SpriteBatch.DrawString(FontManager.MainFont, a.Name, new Vector2(p.X + 2, (int)(p.Y + 26 + 2)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, a.Name, new Vector2(p.X, (int)(p.Y + 26)), fontColor);

        Color c = fontColor;
        int per = (int)((float)a.CurrentPP / a.MaxPP * 100);
        if (per <= 33 && per > 10) c = Color.Orange;
        else if (per <= 10) c = Color.IndianRed;

        if (c != Color.Black)
            Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(p.X + 96 - 34 + 2, (int)(p.Y + 56 + 2)), Color.Black);
        Core.SpriteBatch.DrawString(FontManager.MainFont, Localization.GetString("property_PP", "PP") + " " + a.CurrentPP + " / " + a.MaxPP, new Vector2(p.X + 96 - 34, (int)(p.Y + 56)), c);
        Core.SpriteBatch.Draw(TextureManager.GetTexture(Element.GetElementTexturePath(), a.Type.GetElementImage(), String.Empty),
            new Rectangle((int)p.X, (int)(p.Y + 56), 48, 16), Color.White);
    }

    public override void Update()
    {
        if (Controls.Up(true, true, true, true, true) == true) _index -= 1;
        if (Controls.Down(true, true, true, true, true) == true) _index += 1;

        _index = _index.Clamp(0, _movesList.Count - 1);

        if (_index - _scrollIndex > 3) _scrollIndex += 1;
        if (_index - _scrollIndex < 0) _scrollIndex -= 1;

        if (Controls.Accept(true, true, true) == true)
        {
            if (_movesList.Count == 0)
            {
                Core.SetScreen(PreScreen!);
                SoundManager.PlaySound("select");
            }
            else
            {
                LearnMove(_movesList[_index]);
                SoundManager.PlaySound("select");
            }
        }

        if (Controls.Dismiss(true, true, true) == true)
        {
            Core.SetScreen(PreScreen!);
            SoundManager.PlaySound("select");
        }
    }

    private void LearnMove(BattleSystem.Attack a)
    {
        if (_pokemon.attacks.Count < 4)
        {
            LearnedMove = true;
            _pokemon.attacks.Add(BattleSystem.Attack.GetAttackByID(a.ID));
            TextBox.Show(Localization.GetString("learn_move_PokemonLearnedMove_WithDots", "... [POKEMONNAME] learned~[MOVENAME]!")
                .Replace("[POKEMONNAME]", _pokemon.GetDisplayName())
                .Replace("[MOVENAME]", a.Name));
            SoundManager.PlaySound("success_small", false);
            Core.SetScreen(PreScreen!);
        }
        else
        {
            Core.SetScreen(new LearnAttackScreen(Core.CurrentScreen!.PreScreen!, _pokemon, BattleSystem.Attack.GetAttackByID(a.ID)));
        }
    }
}
