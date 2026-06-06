using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(391, "TM 91")]
public class TM91 : TechMachine
{
    public TM91() : base(true, 1500, 347)
    {
    }

}
