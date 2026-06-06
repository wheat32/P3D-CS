using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Apricorns;

[Item(97, "White Apricorn")]
public class WhtApricorn : Apricorn
{
    public override String Description { get; protected set; } = "A white Apricorn. It doesn't smell like anything.";
    public WhtApricorn()
    {
        _textureRectangle = new Rectangle(0, 96, 24, 24);
    }

}
