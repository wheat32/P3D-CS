using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(252, "HM 10")]
public class HM10 : TechMachine
{
    public HM10() : base(false, 100, 291)
    {
        _textureRectangle = new Rectangle(96, 192, 24, 24);
    }

}
