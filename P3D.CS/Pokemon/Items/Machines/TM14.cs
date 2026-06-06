using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(204, "TM 14")]
public class TM14 : TechMachine
{
    public TM14() : base(true, 5500, 59)
    {
    }

}
