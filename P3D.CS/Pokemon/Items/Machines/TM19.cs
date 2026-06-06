using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(209, "TM 19")]
public class TM19 : TechMachine
{
    public TM19() : base(true, 3000, 202)
    {
    }

}
