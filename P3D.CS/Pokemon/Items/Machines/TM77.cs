using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(377, "TM 77")]
public class TM77 : TechMachine
{
    public TM77() : base(true, 2000, 120)
    {
    }

}
