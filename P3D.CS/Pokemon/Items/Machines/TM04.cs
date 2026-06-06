using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(194, "TM 04")]
public class TM04 : TechMachine
{
    public TM04() : base(true, 2000, 205)
    {
    }

}
