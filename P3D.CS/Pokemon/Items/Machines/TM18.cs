using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(208, "TM 18")]
public class TM18 : TechMachine
{
    public TM18() : base(true, 2000, 240)
    {
    }

}
