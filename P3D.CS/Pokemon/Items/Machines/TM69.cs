using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(369, "TM 69")]
public class TM69 : TechMachine
{
    public TM69() : base(true, 5000, 82)
    {
    }

}
