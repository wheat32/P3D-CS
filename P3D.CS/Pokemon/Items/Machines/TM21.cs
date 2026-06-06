using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(211, "TM 21")]
public class TM21 : TechMachine
{
    public TM21() : base(true, 1000, 218)
    {
        CanTeachAlways = true;
    }

}
