using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(99, "Black Apricorn")]
public class BlkApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A black Apricorn It has an indescribable scent.";
    public BlkApricorn()
    {
        _textureRectangle = new Rectangle(48, 96, 24, 24);
    }

}
