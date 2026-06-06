using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(456, "TM 156")]
public class TM156 : TechMachine
{
    public TM156() : base(true, 4000, 512)
    {
    }

}
