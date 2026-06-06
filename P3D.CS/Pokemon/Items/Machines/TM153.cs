using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(453, "TM 153")]
public class TM153 : TechMachine
{
    public TM153() : base(true, 3000, 507)
    {
    }

}
