using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(415, "TM 115")]
public class TM115 : TechMachine
{
    public TM115() : base(true, 3000, 451)
    {
    }

}
