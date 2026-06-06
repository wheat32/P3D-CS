using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(406, "TM 106")]
public class TM106 : TechMachine
{
    public TM106() : base(true, 3000, 285)
    {
    }

}
