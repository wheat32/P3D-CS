using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(238, "TM 48")]
public class TM48 : TechMachine
{
    public TM48() : base(true, 3000, 7)
    {
    }

}
