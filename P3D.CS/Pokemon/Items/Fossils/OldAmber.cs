using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Standard;

[Item(603, "Old Amber")]
public class OldAmber : FossilItem
{
    public override String Description { get; protected set; } = "A piece of amber that still contains the genetic material of an ancient Pokémon. It's clear with a tawny, reddish tint.";
    public OldAmber()
    {
        _textureRectangle = new Rectangle(48, 0, 24, 24);
    }

}
