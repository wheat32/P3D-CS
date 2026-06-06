using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(442, "TM 142")]
public class TM142 : TechMachine
{
    public TM142() : base(true, 3000, 473)
    {
    }

}
