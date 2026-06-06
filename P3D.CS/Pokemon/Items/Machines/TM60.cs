using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(360, "TM 60")]
public class TM60 : TechMachine
{
    public TM60() : base(true, 2000, 61)
    {
    }

}
