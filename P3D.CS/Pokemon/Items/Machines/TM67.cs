using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(367, "TM 67")]
public class TM67 : TechMachine
{
    public TM67() : base(true, 2000, 99)
    {
        CanTeachAlways = true;
    }

}
