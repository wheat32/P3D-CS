using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(362, "TM 62")]
public class TM62 : TechMachine
{
    public TM62() : base(true, 4000, 58)
    {
    }

}
