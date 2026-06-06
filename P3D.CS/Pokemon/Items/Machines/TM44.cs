using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(234, "TM 44")]
public class TM44 : TechMachine
{
    public TM44() : base(true, 3000, 156)
    {
        CanTeachAlways = true;
    }

}
