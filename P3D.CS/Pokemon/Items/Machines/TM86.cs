using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(386, "TM 86")]
public class TM86 : TechMachine
{
    public TM86() : base(true, 4000, 161)
    {
    }

}
