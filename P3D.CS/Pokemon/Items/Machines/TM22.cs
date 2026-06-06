using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(212, "TM 22")]
public class TM22 : TechMachine
{
    public TM22() : base(true, 3000, 76)
    {
    }

}
