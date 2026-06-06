using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(191, "TM 01")]
public class TM01 : TechMachine
{
    public TM01() : base(true, 1500, 223)
    {
    }

}
