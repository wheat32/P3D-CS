using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(231, "TM 41")]
public class TM41 : TechMachine
{
    public TM41() : base(true, 3000, 9)
    {
    }

}
