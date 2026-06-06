using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(468, "TM 168")]
public class TM168 : TechMachine
{
    public TM168() : base(true, 4000, 612)
    {
    }

}
