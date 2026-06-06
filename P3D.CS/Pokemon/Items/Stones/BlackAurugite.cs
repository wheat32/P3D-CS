using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(653, "Black Augurite")]
public class BlackAugurite : StoneItem
{
    public override String Description { get; protected set; } = "A glassy black stone that produces a sharp cutting edge when split. It’s loved by a certain Pokémon.";
    public BlackAugurite()
    {
        _textureRectangle = new Rectangle(384, 408, 24, 24);
    }

}
