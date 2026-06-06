using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(451, "TM 151")]
public class TM151 : TechMachine
{
    public TM151() : base(true, 2000, 502)
    {
    }

}
