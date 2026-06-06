using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(366, "TM 66")]
public class TM66 : TechMachine
{
    public TM66() : base(true, 3000, 69)
    {
    }

}
