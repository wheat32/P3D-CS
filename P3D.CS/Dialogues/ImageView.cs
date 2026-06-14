using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D;

public class ImageView
{
    public bool Showing;
    public float Delay;

    private Texture2D? _texture;

    public void Show(Texture2D texture, String sound)
    {
        Delay = 8.0f;
        Showing = true;
        if (String.IsNullOrEmpty(sound) == false)
            SoundManager.PlaySound(sound);
        _texture = texture;
    }

    public void Update()
    {
        if (Delay > 0.0f)
        {
            Delay -= 0.1f;
            if (Delay <= 0.0f)
                Delay = 0.0f;
        }
        else if (Delay == 0.0f)
        {
            if (Controls.Accept() == true || Controls.Dismiss() == true)
            {
                Showing = false;
                SoundManager.PlaySound("select");
            }
        }
    }

    public void Draw()
    {
        if (Showing == false || _texture == null)
            return;

        int frameW = 288;
        int frameH = 288;
        int imageW = 288;
        int imageH = 288;
        const int defaultSize = 288;

        if (_texture.Width > _texture.Height)
        {
            frameW = (int)Math.Ceiling((double)_texture.Width / _texture.Height) * 288;
            imageW = (int)(_texture.Width / (float)_texture.Height * 288);
            int scaleInt = 9;
            while (imageW > Core.windowSize.Width - 288)
            {
                imageW = (int)(_texture.Width / (float)_texture.Height * 32 * scaleInt);
                imageH = (int)(defaultSize / 9.0 * scaleInt);
                frameW = (int)(Math.Ceiling((double)_texture.Width / _texture.Height) * 32 * scaleInt);
                frameH = imageH;
                scaleInt--;
            }
            while (imageW > frameW - 32)
                frameW += 32;
        }

        if (_texture.Height > _texture.Width)
        {
            frameH = (int)Math.Floor((double)_texture.Height / _texture.Width) * 288;
            imageH = (int)(_texture.Height / (float)_texture.Width * 288);
            int scaleInt = 9;
            while (imageH > Core.windowSize.Height - 288)
            {
                imageH = (int)(_texture.Height / (float)_texture.Width * 32 * scaleInt);
                imageW = (int)(defaultSize / 9.0 * scaleInt);
                frameH = (int)(Math.Floor((double)_texture.Height / _texture.Width) * 32 * scaleInt);
                frameW = imageW;
                scaleInt--;
            }
            while (imageH > frameH - 32)
                frameH += 32;
        }

        Vector2 p = Core.GetMiddleInterfacePosition(new Size(frameW + 64, frameH + 64));
        Vector2 pImage = Core.GetMiddleInterfacePosition(new Size(imageW + 64, imageH + 64));

        Canvas.DrawImageBorder(
            TextureManager.GetTexture(@"GUI\Menus\Menu", new Rectangle(0, 0, 48, 48), String.Empty),
            2,
            new Rectangle((int)p.X - 32, (int)p.Y - 32, frameW + 64, frameH + 64),
            true);

        if (Delay == 0.0f)
        {
            Core.SpriteBatch.DrawInterface(
                TextureManager.GetTexture(@"GUI\Overworld\ImageView"),
                new Rectangle((int)p.X - 32 + frameW + 64, (int)p.Y - 32 + frameH + 64 + 16, 16, 16),
                new Rectangle(0, 0, 16, 16), Color.White);
        }

        Core.SpriteBatch.DrawInterface(_texture,
            new Rectangle((int)pImage.X + 16, (int)pImage.Y + 16, imageW, imageH),
            Color.White);
    }
}
