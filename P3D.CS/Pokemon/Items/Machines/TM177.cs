using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(477, "TM 177")]
public class TM177 : TechMachine
{
    public TM177() : base(true, 2000, 200)
    {
    }

}
