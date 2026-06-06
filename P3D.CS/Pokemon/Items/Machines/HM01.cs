using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(247, "HM 01")]
public class HM01 : TechMachine
{
    public HM01() : base(false, 100, 15)
    {
        _textureRectangle = new Rectangle(48, 192, 24, 24);
    }

}
