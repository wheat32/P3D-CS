using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(465, "TM 165")]
public class TM165 : TechMachine
{
    public TM165() : base(true, 3000, 555)
    {
    }

}
