using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(387, "TM 87")]
public class TM87 : TechMachine
{
    public TM87() : base(true, 2000, 164)
    {
        CanTeachAlways = true;
    }

}
