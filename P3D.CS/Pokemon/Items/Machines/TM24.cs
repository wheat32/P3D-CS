using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(214, "TM 24")]
public class TM24 : TechMachine
{
    public TM24() : base(true, 3000, 225)
    {
    }

}
