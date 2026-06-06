using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(424, "TM 124")]
public class TM124 : TechMachine
{
    public TM124() : base(true, 3000, 416)
    {
        CanTeachWhenFullyEvolved = true;
    }

}
