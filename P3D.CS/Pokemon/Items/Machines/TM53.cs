using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(353, "TM 53")]
public class TM53 : TechMachine
{
    public TM53() : base(true, 2000, 14)
    {
    }

}
