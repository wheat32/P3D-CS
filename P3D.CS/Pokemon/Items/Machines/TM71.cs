using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(371, "TM 71")]
public class TM71 : TechMachine
{
    public TM71() : base(true, 5000, 90)
    {
    }

}
