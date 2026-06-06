using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(470, "TM 170")]
public class TM170 : TechMachine
{
    public TM170() : base(true, 2000, 590)
    {
    }

}
