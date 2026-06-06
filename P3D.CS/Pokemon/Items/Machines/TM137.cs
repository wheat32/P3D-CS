using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(437, "TM 137")]
public class TM137 : TechMachine
{
    public TM137() : base(true, 3000, 365)
    {
    }

}
