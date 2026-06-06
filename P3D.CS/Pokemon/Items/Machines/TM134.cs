using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(434, "TM 134")]
public class TM134 : TechMachine
{
    public TM134() : base(true, 2000, 363)
    {
        CanTeachAlways = true;
    }

}
