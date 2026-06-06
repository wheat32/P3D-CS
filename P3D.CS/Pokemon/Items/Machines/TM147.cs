using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(447, "TM 147")]
public class TM147 : TechMachine
{
    public TM147() : base(true, 3000, 488)
    {
    }

}
