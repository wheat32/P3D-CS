using Microsoft.Xna.Framework;
using P3D.BattleSystem;

namespace P3D.Items.Machines;

[Item(351, "TM 51")]
public class TM51 : TechMachine
{
    public TM51() : base(true, 3000, 5)
    {
    }

}
