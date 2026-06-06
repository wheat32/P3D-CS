using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(374, "TM 74")]
public class TM74 : TechMachine
{
    public TM74() : base(true, 1000, 115)
    {
    }

}
