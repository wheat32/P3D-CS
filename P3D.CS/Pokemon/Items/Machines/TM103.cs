using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(403, "TM 103")]
public class TM103 : TechMachine
{
    public TM103() : base(true, 1500, 259)
    {
    }

}
