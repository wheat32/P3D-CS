using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(210, "TM 20")]
public class TM20 : TechMachine
{
    public TM20() : base(true, 3000, 203)
    {
        CanTeachAlways = true;
    }

}
