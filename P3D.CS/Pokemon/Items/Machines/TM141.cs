using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(441, "TM 141")]
public class TM141 : TechMachine
{
    public TM141() : base(true, 3000, 468)
    {
    }

}
