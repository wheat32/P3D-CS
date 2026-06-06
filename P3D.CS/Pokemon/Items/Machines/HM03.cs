using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(245, "HM 03")]
public class HM03 : TechMachine
{
    public HM03() : base(false, 100, 57)
    {
        _textureRectangle = new Rectangle(96, 192, 24, 24);
    }

}
