using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(230, "TM 40")]
public class TM40 : TechMachine
{
    public TM40() : base(true, 1000, 111)
    {
    }

}
