using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(420, "TM 120")]
public class TM120 : TechMachine
{
    public TM120() : base(true, 2000, 373)
    {
    }

}
