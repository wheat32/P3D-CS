using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(459, "TM 159")]
public class TM159 : TechMachine
{
    public TM159() : base(true, 3000, 522)
    {
    }

}
