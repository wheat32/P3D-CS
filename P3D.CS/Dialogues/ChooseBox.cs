using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class ChooseBox
{
    public delegate void DoAnswer(int result);

    public String[] Options = [];
    public int Index;
    public bool Showing;
    public bool readyForResult;
    public int result;
    public int ResultID;
    public bool ActionScript;
    public static int CancelIndex = -1;
    public FontContainer? TextFont;
    public bool DoDelegate;
    public Entity[]? UpdateEntities;

    private DoAnswer? _subs;

    public void Show(String[] options, DoAnswer doSubs)
    {
        ResultID = 0;
        Options = options;
        Index = 0;
        readyForResult = false;
        Showing = true;
        _subs = doSubs;
        ActionScript = false;
        DoDelegate = true;
        TextFont = FontManager.GetFontContainer("textfont");
        SetupOptions();
    }

    public void Show(String[] options, int id, bool actionScript)
    {
        ResultID = id;
        Options = options;
        Index = 0;
        readyForResult = false;
        Showing = true;
        ActionScript = actionScript;
        DoDelegate = false;
        TextFont = FontManager.GetFontContainer("textfont");
        SetupOptions();
    }

    public void Show(String[] options, int id, Entity[] entities)
    {
        ResultID = id;
        Options = options;
        Index = 0;
        readyForResult = false;
        Showing = true;
        UpdateEntities = entities;
        ActionScript = false;
        DoDelegate = false;
        TextFont = FontManager.GetFontContainer("textfont");
        SetupOptions();
    }

    private void SetupOptions()
    {
        for (int i = 0; i < Options.Length; i++)
        {
            Options[i] = Options[i]
                .Replace("<playername>", Core.Player.Name)
                .Replace("<player.name>", Core.Player.Name);
        }
    }

    public int GetResult(int id)
    {
        if (readyForResult == true && ResultID == id)
            return result;
        return -1;
    }

    public void Update() => Update(true);

    public void Update(bool raiseClickEvent)
    {
        if (Showing == false)
            return;

        if (Controls.Down(true, true, true) == true)
            Index += 1;
        if (Controls.Up(true, true, true) == true)
            Index -= 1;

        if (Index < 0)
            Index = Options.Length - 1;
        if (Index == Options.Length)
            Index = 0;

        if (raiseClickEvent == true)
        {
            if (Controls.Accept() == true)
            {
                PlayClickSound();
                result = Index;
                HandleResult();
            }
            if (Controls.Dismiss() == true && CancelIndex > -1)
            {
                PlayClickSound();
                result = CancelIndex;
                HandleResult();
            }
        }
    }

    private void PlayClickSound()
    {
        if (Screen.TextBox.Showing == false)
            SoundManager.PlaySound("select");
    }

    private void HandleResult()
    {
        ChooseBox.CancelIndex = -1;
        readyForResult = true;
        Showing = false;
        if (DoDelegate == true)
        {
            _subs!(result);
        }
        else
        {
            if (Core.CurrentScreen.Identification == Screen.Identifications.OverworldScreen ||
                Core.CurrentScreen.Identification == Screen.Identifications.NewGameScreen)
            {
                if (ActionScript == true)
                {
                    OverworldScreen c = (OverworldScreen)Core.CurrentScreen;
                    c.ActionScript.Switch(Options[result]);
                }
                else
                {
                    foreach (Entity entity in UpdateEntities!)
                        entity.ResultFunction(result);
                }
            }
        }
    }

    public void Draw()
    {
        if (Showing == false)
            return;

        float scale = (float)Math.Ceiling(Core.SpriteBatch.InterfaceScale());
        Vector2 position = new Vector2(
            (int)(Core.windowSize.Width / 2 - 48 * scale),
            Core.windowSize.Height - (int)(160.0f * scale) - (int)(96.0f * scale) - (int)(Options.Length - 1) * 48 * scale);
        Draw(position);
    }

    public void Draw(Vector2 position, bool drawBox = true, float size = 1.0f)
    {
        if (Showing == false)
            return;

        float scale = (float)Math.Ceiling(Core.SpriteBatch.InterfaceScale());
        int sizeMultiplier = (int)(3 * scale);

        if (drawBox == true)
        {
            int maxWidth = 0;
            for (int i = 0; i < Options.Length; i++)
            {
                while (TextFont!.SpriteFont.MeasureString(Options[i].Replace("[POKE]", "Poké")).X - 16 > maxWidth)
                    maxWidth += 16;
                if (maxWidth < 48)
                    maxWidth = 48;
            }
            Canvas.DrawImageBorder(
                TextureManager.GetTexture(@"GUI\Overworld\ChooseBox", new Rectangle(0, 0, 48, 48), String.Empty),
                sizeMultiplier,
                new Rectangle((int)position.X, (int)position.Y,
                    (int)(maxWidth * sizeMultiplier * size),
                    (int)(48 * size * scale * Options.Length)),
                false);
        }

        for (int i = 0; i < Options.Length; i++)
        {
            float useSize = size;
            if (TextFont != null && (TextFont.FontName.ToLower() == "textfont" || TextFont.FontName.ToLower() == "braille"))
                useSize = 2 * size;
            useSize = (int)(useSize * scale);
            Core.SpriteBatch.DrawString(
                TextFont!.SpriteFont,
                Options[i].Replace("[POKE]", "Poké"),
                new Vector2((int)(position.X + 48 * scale), (int)position.Y + (int)((32 + i * 48 * size) * scale)),
                Color.Black, 0f, Vector2.Zero, useSize, SpriteEffects.None, 0f);
        }

        Core.SpriteBatch.Draw(
            TextureManager.GetTexture(@"GUI\Overworld\ChooseBox"),
            new Rectangle(
                (int)(position.X + 24 * scale),
                (int)(position.Y + (34 + Index * 48 * size) * scale),
                (int)(24 * size * scale),
                (int)(24 * size * scale)),
            new Rectangle(72, 0, 8, 8), Color.White);
    }
}
