using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public static class Canvas
{
    private const int CANVAS_TEX_SIZE = 1;
    private const int MAX_GRADIENT_TEXTURE_DIM = 2048;
    private const int COLOR_MAX = 255;

    private static Texture2D _canvas = null!;

    public static void SetupCanvas()
    {
        _canvas = new Texture2D(Core.GraphicsDevice, CANVAS_TEX_SIZE, CANVAS_TEX_SIZE);
        _canvas.SetData([ Color.White ]);
    }

    public static void DrawRectangle(Rectangle rectangle, Color color)
    {
        Core.SpriteBatch.Draw(_canvas, rectangle, color);
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        spriteBatch.Draw(_canvas, rectangle, color);
    }

    public static void DrawRectangle(Rectangle rectangle, Color color, bool scaleToScreen)
    {
        if (scaleToScreen == true)
        {
            Core.SpriteBatch.DrawInterface(_canvas, rectangle, color);
        }
        else
        {
            Core.SpriteBatch.Draw(_canvas, rectangle, color);
        }
    }

    // Borders

    public static void DrawBorder(int borderLength, Rectangle rectangle, Color color)
    {
        DrawBorder(borderLength, rectangle, color, false);
    }

    public static void DrawBorder(int borderLength, Rectangle rectangle, Color color, bool scaleToScreen)
    {
        DrawRectangle(new Rectangle(rectangle.X + borderLength, rectangle.Y, rectangle.Width - borderLength, borderLength), color, scaleToScreen);
        DrawRectangle(new Rectangle(rectangle.X + rectangle.Width - borderLength, rectangle.Y + borderLength, borderLength, rectangle.Height - borderLength), color, scaleToScreen);
        DrawRectangle(new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - borderLength, rectangle.Width - borderLength, borderLength), color, scaleToScreen);
        DrawRectangle(new Rectangle(rectangle.X, rectangle.Y, borderLength, rectangle.Height - borderLength), color, scaleToScreen);
    }

    public static void DrawImageBorder(Texture2D texture, int sizeMultiplier, Rectangle rectangle)
    {
        DrawImageBorder(texture, sizeMultiplier, rectangle, Color.White, false);
    }

    public static void DrawImageBorder(Texture2D texture, int sizeMultiplier, Rectangle rectangle, bool scaleToScreen)
    {
        DrawImageBorder(texture, sizeMultiplier, rectangle, Color.White, scaleToScreen);
    }

    public static void DrawImageBorder(Texture2D texture, int sizeMultiplier, Rectangle rectangle, Color color, bool scaleToScreen)
    {
        Vector2 borderSize = new Vector2(rectangle.Width, rectangle.Height);
        int tileW = (int)Math.Floor(texture.Width / 3f);
        int tileH = (int)Math.Floor(texture.Height / 3f);
        int step = tileW * sizeMultiplier;

        for (int x = 0; x <= (int)borderSize.X; x += step)
        {
            for (int y = 0; y <= (int)borderSize.Y; y += step)
            {
                Rectangle tile = new Rectangle(tileW, tileH, tileW, tileH);
                if (x == 0 && y == 0)                              tile = new Rectangle(0, 0, tileW, tileH);
                else if (x == (int)borderSize.X && y == 0)        tile = new Rectangle(tileW * 2, 0, tileW, tileH);
                else if (x == 0 && y == (int)borderSize.Y)        tile = new Rectangle(0, tileH * 2, tileW, tileH);
                else if (x == (int)borderSize.X && y == (int)borderSize.Y) tile = new Rectangle(tileW * 2, tileH * 2, tileW, tileH);
                else if (x == 0)                                   tile = new Rectangle(0, tileH, tileW, tileH);
                else if (y == 0)                                   tile = new Rectangle(tileW, 0, tileW, tileH);
                else if (x == (int)borderSize.X)                  tile = new Rectangle(tileW * 2, tileH, tileW, tileH);
                else if (y == (int)borderSize.Y)                  tile = new Rectangle(tileW, tileH * 2, tileW, tileH);

                Rectangle dest = new Rectangle(x + rectangle.X, y + rectangle.Y,
                    sizeMultiplier * tileW, sizeMultiplier * tileH);
                if (scaleToScreen == true)
                {
                    Core.SpriteBatch.DrawInterface(texture, dest, tile, Color.White);
                }
                else
                {
                    Core.SpriteBatch.Draw(texture, dest, tile, Color.White);
                }
            }
        }
    }

    // Scroll Bars

    public static void DrawScrollBar(Vector2 position, int allItems, int seeableItems, int selection,
        Size size, bool horizontal, Color color1, Color color2)
    {
        DrawScrollBar(position, allItems, seeableItems, selection, size, horizontal, color1, color2, false);
    }

    public static void DrawScrollBar(Vector2 position, int allItems, int seeableItems, int selection,
        Size size, bool horizontal, Color color1, Color color2, bool scaleToScreen)
    {
        DrawRectangle(new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height), color1, scaleToScreen);
        if (horizontal == false)
        {
            int sizeY = allItems > seeableItems
                ? (int)((seeableItems / (float)allItems) * size.Height)
                : size.Height;
            int posY = allItems > seeableItems
                ? (int)(Math.Abs(selection) * size.Height / (float)allItems)
                : 0;
            DrawRectangle(new Rectangle((int)position.X, (int)position.Y + posY, size.Width, sizeY), color2, scaleToScreen);
        }
        else
        {
            int sizeX = allItems > seeableItems
                ? (int)((seeableItems / (float)allItems) * size.Width)
                : size.Width;
            int posX = allItems > seeableItems
                ? (int)(Math.Abs(selection) * size.Width / (float)allItems)
                : 0;
            DrawRectangle(new Rectangle((int)position.X + posX, (int)position.Y, sizeX, size.Height), color2, scaleToScreen);
        }
    }

    public static void DrawScrollBar(Vector2 position, int allItems, int seeableItems, int selection,
        Size size, bool horizontal, Texture2D texture1, Texture2D texture2)
    {
        DrawScrollBar(position, allItems, seeableItems, selection, size, horizontal, texture1, texture2, false);
    }

    public static void DrawScrollBar(Vector2 position, int allItems, int seeableItems, int selection,
        Size size, bool horizontal, Texture2D texture1, Texture2D texture2, bool scaleToScreen)
    {
        Rectangle bgRect = new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height);
        if (scaleToScreen == true)
        {
            Core.SpriteBatch.DrawInterface(texture1, bgRect, Color.White);
        }
        else
        {
            Core.SpriteBatch.Draw(texture1, bgRect, Color.White);
        }

        if (horizontal == false)
        {
            int sizeY = allItems > seeableItems
                ? (int)((seeableItems / (float)allItems) * size.Height)
                : size.Height;
            int posY = allItems > seeableItems
                ? (int)(Math.Abs(selection) * size.Height / (float)allItems)
                : 0;
            Rectangle thumbRect = new Rectangle((int)position.X, (int)position.Y + posY, size.Width, sizeY);
            if (scaleToScreen == true)
                Core.SpriteBatch.DrawInterface(texture2, thumbRect, Color.White);
            else
                Core.SpriteBatch.Draw(texture2, thumbRect, Color.White);
        }
        else
        {
            int sizeX = allItems > seeableItems
                ? (int)((seeableItems / (float)allItems) * size.Width)
                : size.Width;
            int posX = allItems > seeableItems
                ? (int)(Math.Abs(selection) * size.Width / (float)allItems)
                : 0;
            Rectangle thumbRect = new Rectangle((int)position.X + posX, (int)position.Y, sizeX, size.Height);
            if (scaleToScreen == true)
                Core.SpriteBatch.DrawInterface(texture2, thumbRect, Color.White);
            else
                Core.SpriteBatch.Draw(texture2, thumbRect, Color.White);
        }
    }

    // Gradients

    private struct GradientConfiguration
    {
        private Texture2D _texture;
        private readonly int _width, _height, _steps;
        private readonly Color _from, _to;
        private readonly bool _horizontal;

        public GradientConfiguration(int width, int height, Color from, Color to, bool horizontal, int steps)
        {
            _width = width;
            _height = height;
            _from = from;
            _to = to;
            _horizontal = horizontal;
            _steps = steps;
            _texture = null!;
            GenerateTexture();
        }

        private void GenerateTexture()
        {
            int uSize = _horizontal ? _height : _width;
            int stepCount = _steps < 0 ? uSize : _steps;
            float stepSize = (float)Math.Ceiling((float)uSize / stepCount);

            int diffR = _to.R - _from.R;
            int diffG = _to.G - _from.G;
            int diffB = _to.B - _from.B;
            int diffA = _to.A - _from.A;

            Color[] colorArray = new Color[_width * _height];

            for (int cStep = 1; cStep <= stepCount; cStep++)
            {
                int cR = Math.Clamp((int)((diffR / (float)stepCount) * cStep) + _from.R, 0, COLOR_MAX);
                int cG = Math.Clamp((int)((diffG / (float)stepCount) * cStep) + _from.G, 0, COLOR_MAX);
                int cB = Math.Clamp((int)((diffB / (float)stepCount) * cStep) + _from.B, 0, COLOR_MAX);
                int cA = Math.Clamp((int)((diffA / (float)stepCount) * cStep) + _from.A, 0, COLOR_MAX);
                Color c = new Color(cR, cG, cB, cA);

                int length = (int)Math.Ceiling(stepSize);
                int start = (int)((cStep - 1) * stepSize);

                if (_horizontal == true)
                {
                    for (int row = start; row < start + length && row < _height; row++)
                    {
                        for (int col = 0; col < _width; col++)
                        {
                            colorArray[col + row * _width] = c;
                        }
                    }
                }
                else
                {
                    for (int col = start; col < start + length && col < _width; col++)
                    {
                        for (int row = 0; row < _height; row++)
                        {
                            colorArray[col + row * _width] = c;
                        }
                    }
                }
            }

            _texture = new Texture2D(Core.GraphicsDevice, _width, _height);
            _texture.SetData(colorArray);
        }

        public bool IsConfig(int width, int height, Color from, Color to, bool horizontal, int steps)
        {
            return _width == width && _height == height &&
                   _from == from && _to == to &&
                   _horizontal == horizontal && _steps == steps;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle r)
        {
            spriteBatch.Draw(_texture, r, Color.White);
        }
    }

    private static List<GradientConfiguration> _gradientConfigs = [];

    public static void DrawGradient(Rectangle rectangle, Color from, Color to, bool horizontal, int steps)
    {
        DrawGradient(Core.SpriteBatch, rectangle, from, to, horizontal, steps);
    }

    public static void DrawGradient(SpriteBatch spriteBatch, Rectangle rectangle, Color from, Color to,
        bool horizontal, int steps)
    {
        if (rectangle.Width <= 0 || rectangle.Height <= 0)
        {
            return;
        }

        horizontal = !horizontal;
        int tw = Math.Min(rectangle.Width, MAX_GRADIENT_TEXTURE_DIM);
        int th = Math.Min(rectangle.Height, MAX_GRADIENT_TEXTURE_DIM);

        GradientConfiguration gConfig = default;
        bool found = false;
        foreach (GradientConfiguration g in _gradientConfigs)
        {
            if (g.IsConfig(tw, th, from, to, horizontal, steps) == true)
            {
                gConfig = g;
                found = true;
                break;
            }
        }
        if (found == false)
        {
            gConfig = new GradientConfiguration(tw, th, from, to, horizontal, steps);
            _gradientConfigs.Add(gConfig);
        }
        gConfig.Draw(spriteBatch, rectangle);
    }

    // Lines

    public static void DrawLine(Color color, Vector2 startPoint, Vector2 endPoint, double width)
    {
        double angle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
        double length = Vector2.Distance(startPoint, endPoint);
        Core.SpriteBatch.Draw(_canvas, startPoint, null, color,
            (float)angle, Vector2.Zero,
            new Vector2((float)length, (float)width),
            SpriteEffects.None, 0);
    }
}
