using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(92, "Yellow Apricorn")]
public class YlwApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A yellow Apricorn. It has an invigorating scent.";
    public YlwApricorn()
    {
        _textureRectangle = new Rectangle(384, 72, 24, 24);
    }

}
