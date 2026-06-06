using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(454, "TM 154")]
public class TM154 : TechMachine
{
    public TM154() : base(true, 4000, 510)
    {
    }

}
