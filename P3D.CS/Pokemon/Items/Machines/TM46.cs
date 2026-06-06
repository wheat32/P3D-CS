using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(236, "TM 46")]
public class TM46 : TechMachine
{
    public TM46() : base(true, 3000, 168)
    {
    }

}
