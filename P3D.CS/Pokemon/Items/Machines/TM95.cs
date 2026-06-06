using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(395, "TM 95")]
public class TM95 : TechMachine
{
    public TM95() : base(true, 1500, 269)
    {
    }

}
