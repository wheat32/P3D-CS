using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(382, "TM 82")]
public class TM82 : TechMachine
{
    public TM82() : base(true, 2000, 86)
    {
    }

}
