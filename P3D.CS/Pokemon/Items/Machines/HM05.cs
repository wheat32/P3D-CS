using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(243, "HM 05")]
public class HM05 : TechMachine
{
    public HM05() : base(false, 100, 148)
    {
        _textureRectangle = new Rectangle(432, 312, 24, 24);
    }

}
