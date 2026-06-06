using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(429, "TM 129")]
public class TM129 : TechMachine
{
    public TM129() : base(true, 3000, 360)
    {
    }

}
