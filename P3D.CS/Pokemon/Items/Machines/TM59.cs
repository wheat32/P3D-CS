using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(359, "TM 59")]
public class TM59 : TechMachine
{
    public TM59() : base(true, 4000, 38)
    {
    }

}
