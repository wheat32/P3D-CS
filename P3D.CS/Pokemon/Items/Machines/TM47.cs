using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(237, "TM 47")]
public class TM47 : TechMachine
{
    public TM47() : base(true, 3000, 211)
    {
    }

}
