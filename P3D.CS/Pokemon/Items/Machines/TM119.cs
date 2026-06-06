using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(419, "TM 119")]
public class TM119 : TechMachine
{
    public TM119() : base(true, 3000, 318)
    {
    }

}
