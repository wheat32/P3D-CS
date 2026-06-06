using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(220, "TM 30")]
public class TM30 : TechMachine
{
    public TM30() : base(true, 1500, 247)
    {
    }

}
