using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(445, "TM 145")]
public class TM145 : TechMachine
{
    public TM145() : base(true, 2000, 479)
    {
    }

}
