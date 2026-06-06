using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(656, "Exp. Charm")]
public class ExpCharm : KeyItem
{
    public override String Description { get; protected set; } = "Having one of these charms increases the Exp. Points your Pokémon get. It's a strange, stretchy charm that encourages growth.";
    public ExpCharm()
    {
        _textureRectangle = new Rectangle(456, 408, 24, 24);
    }

}
