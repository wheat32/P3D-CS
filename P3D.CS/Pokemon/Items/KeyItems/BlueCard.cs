using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.KeyItems;

[Item(116, "Blue Card")]
public class BlueCard : KeyItem
{
    public override String Description { get; protected set; } = "A card to save points for the Buena's Password show.";
    public BlueCard()
    {
        _textureRectangle = new Rectangle(408, 96, 24, 24);
    }

}
