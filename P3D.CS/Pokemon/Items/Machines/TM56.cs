using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(356, "TM 56")]
public class TM56 : TechMachine
{
    public TM56() : base(true, 2000, 32)
    {
    }

}
