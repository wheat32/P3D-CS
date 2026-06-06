using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(207, "TM 17")]
public class TM17 : TechMachine
{
    public TM17() : base(true, 3000, 182)
    {
        CanTeachAlways = true;
    }

}
