using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(400, "TM 100")]
public class TM100 : TechMachine
{
    public TM100() : base(true, 3000, 53)
    {
    }

}
