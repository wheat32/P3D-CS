using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(401, "TM 101")]
public class TM101 : TechMachine
{
    public TM101() : base(true, 2000, 317)
    {
    }

}
