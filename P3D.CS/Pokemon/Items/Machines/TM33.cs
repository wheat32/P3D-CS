using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(223, "TM 33")]
public class TM33 : TechMachine
{
    public TM33() : base(true, 3000, 8)
    {
    }

}
