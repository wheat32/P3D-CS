using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(203, "TM 13")]
public class TM13 : TechMachine
{
    public TM13() : base(true, 1000, 173)
    {
        CanTeachAlways = true;
    }

}
