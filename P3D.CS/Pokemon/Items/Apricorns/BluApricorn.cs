using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(89, "Blue Apricorn")]
public class BluApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A blue Apricorn. It smells a bit like grass.";
    public BluApricorn()
    {
        _textureRectangle = new Rectangle(336, 72, 24, 24);
    }

}
