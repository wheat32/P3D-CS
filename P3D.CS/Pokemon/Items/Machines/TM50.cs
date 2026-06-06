using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(240, "TM 50")]
public class TM50 : TechMachine
{
    public TM50() : base(true, 2000, 171)
    {
    }

}
