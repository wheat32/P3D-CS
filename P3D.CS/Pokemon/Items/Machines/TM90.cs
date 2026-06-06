using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(390, "TM 90")]
public class TM90 : TechMachine
{
    public TM90() : base(true, 1000, 352)
    {
    }

}
