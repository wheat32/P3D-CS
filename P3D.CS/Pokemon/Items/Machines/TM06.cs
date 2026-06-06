using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(196, "TM 06")]
public class TM06 : TechMachine
{
    public override int BattlePointsPrice { get; } = 32;
    public TM06() : base(true, 3000, 92)
    {
        CanTeachAlways = true;
    }

}
