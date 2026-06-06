using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(464, "TM 164")]
public class TM164 : TechMachine
{
    public TM164() : base(true, 5000, 528)
    {
    }

}
