using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(450, "TM 150")]
public class TM150 : TechMachine
{
    public TM150() : base(true, 2000, 497)
    {
    }

}
