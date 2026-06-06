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
        {
            SoundManager.PlaySound(sound);
        }
        _texture = texture;
    }

    public void Update() { }
    public void Draw() { }
}
