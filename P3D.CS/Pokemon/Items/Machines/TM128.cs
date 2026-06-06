using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(428, "TM 128")]
public class TM128 : TechMachine
{
    public TM128() : base(true, 3000, 419)
    {
    }

}
