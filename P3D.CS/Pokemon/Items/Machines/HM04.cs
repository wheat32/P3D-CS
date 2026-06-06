using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(246, "HM 04")]
public class HM04 : TechMachine
{
    public HM04() : base(false, 100, 70)
    {
        _textureRectangle = new Rectangle(48, 192, 24, 24);
    }

}
