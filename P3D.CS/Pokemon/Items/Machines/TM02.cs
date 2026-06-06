using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(192, "TM 02")]
public class TM02 : TechMachine
{
    public TM02() : base(true, 2000, 29)
    {
    }

}
