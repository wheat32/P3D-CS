using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(384, "TM 84")]
public class TM84 : TechMachine
{
    public TM84() : base(true, 3000, 153)
    {
    }

}
