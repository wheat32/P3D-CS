using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(405, "TM 105")]
public class TM105 : TechMachine
{
    public TM105() : base(true, 2000, 290)
    {
        CanTeachAlways = true;
    }

}
