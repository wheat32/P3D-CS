using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(449, "TM 149")]
public class TM149 : TechMachine
{
    public TM149() : base(true, 2500, 496)
    {
        CanTeachAlways = true;
    }

}
