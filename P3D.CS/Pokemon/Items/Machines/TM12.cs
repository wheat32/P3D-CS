using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(202, "TM 12")]
public class TM12 : TechMachine
{
    public TM12() : base(true, 1000, 230)
    {
    }

}
