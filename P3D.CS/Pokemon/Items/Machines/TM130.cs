using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(430, "TM 130")]
public class TM130 : TechMachine
{
    public TM130() : base(true, 2000, 446)
    {
    }

}
