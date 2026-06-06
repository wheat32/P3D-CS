using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(354, "TM 54")]
public class TM54 : TechMachine
{
    public TM54() : base(true, 1000, 18)
    {
    }

}
