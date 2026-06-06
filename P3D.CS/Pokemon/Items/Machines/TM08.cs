using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(198, "TM 08")]
public class TM08 : TechMachine
{
    public TM08() : base(true, 1000, 249)
    {
    }

}
