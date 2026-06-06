using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(363, "TM 63")]
public class TM63 : TechMachine
{
    public TM63() : base(true, 5000, 6)
    {
    }

}
