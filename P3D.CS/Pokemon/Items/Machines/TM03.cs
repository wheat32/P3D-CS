using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(193, "TM 03")]
public class TM03 : TechMachine
{
    public TM03() : base(true, 3000, 174)
    {
        CanTeachAlways = true;
    }

}
