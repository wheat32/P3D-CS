using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(213, "TM 23")]
public class TM23 : TechMachine
{
    public TM23() : base(true, 1500, 231)
    {
    }

}
