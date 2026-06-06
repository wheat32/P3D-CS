using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(392, "TM 92")]
public class TM92 : TechMachine
{
    public TM92() : base(true, 3000, 258)
    {
    }

}
