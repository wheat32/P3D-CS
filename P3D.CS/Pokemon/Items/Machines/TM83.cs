using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(383, "TM 83")]
public class TM83 : TechMachine
{
    public TM83() : base(true, 2000, 149)
    {
    }

}
