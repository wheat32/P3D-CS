using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(432, "TM 132")]
public class TM132 : TechMachine
{
    public TM132() : base(true, 3000, 399)
    {
    }

}
