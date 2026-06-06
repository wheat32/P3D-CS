using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(425, "TM 125")]
public class TM125 : TechMachine
{
    public TM125() : base(true, 1500, 397)
    {
    }

}
