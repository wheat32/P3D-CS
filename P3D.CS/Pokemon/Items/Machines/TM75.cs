using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(375, "TM 75")]
public class TM75 : TechMachine
{
    public TM75() : base(true, 2000, 117)
    {
        CanTeachAlways = true;
    }

}
