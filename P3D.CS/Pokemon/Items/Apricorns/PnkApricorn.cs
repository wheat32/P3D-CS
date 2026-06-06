using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(101, "Pink Apricorn")]
public class PnkApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A pink Apricorn. It has a nice, sweet scent.";
    public PnkApricorn()
    {
        _textureRectangle = new Rectangle(72, 96, 24, 24);
    }

}
