using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(379, "TM 79")]
public class TM79 : TechMachine
{
    public TM79() : base(true, 4000, 130)
    {
    }

}
