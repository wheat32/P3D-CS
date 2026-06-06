using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(195, "TM 05")]
public class TM05 : TechMachine
{
    public TM05() : base(true, 500, 46)
    {
    }

}
