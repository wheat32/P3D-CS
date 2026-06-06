using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(85, "Red Apricorn")]
public class RedApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A red Apricorn. It assails your nostrils.";
    public RedApricorn()
    {
        _textureRectangle = new Rectangle(240, 72, 24, 24);
    }

}
