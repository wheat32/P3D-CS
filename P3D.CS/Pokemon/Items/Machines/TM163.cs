using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(463, "TM 163")]
public class TM163 : TechMachine
{
    public TM163() : base(true, 3000, 526)
    {
    }

}
