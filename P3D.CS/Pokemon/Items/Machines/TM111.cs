using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(411, "TM 111")]
public class TM111 : TechMachine
{
    public TM111() : base(true, 3000, 412)
    {
    }

}
