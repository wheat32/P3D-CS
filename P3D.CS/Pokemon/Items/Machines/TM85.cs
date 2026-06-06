using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(385, "TM 85")]
public class TM85 : TechMachine
{
    public TM85() : base(true, 3000, 157)
    {
    }

}
