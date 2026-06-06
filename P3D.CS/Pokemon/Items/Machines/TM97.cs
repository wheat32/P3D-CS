using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(397, "TM 97")]
public class TM97 : TechMachine
{
    public TM97() : base(true, 2000, 219)
    {
    }

}
