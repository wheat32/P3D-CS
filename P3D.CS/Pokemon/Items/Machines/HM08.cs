using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(250, "HM 08")]
public class HM08 : TechMachine
{
    public HM08() : base(false, 100, 431)
    {
        _textureRectangle = new Rectangle(48, 192, 24, 24);
    }

}
