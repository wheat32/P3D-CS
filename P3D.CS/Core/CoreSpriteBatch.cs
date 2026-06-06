using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

/// <summary>SpriteBatch wrapper with interface-scaling and canvas drawing helpers.</summary>
public class CoreSpriteBatch : SpriteBatch
{
    private const int CANVAS_WIDTH = 1;
    private const int CANVAS_HEIGHT = 1;
    private const double SCALE_SMALL = 0.5;
    private const double SCALE_NORMAL = 1.0;
    private const double SCALE_LARGE = 2.0;

    private bool _running;
    private Texture2D _canvasTexture = null!;

    public CoreSpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        SetupCanvas();
    }

    // State management

    public void BeginBatch()
    {
        Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied,
              SamplerState.PointClamp, DepthStencilState.Default,
              RasterizerState.CullCounterClockwise);
        _running = true;
    }

    public void EndBatch()
    {
        End();
        _running = false;
    }

    public bool Running => _running;

    // DrawInterface (Rectangle overloads)

    public void DrawInterface(Texture2D texture, Rectangle dest, Color color)
    {
        DrawInterface(texture, dest, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f, true);
    }

    public void DrawInterface(Texture2D texture, Rectangle dest, Rectangle? src, Color color)
    {
        DrawInterface(texture, dest, src, color, 0f, Vector2.Zero, SpriteEffects.None, 0f, true);
    }

    public void DrawInterface(Texture2D texture, Rectangle dest, Rectangle? src, Color color,
        float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        DrawInterface(texture, dest, src, color, rotation, origin, effects, layerDepth, true);
    }

    public void DrawInterface(Texture2D texture, Rectangle dest, Rectangle? src, Color color,
        float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, bool transformPosition)
    {
        double scale = InterfaceScale();
        if (transformPosition == true)
        {
            dest = new Rectangle(
                (int)(dest.X * scale), (int)(dest.Y * scale),
                (int)(dest.Width * scale), (int)(dest.Height * scale));
        }
        else
        {
            dest = new Rectangle(
                (int)(dest.X * scale), (int)(dest.Y * scale),
                dest.Width, dest.Height);
        }
        Draw(texture, dest, src, color, rotation, origin, effects, layerDepth);
    }

    // DrawInterface (Vector2 overloads)

    public void DrawInterface(Texture2D texture, Vector2 pos, Color color)
    {
        DrawInterface(texture, pos, null, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f, true);
    }

    public void DrawInterface(Texture2D texture, Vector2 pos, Rectangle? src, Color color)
    {
        DrawInterface(texture, pos, src, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f, true);
    }

    public void DrawInterface(Texture2D texture, Vector2 pos, Rectangle? src, Color color,
        float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        DrawInterface(texture, pos, src, color, rotation, origin, new Vector2(scale), effects, layerDepth, true);
    }

    public void DrawInterface(Texture2D texture, Vector2 pos, Rectangle? src, Color color,
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        DrawInterface(texture, pos, src, color, rotation, origin, scale, effects, layerDepth, true);
    }

    public void DrawInterface(Texture2D texture, Vector2 pos, Rectangle? src, Color color,
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth,
        bool transformPosition)
    {
        double x = InterfaceScale();
        if (transformPosition == true)
        {
            pos = new Vector2((float)(pos.X * x), (float)(pos.Y * x));
        }
        scale = new Vector2((float)(scale.X * x), (float)(scale.Y * x));
        Draw(texture, pos, src, color, rotation, origin, scale, effects, layerDepth);
    }

    // DrawInterfaceString

    public void DrawInterfaceString(SpriteFont font, String text, Vector2 pos, Color color)
    {
        DrawInterfaceString(font, new StringBuilder(text), pos, color,
            0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f, true);
    }

    public void DrawInterfaceString(SpriteFont font, String text, Vector2 pos, Color color,
        float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        DrawInterfaceString(font, new StringBuilder(text), pos, color,
            rotation, origin, new Vector2(scale), effects, layerDepth, true);
    }

    public void DrawInterfaceString(SpriteFont font, String text, Vector2 pos, Color color,
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        DrawInterfaceString(font, new StringBuilder(text), pos, color,
            rotation, origin, scale, effects, layerDepth, true);
    }

    public void DrawInterfaceString(SpriteFont font, StringBuilder text, Vector2 pos, Color color)
    {
        DrawInterfaceString(font, text, pos, color,
            0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f, true);
    }

    public void DrawInterfaceString(SpriteFont font, StringBuilder text, Vector2 pos, Color color,
        float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        DrawInterfaceString(font, text, pos, color,
            rotation, origin, new Vector2(scale), effects, layerDepth, true);
    }

    public void DrawInterfaceString(SpriteFont font, StringBuilder text, Vector2 pos, Color color,
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        DrawInterfaceString(font, text, pos, color, rotation, origin, scale, effects, layerDepth, true);
    }

    public void DrawInterfaceString(SpriteFont font, StringBuilder text, Vector2 pos, Color color,
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth,
        bool transformPosition)
    {
        double x = InterfaceScale();
        if (transformPosition == true)
        {
            pos = new Vector2((float)(pos.X * x), (float)(pos.Y * x));
        }
        scale = new Vector2((float)(scale.X * x), (float)(scale.Y * x));
        DrawString(font, text, pos, color, rotation, origin, scale, effects, layerDepth);
    }

    // Interface scale

    public double InterfaceScale()
    {
        switch (Core.GameOptions.InterfaceScale)
        {
            case 1:
                return SCALE_SMALL;

            case 2:
                return SCALE_NORMAL;

            case 3:
                return SCALE_LARGE;

            default:
            {
                Size min = Core.CurrentScreen.GetScreenScaleMinimum();
                Size max = Core.CurrentScreen.GetScreenScaleMaximum();
                if (Core.windowSize.Height < min.Height || Core.windowSize.Width < min.Width)
                {
                    return SCALE_SMALL;
                }
                if (Core.windowSize.Height > max.Height || Core.windowSize.Width > max.Width)
                {
                    return SCALE_LARGE;
                }
                return SCALE_NORMAL;
            }
        }
    }

    // Canvas

    private void SetupCanvas()
    {
        _canvasTexture = new Texture2D(GraphicsDevice, CANVAS_WIDTH, CANVAS_HEIGHT);
        _canvasTexture.SetData([ Color.White ]);
    }

    public void DrawRectangle(Rectangle dest, Color color)
    {
        DrawRectangle(dest, color, 0f, Vector2.Zero, 0f, false);
    }

    public void DrawRectangle(Rectangle dest, Color color, float rotation, Vector2 origin, float layerDepth)
    {
        DrawRectangle(dest, color, rotation, origin, layerDepth, false);
    }

    public void DrawRectangle(Rectangle dest, Color color, float rotation, Vector2 origin,
                               float layerDepth, bool scaleToScreen)
    {
        if (scaleToScreen == true)
        {
            Core.SpriteBatch.DrawInterface(_canvasTexture, dest, null, color,
                rotation, origin, SpriteEffects.None, layerDepth, true);
        }
        else
        {
            Core.SpriteBatch.Draw(_canvasTexture, dest, null, color,
                rotation, origin, SpriteEffects.None, layerDepth);
        }
    }

    public void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
    {
        double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
        double length = Vector2.Distance(start, end);
        Core.SpriteBatch.Draw(_canvasTexture, start, null, color,
            (float)angle, Vector2.Zero,
            new Vector2((float)length, thickness),
            SpriteEffects.None, 0);
    }
}
