using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(228, "TM 38")]
public class TM38 : TechMachine
{
    public TM38() : base(true, 5500, 126)
    {
    }

}
