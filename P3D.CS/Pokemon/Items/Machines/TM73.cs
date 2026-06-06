using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(373, "TM 73")]
public class TM73 : TechMachine
{
    public TM73() : base(true, 2000, 102)
    {
        CanTeachAlways = true;
    }

}
