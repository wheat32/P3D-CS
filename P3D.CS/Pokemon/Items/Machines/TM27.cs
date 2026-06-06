using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(217, "TM 27")]
public class TM27 : TechMachine
{
    public TM27() : base(true, 1000, 216)
    {
        CanTeachAlways = true;
    }

}
