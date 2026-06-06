using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(244, "HM 02")]
public class HM02 : TechMachine
{
    public HM02() : base(false, 100, 19)
    {
        _textureRectangle = new Rectangle(73, 192, 24, 24);
    }

}
