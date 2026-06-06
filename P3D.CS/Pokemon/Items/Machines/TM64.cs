using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(364, "TM 64")]
public class TM64 : TechMachine
{
    public TM64() : base(true, 3000, 66)
    {
    }

}
