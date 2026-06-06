using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(399, "TM 99")]
public class TM99 : TechMachine
{
    public TM99() : base(true, 1500, 351)
    {
    }

}
