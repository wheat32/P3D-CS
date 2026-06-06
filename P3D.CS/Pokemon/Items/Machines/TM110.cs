using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(410, "TM 110")]
public class TM110 : TechMachine
{
    public TM110() : base(true, 5500, 411)
    {
    }

}
