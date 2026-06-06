using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(412, "TM 112")]
public class TM112 : TechMachine
{
    public TM112() : base(true, 2000, 206)
    {
    }

}
