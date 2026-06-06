using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(404, "TM 104")]
public class TM104 : TechMachine
{
    public TM104() : base(true, 3000, 263)
    {
        CanTeachAlways = true;
    }

}
