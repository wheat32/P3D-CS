using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(435, "TM 135")]
public class TM135 : TechMachine
{
    public TM135() : base(true, 3000, 398)
    {
    }

}
