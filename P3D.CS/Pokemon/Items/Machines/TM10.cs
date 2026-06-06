using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(200, "TM 10")]
public class TM10 : TechMachine
{
    public TM10() : base(true, 3000, 237)
    {
        CanTeachAlways = true;
    }

}
