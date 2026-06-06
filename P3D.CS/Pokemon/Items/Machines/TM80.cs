using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(380, "TM 80")]
public class TM80 : TechMachine
{
    public TM80() : base(true, 2000, 135)
    {
    }

}
