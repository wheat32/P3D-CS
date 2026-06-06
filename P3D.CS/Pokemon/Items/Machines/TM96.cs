using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(396, "TM 96")]
public class TM96 : TechMachine
{
    public TM96() : base(true, 2000, 113)
    {
    }

}
