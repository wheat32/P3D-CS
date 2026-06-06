using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(224, "TM 34")]
public class TM34 : TechMachine
{
    public TM34() : base(true, 1000, 207)
    {
        CanTeachAlways = true;
    }

}
