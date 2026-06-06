using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(251, "HM 09")]
public class HM09 : TechMachine
{
    public HM09() : base(false, 100, 560)
    {
        _textureRectangle = new Rectangle(120, 192, 24, 24);
    }

}
