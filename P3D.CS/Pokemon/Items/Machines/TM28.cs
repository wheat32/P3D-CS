using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(218, "TM 28")]
public class TM28 : TechMachine
{
    public TM28() : base(true, 2000, 91)
    {
    }

}
