using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(440, "TM 140")]
public class TM140 : TechMachine
{
    public TM140() : base(true, 5500, 433)
    {
    }

}
