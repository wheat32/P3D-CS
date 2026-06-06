using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(480, "TM 180")]
public class TM180 : TechMachine
{
    public TM180() : base(true, 2000, 529)
    {
    }

}
