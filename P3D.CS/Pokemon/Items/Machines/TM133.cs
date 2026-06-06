using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(433, "TM 133")]
public class TM133 : TechMachine
{
    public TM133() : base(true, 3000, 404)
    {
    }

}
