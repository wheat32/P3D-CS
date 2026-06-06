using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(205, "TM 15")]
public class TM15 : TechMachine
{
    public TM15() : base(true, 3000, 63)
    {
        CanTeachWhenFullyEvolved = true;
    }

}
