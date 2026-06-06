using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(358, "TM 58")]
public class TM58 : TechMachine
{
    public TM58() : base(true, 3000, 36)
    {
    }

}
