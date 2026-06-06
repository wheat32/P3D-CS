using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(372, "TM 72")]
public class TM72 : TechMachine
{
    public TM72() : base(true, 1000, 100)
    {
    }

}
