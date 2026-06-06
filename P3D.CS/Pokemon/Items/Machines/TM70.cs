using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(370, "TM 70")]
public class TM70 : TechMachine
{
    public TM70() : base(true, 2000, 85)
    {
    }

}
