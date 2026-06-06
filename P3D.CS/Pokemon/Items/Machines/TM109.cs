using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(409, "TM 109")]
public class TM109 : TechMachine
{
    public TM109() : base(true, 2000, 355)
    {
    }

}
