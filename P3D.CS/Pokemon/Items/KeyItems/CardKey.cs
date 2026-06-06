using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(127, "Card Key")]
public class CardKey : KeyItem
{
    public override String Description { get; protected set; } = "A card key that opens a shutter in the Radio Tower.";
    public CardKey()
    {
        _textureRectangle = new Rectangle(144, 120, 24, 24);
    }

}
