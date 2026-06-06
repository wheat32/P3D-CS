using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(418, "TM 118")]
public class TM118 : TechMachine
{
    public TM118() : base(true, 2000, 261)
    {
    }

}
