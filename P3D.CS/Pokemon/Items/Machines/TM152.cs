using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(452, "TM 152")]
public class TM152 : TechMachine
{
    public TM152() : base(true, 4000, 503)
    {
    }

}
