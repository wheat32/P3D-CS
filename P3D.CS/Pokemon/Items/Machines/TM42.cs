using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(232, "TM 42")]
public class TM42 : TechMachine
{
    public TM42() : base(true, 3000, 138)
    {
    }

}
