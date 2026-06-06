using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(462, "TM 162")]
public class TM162 : TechMachine
{
    public TM162() : base(true, 4000, 525)
    {
    }

}
