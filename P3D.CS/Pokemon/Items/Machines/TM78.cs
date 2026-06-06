using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(378, "TM 78")]
public class TM78 : TechMachine
{
    public TM78() : base(true, 2000, 121)
    {
    }

}
