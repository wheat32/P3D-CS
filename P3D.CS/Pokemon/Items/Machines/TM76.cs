using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(376, "TM 76")]
public class TM76 : TechMachine
{
    public TM76() : base(true, 4000, 118)
    {
    }

}
