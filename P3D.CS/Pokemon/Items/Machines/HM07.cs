using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(249, "HM 07")]
public class HM07 : TechMachine
{
    public HM07() : base(false, 100, 127)
    {
        _textureRectangle = new Rectangle(96, 192, 24, 24);
    }

}
