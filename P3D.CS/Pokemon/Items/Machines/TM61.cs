using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(361, "TM 61")]
public class TM61 : TechMachine
{
    public TM61() : base(true, 1000, 55)
    {
    }

}
