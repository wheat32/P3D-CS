using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(394, "TM 94")]
public class TM94 : TechMachine
{
    public TM94() : base(true, 2000, 331)
    {
    }

}
