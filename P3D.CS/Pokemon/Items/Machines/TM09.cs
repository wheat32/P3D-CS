using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(199, "TM 09")]
public class TM09 : TechMachine
{
    public TM09() : base(true, 1000, 244)
    {
    }

}
