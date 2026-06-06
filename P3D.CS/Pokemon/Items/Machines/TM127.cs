using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(427, "TM 127")]
public class TM127 : TechMachine
{
    public TM127() : base(true, 3000, 444)
    {
    }

}
