using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(416, "TM 116")]
public class TM116 : TechMachine
{
    public TM116() : base(true, 3000, 406)
    {
    }

}
