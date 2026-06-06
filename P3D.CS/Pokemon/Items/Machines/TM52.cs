using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(352, "TM 52")]
public class TM52 : TechMachine
{
    public TM52() : base(true, 2000, 13)
    {
    }

}
