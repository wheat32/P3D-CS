using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(408, "TM 108")]
public class TM108 : TechMachine
{
    public TM108() : base(true, 5500, 315)
    {
    }

}
