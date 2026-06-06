using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(222, "TM 32")]
public class TM32 : TechMachine
{
    public TM32() : base(true, 2000, 104)
    {
        CanTeachAlways = true;
    }

}
