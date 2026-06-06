using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(365, "TM 65")]
public class TM65 : TechMachine
{
    public TM65() : base(true, 2000, 68)
    {
    }

}
