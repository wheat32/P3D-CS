using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(381, "TM 81")]
public class TM81 : TechMachine
{
    public TM81() : base(true, 5000, 143)
    {
    }

}
