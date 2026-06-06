using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(355, "TM 55")]
public class TM55 : TechMachine
{
    public TM55() : base(true, 3000, 25)
    {
    }

}
