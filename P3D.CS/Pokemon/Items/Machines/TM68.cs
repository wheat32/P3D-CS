using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(368, "TM 68")]
public class TM68 : TechMachine
{
    public TM68() : base(true, 5000, 72)
    {
    }

}
