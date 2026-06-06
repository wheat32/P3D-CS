using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(398, "TM 98")]
public class TM98 : TechMachine
{
    public TM98() : base(true, 3000, 280)
    {
    }

}
