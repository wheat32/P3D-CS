using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(225, "TM 35")]
public class TM35 : TechMachine
{
    public TM35() : base(true, 3000, 214)
    {
        CanTeachAlways = true;
    }

}
