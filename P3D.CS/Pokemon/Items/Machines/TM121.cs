using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(421, "TM 121")]
public class TM121 : TechMachine
{
    public TM121() : base(true, 3000, 421)
    {
    }

}
