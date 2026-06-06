using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(93, "Green Apricorn")]
public class GrnApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A green Apricorn. It has a mysterious, aromatic scent.";
    public GrnApricorn()
    {
        _textureRectangle = new Rectangle(408, 72, 24, 24);
    }

}
