using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(248, "HM 06")]
public class HM06 : TechMachine
{
    public HM06() : base(false, 100, 250)
    {
        _textureRectangle = new Rectangle(96, 192, 24, 24);
    }

}
